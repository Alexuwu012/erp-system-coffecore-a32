using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class ProductosControl : BaseGridControl
{
    protected override string ModuleTitle => "🍽  Productos & Categorias";

    TextBox txNombre=null!, txDesc=null!;
    NumericUpDown numPrecio=null!, numCosto=null!;
    ComboBox cboCategoria=null!;
    int editId=-1;

    protected override void BuildToolbar(Panel p)
    {
        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 8, 0, 0),
            BackColor = Color.Transparent
        };
        txtSearch = MakeTextBox("🔍  Buscar producto...");
        txtSearch.Width = 260;
        txtSearch.TextChanged += (s, e) => LoadData();

        layout.Controls.AddRange([
            txtSearch,
            MakeButton("+ Nuevo", Accent, (s,e) => ShowForm(-1)),
            MakeButton("✏ Editar", Color.FromArgb(60,100,200), (s,e) => EditSelected()),
            MakeButton("✕ Desactivar", Color.FromArgb(180,50,50), (s,e) => DeleteSelected()),
            MakeButton("↻", Color.FromArgb(40,40,55), (s,e) => LoadData())
        ]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p)
    {
        p.Width = 310;
        var title = new Label
        {
            Text = "➕  Producto",
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = Accent,
            Dock = DockStyle.Top,
            Height = 36
        };
        txNombre = MakeTextBox("Nombre del producto");
        txDesc   = MakeTextBox("Descripcion");
        numPrecio = new NumericUpDown
        {
            Maximum = 9999999,
            DecimalPlaces = 2,
            BackColor = Color.FromArgb(32,32,46),
            ForeColor = TextLight
        };
        numCosto = new NumericUpDown
        {
            Maximum = 9999999,
            DecimalPlaces = 2,
            BackColor = Color.FromArgb(32,32,46),
            ForeColor = TextLight
        };
        cboCategoria = new ComboBox
        {
            BackColor = Color.FromArgb(32,32,46),
            ForeColor = TextLight,
            FlatStyle = FlatStyle.Flat,
            DisplayMember = "nombre",
            ValueMember = "id"
        };

        try
        {
            var dt = new System.Data.DataTable();
            using var conn = DBConnection.GetConnection();
            using var da = new SqlDataAdapter("SELECT id_categoria AS id, nombre FROM Categorias", conn);
            da.Fill(dt);
            cboCategoria.DataSource = dt;
        }
        catch { }

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.Transparent
        };

        void AddRow(string lbl, Control ctrl)
        {
            ctrl.Width = 270;
            flow.Controls.Add(MakeLabel(lbl));
            flow.Controls.Add(ctrl);
            flow.Controls.Add(new Panel { Height = 6, Width = 270, BackColor = Color.Transparent });
        }

        AddRow("Nombre *", txNombre);
        AddRow("Descripcion", txDesc);
        AddRow("Precio Venta *", numPrecio);
        AddRow("Costo", numCosto);
        AddRow("Categoria", cboCategoria);

        flow.Controls.Add(MakeButton("💾 Guardar", Accent, (s,e) => Guardar()));
        flow.Controls.Add(new Panel { Height = 8, Width = 270, BackColor = Color.Transparent });
        flow.Controls.Add(MakeButton("✕ Cancelar", Color.FromArgb(60,60,80), (s,e) => { panelForm.Visible=false; editId=-1; }));

        p.Controls.AddRange([flow, title]);
        flow.Dock = DockStyle.Fill;
    }

    protected override void LoadData()
    {
        var t = txtSearch?.Text.Trim() ?? "";
        var dt = FetchTable(@"
            SELECT p.id_producto AS ID, p.nombre AS Nombre, p.descripcion AS Descripcion,
                p.precio AS Precio, p.costo AS Costo, c.nombre AS Categoria,
                CASE p.activo WHEN 1 THEN '✓' ELSE '✗' END AS Activo
            FROM Productos p
            LEFT JOIN Categorias c ON c.id_categoria=p.id_categoria
            WHERE @t='' OR LOWER(p.nombre) LIKE '%'+@t+'%'
            ORDER BY p.nombre",
            cmd => cmd.Parameters.AddWithValue("@t", t.ToLower()));
        grid.DataSource = dt;
        if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;
    }

    void ShowForm(int id)
    {
        editId = id;
        if (id == -1)
        {
            txNombre.Text = txDesc.Text = "";
            numPrecio.Value = numCosto.Value = 0;
        }
        panelForm.Visible = true;
    }

    void EditSelected()
    {
        if (grid.CurrentRow == null) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        try
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand("SELECT * FROM Productos WHERE id_producto=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                txNombre.Text   = r["nombre"].ToString()!;
                txDesc.Text     = r["descripcion"].ToString()!;
                numPrecio.Value = (decimal)r["precio"];
                numCosto.Value  = (decimal)r["costo"];
                editId = id;
                panelForm.Visible = true;
            }
        }
        catch (Exception ex) { ShowMessage(ex.Message, true); }
    }

    void Guardar()
    {
        if (string.IsNullOrWhiteSpace(txNombre.Text) || numPrecio.Value <= 0)
        { ShowMessage("Nombre y precio son requeridos.", true); return; }

        var cat = cboCategoria.SelectedValue;

        if (editId == -1)
            RunQuery(@"INSERT INTO Productos(nombre,descripcion,precio,costo,id_categoria)
                       VALUES(@n,@d,@p,@c,@cat)",
                cmd => {
                    cmd.Parameters.AddWithValue("@n", txNombre.Text);
                    cmd.Parameters.AddWithValue("@d", txDesc.Text);
                    cmd.Parameters.AddWithValue("@p", numPrecio.Value);
                    cmd.Parameters.AddWithValue("@c", numCosto.Value);
                    cmd.Parameters.AddWithValue("@cat", cat ?? (object)DBNull.Value);
                }, "✅ Producto creado.");
        else
            RunQuery(@"UPDATE Productos SET nombre=@n,descripcion=@d,
                       precio=@p,costo=@c,id_categoria=@cat WHERE id_producto=@id",
                cmd => {
                    cmd.Parameters.AddWithValue("@n", txNombre.Text);
                    cmd.Parameters.AddWithValue("@d", txDesc.Text);
                    cmd.Parameters.AddWithValue("@p", numPrecio.Value);
                    cmd.Parameters.AddWithValue("@c", numCosto.Value);
                    cmd.Parameters.AddWithValue("@cat", cat ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", editId);
                }, "✅ Producto actualizado.");

        panelForm.Visible = false;
        editId = -1;
    }

    void DeleteSelected()
    {
        if (grid.CurrentRow == null) return;
        if (MessageBox.Show("¿Desactivar este producto?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        RunQuery("UPDATE Productos SET activo=0 WHERE id_producto=@id",
            cmd => cmd.Parameters.AddWithValue("@id", id), "Producto desactivado.");
    }
}
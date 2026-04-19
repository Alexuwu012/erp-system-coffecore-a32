using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class CategoriasControl : BaseGridControl
{
    protected override string ModuleTitle => "🏷️  Categorias de Productos";

    TextBox txNombre=null!, txDesc=null!;
    int editId=-1;

    protected override void BuildToolbar(Panel p)
    {
        var layout = new FlowLayoutPanel
        {
            Dock=DockStyle.Fill, FlowDirection=FlowDirection.LeftToRight,
            Padding=new Padding(0,8,0,0), BackColor=Color.Transparent
        };
        txtSearch = MakeTextBox("🔍  Buscar categoria...");
        txtSearch.Width = 240;
        txtSearch.TextChanged += (s,e) => LoadData();
        layout.Controls.AddRange([
            txtSearch,
            MakeButton("+ Nueva", Accent, (s,e) => ShowForm(-1)),
            MakeButton("✏ Editar", Color.FromArgb(60,100,200), (s,e) => EditSelected()),
            MakeButton("✕ Eliminar", Color.FromArgb(180,50,50), (s,e) => DeleteSelected()),
            MakeButton("↻", Color.FromArgb(40,40,55), (s,e) => LoadData())
        ]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p)
    {
        p.Width = 310;
        var title = new Label
        {
            Text = "🏷️  Categoria",
            Font = new Font("Segoe UI",11f,FontStyle.Bold),
            ForeColor = Accent, Dock = DockStyle.Top, Height = 36
        };
        txNombre = MakeTextBox("Nombre de la categoria");
        txDesc   = MakeTextBox("Descripcion");

        var flow = new FlowLayoutPanel
        {
            Dock=DockStyle.Fill, FlowDirection=FlowDirection.TopDown,
            WrapContents=false, AutoScroll=true, BackColor=Color.Transparent
        };
        void A(string l, Control c)
        {
            c.Width=270;
            flow.Controls.Add(MakeLabel(l));
            flow.Controls.Add(c);
            flow.Controls.Add(new Panel{Height=6,Width=270,BackColor=Color.Transparent});
        }
        A("Nombre *", txNombre);
        A("Descripcion", txDesc);

        var lblCount = new Label
        {
            ForeColor = TextMuted,
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = true,
            Text = ""
        };

        flow.Controls.Add(MakeButton("💾 Guardar", Accent, (s,e) => Guardar()));
        flow.Controls.Add(new Panel{Height=8,Width=270,BackColor=Color.Transparent});
        flow.Controls.Add(MakeButton("✕ Cancelar", Color.FromArgb(60,60,80), (s,e) => {
            panelForm.Visible=false; editId=-1;
        }));
        p.Controls.AddRange([flow, title]);
        flow.Dock = DockStyle.Fill;
    }

    protected override void LoadData()
    {
        var t = txtSearch?.Text.Trim() ?? "";
        var dt = FetchTable(@"
            SELECT c.id_categoria AS ID,
                c.nombre AS Nombre,
                c.descripcion AS Descripcion,
                COUNT(p.id_producto) AS Productos
            FROM Categorias c
            LEFT JOIN Productos p ON p.id_categoria=c.id_categoria AND p.activo=1
            WHERE @t='' OR LOWER(c.nombre) LIKE '%'+@t+'%'
            GROUP BY c.id_categoria, c.nombre, c.descripcion
            ORDER BY c.nombre",
            cmd => cmd.Parameters.AddWithValue("@t", t.ToLower()));
        grid.DataSource = dt;
        if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;
    }

    void ShowForm(int id)
    {
        editId = id;
        if (id == -1) txNombre.Text = txDesc.Text = "";
        panelForm.Visible = true;
    }

    void EditSelected()
    {
        if (grid.CurrentRow == null) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        try
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand("SELECT * FROM Categorias WHERE id_categoria=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                txNombre.Text = r["nombre"].ToString()!;
                txDesc.Text   = r["descripcion"].ToString()!;
                editId = id;
                panelForm.Visible = true;
            }
        }
        catch (Exception ex) { ShowMessage(ex.Message, true); }
    }

    void Guardar()
    {
        if (string.IsNullOrWhiteSpace(txNombre.Text))
        { ShowMessage("Nombre es obligatorio.", true); return; }

        if (editId == -1)
            RunQuery("INSERT INTO Categorias(nombre,descripcion)VALUES(@n,@d)",
                cmd => {
                    cmd.Parameters.AddWithValue("@n", txNombre.Text);
                    cmd.Parameters.AddWithValue("@d", txDesc.Text);
                }, "✅ Categoria creada.");
        else
            RunQuery("UPDATE Categorias SET nombre=@n,descripcion=@d WHERE id_categoria=@id",
                cmd => {
                    cmd.Parameters.AddWithValue("@n", txNombre.Text);
                    cmd.Parameters.AddWithValue("@d", txDesc.Text);
                    cmd.Parameters.AddWithValue("@id", editId);
                }, "✅ Categoria actualizada.");

        panelForm.Visible = false;
        editId = -1;
    }

    void DeleteSelected()
    {
        if (grid.CurrentRow == null) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        int prods = 0;
        try
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM Productos WHERE id_categoria=@id AND activo=1", conn);
            cmd.Parameters.AddWithValue("@id", id);
            prods = Convert.ToInt32(cmd.ExecuteScalar());
        }
        catch { }

        if (prods > 0)
        { ShowMessage($"No se puede eliminar. Tiene {prods} producto(s) activo(s).", true); return; }

        if (MessageBox.Show("¿Eliminar categoria?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

        RunQuery("DELETE FROM Categorias WHERE id_categoria=@id",
            cmd => cmd.Parameters.AddWithValue("@id", id), "Categoria eliminada.");
    }
}
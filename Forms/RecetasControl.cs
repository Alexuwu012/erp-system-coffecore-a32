using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class RecetasControl : BaseGridControl
{
    protected override string ModuleTitle => "📖  Recetas de Productos";

    ComboBox cboProducto=null!, cboInsumo=null!;
    NumericUpDown numCantidad=null!;
    DataGridView gridReceta=null!;
    System.Data.DataTable recetaTable=null!;
    Label lblProductoSel=null!;
    int idProductoSel=-1;

    protected override void BuildToolbar(Panel p)
    {
        var layout = new FlowLayoutPanel
        {
            Dock=DockStyle.Fill, FlowDirection=FlowDirection.LeftToRight,
            Padding=new Padding(0,8,0,0), BackColor=Color.Transparent
        };
        txtSearch = MakeTextBox("🔍  Buscar producto...");
        txtSearch.Width = 260;
        txtSearch.TextChanged += (s,e) => LoadData();
        layout.Controls.AddRange([
            txtSearch,
            MakeButton("📖 Ver Receta", Accent, (s,e) => VerReceta()),
            MakeButton("↻", Color.FromArgb(40,40,55), (s,e) => LoadData())
        ]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p)
    {
        p.Width = 360;
        var title = new Label
        {
            Text = "📖  Receta",
            Font = new Font("Segoe UI",11f,FontStyle.Bold),
            ForeColor = Accent, Dock = DockStyle.Top, Height = 36
        };

        lblProductoSel = new Label
        {
            Text = "Seleccione un producto",
            Font = new Font("Segoe UI",10f,FontStyle.Bold),
            ForeColor = TextLight,
            AutoSize = true,
            Padding = new Padding(0,0,0,8)
        };

        cboInsumo = new ComboBox
        {
            BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight,
            FlatStyle=FlatStyle.Flat, DisplayMember="nombre", ValueMember="id",
            Width=220
        };
        numCantidad = new NumericUpDown
        {
            Minimum=0, Maximum=9999, DecimalPlaces=3,
            BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, Width=80
        };

        LoadInsumos();

        recetaTable = new System.Data.DataTable();
        recetaTable.Columns.AddRange([
            new System.Data.DataColumn("id_receta"),
            new System.Data.DataColumn("Insumo"),
            new System.Data.DataColumn("Cantidad"),
            new System.Data.DataColumn("Unidad")
        ]);

        gridReceta = new DataGridView
        {
            Height=200, Width=330,
            BackgroundColor=Color.FromArgb(22,22,32),
            BorderStyle=BorderStyle.None,
            RowHeadersVisible=false,
            AllowUserToAddRows=false,
            ReadOnly=true,
            SelectionMode=DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,
            Font=new Font("Segoe UI",9f),
            DataSource=recetaTable
        };
        gridReceta.DefaultCellStyle.BackColor=Color.FromArgb(22,22,32);
        gridReceta.DefaultCellStyle.ForeColor=TextLight;
        gridReceta.ColumnHeadersDefaultCellStyle.BackColor=Color.FromArgb(30,30,44);
        gridReceta.ColumnHeadersDefaultCellStyle.ForeColor=Accent;
        gridReceta.EnableHeadersVisualStyles=false;

        var rowInsumo = new FlowLayoutPanel
        {
            FlowDirection=FlowDirection.LeftToRight,
            AutoSize=true, BackColor=Color.Transparent
        };
        rowInsumo.Controls.AddRange([
            cboInsumo, numCantidad,
            MakeButton("+", Color.FromArgb(60,120,60), (s,e) => AgregarInsumo())
        ]);

        var flow = new FlowLayoutPanel
        {
            Dock=DockStyle.Fill, FlowDirection=FlowDirection.TopDown,
            WrapContents=false, AutoScroll=true, BackColor=Color.Transparent,
            Padding=new Padding(0,4,0,0)
        };

        flow.Controls.Add(lblProductoSel);
        flow.Controls.Add(new Panel{Height=6,Width=330,BackColor=Color.Transparent});
        flow.Controls.Add(MakeLabel("Insumo / Cantidad"));
        flow.Controls.Add(rowInsumo);
        flow.Controls.Add(new Panel{Height=6,Width=330,BackColor=Color.Transparent});
        flow.Controls.Add(gridReceta);
        flow.Controls.Add(new Panel{Height=6,Width=330,BackColor=Color.Transparent});
        flow.Controls.Add(MakeButton("✕ Quitar Insumo", Color.FromArgb(120,60,60), (s,e) => QuitarInsumo()));
        flow.Controls.Add(new Panel{Height=8,Width=330,BackColor=Color.Transparent});
        flow.Controls.Add(MakeButton("✕ Cerrar", Color.FromArgb(60,60,80), (s,e) => panelForm.Visible=false));

        p.Controls.AddRange([flow, title]);
        flow.Dock = DockStyle.Fill;
    }

    void LoadInsumos()
    {
        try
        {
            var dt = new System.Data.DataTable();
            using var conn = DBConnection.GetConnection();
            using var da = new SqlDataAdapter(
                "SELECT id_insumo AS id, nombre+' ('+unidad+')' AS nombre FROM Insumos WHERE activo=1 ORDER BY nombre", conn);
            da.Fill(dt);
            cboInsumo.DataSource = dt;
        }
        catch { }
    }

    protected override void LoadData()
    {
        var t = txtSearch?.Text.Trim() ?? "";
        var dt = FetchTable(@"
            SELECT p.id_producto AS ID,
                p.nombre AS Producto,
                c.nombre AS Categoria,
                p.precio AS Precio,
                COUNT(r.id_receta) AS NumInsumos
            FROM Productos p
            LEFT JOIN Categorias c ON c.id_categoria=p.id_categoria
            LEFT JOIN RecetasProducto r ON r.id_producto=p.id_producto
            WHERE p.activo=1 AND (@t='' OR LOWER(p.nombre) LIKE '%'+@t+'%')
            GROUP BY p.id_producto, p.nombre, c.nombre, p.precio
            ORDER BY p.nombre",
            cmd => cmd.Parameters.AddWithValue("@t", t.ToLower()));
        grid.DataSource = dt;
        if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;
    }

    void VerReceta()
    {
        if (grid.CurrentRow == null) return;
        idProductoSel = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        string nombre = grid.CurrentRow.Cells[1].Value?.ToString() ?? "";
        lblProductoSel.Text = $"📖 {nombre}";
        CargarReceta();
        panelForm.Visible = true;
    }

    void CargarReceta()
    {
        recetaTable.Rows.Clear();
        try
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand(@"
                SELECT r.id_receta, i.nombre AS Insumo, r.cantidad, i.unidad
                FROM RecetasProducto r
                JOIN Insumos i ON i.id_insumo=r.id_insumo
                WHERE r.id_producto=@id ORDER BY i.nombre", conn);
            cmd.Parameters.AddWithValue("@id", idProductoSel);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var row = recetaTable.NewRow();
                row["id_receta"] = reader["id_receta"];
                row["Insumo"]    = reader["Insumo"];
                row["Cantidad"]  = reader["cantidad"];
                row["Unidad"]    = reader["unidad"];
                recetaTable.Rows.Add(row);
            }
        }
        catch { }
    }

    void AgregarInsumo()
    {
        if (idProductoSel == -1 || cboInsumo.SelectedValue == null) return;
        int idInsumo = Convert.ToInt32(cboInsumo.SelectedValue);
        decimal cant = numCantidad.Value;
        if (cant <= 0) { ShowMessage("La cantidad debe ser mayor a 0.", true); return; }

        RunQuery(@"INSERT INTO RecetasProducto(id_producto,id_insumo,cantidad)VALUES(@p,@i,@c)",
            cmd => {
                cmd.Parameters.AddWithValue("@p", idProductoSel);
                cmd.Parameters.AddWithValue("@i", idInsumo);
                cmd.Parameters.AddWithValue("@c", cant);
            }, "✅ Insumo agregado a la receta.");
        CargarReceta();
        LoadData();
    }

    void QuitarInsumo()
    {
        if (gridReceta.CurrentRow == null || gridReceta.CurrentRow.Index < 0) return;
        int idReceta = Convert.ToInt32(recetaTable.Rows[gridReceta.CurrentRow.Index]["id_receta"]);
        if (MessageBox.Show("¿Quitar este insumo de la receta?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        RunQuery("DELETE FROM RecetasProducto WHERE id_receta=@id",
            cmd => cmd.Parameters.AddWithValue("@id", idReceta), "✅ Insumo quitado.");
        CargarReceta();
        LoadData();
    }
}
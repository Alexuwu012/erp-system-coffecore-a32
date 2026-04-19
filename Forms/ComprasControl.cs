using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class ComprasControl : BaseGridControl
{
    protected override string ModuleTitle => "🛍  Compras a Proveedores";

    ComboBox cboProveedor=null!, cboSucursal=null!, cboInsumo=null!;
    NumericUpDown numCantidad=null!, numPrecio=null!;
    System.Data.DataTable detalleTable=null!;
    DataGridView gridDetalle=null!;
    Label lblTotal=null!;

    protected override void BuildToolbar(Panel p)
    {
        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 8, 0, 0),
            BackColor = Color.Transparent
        };
        txtSearch = MakeTextBox("🔍  Buscar...");
        txtSearch.Width = 200;
        txtSearch.TextChanged += (s, e) => LoadData();

        layout.Controls.AddRange([
            txtSearch,
            MakeButton("+ Nueva Compra", Accent, (s,e) => ShowForm()),
            MakeButton("👁 Ver Detalle", Color.FromArgb(60,100,200), (s,e) => VerDetalle()),
            MakeButton("↻", Color.FromArgb(40,40,55), (s,e) => LoadData())
        ]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p)
    {
        p.Width = 350;
        var title = new Label
        {
            Text = "🛍  Nueva Compra",
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = Accent,
            Dock = DockStyle.Top,
            Height = 36
        };

        cboProveedor = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat, DisplayMember="nombre", ValueMember="id", Width=310 };
        cboSucursal  = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat, DisplayMember="nombre", ValueMember="id", Width=310 };
        cboInsumo    = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat, DisplayMember="nombre", ValueMember="id", Width=200 };
        numCantidad  = new NumericUpDown { Minimum=1, Maximum=99999, DecimalPlaces=2, Value=1, BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, Width=80 };
        numPrecio    = new NumericUpDown { Maximum=9999999, DecimalPlaces=2, BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, Width=90 };
        lblTotal     = new Label { Text="Total: $0.00", Font=new Font("Segoe UI",12f,FontStyle.Bold), ForeColor=Accent, AutoSize=true };

        detalleTable = new System.Data.DataTable();
        detalleTable.Columns.Add(new System.Data.DataColumn("id_insumo"));
        detalleTable.Columns.Add(new System.Data.DataColumn("Insumo"));
        detalleTable.Columns.Add(new System.Data.DataColumn("Cantidad"));
        detalleTable.Columns.Add(new System.Data.DataColumn("Precio"));
        detalleTable.Columns.Add(new System.Data.DataColumn("Subtotal"));

        gridDetalle = new DataGridView
        {
            Height = 140, Width = 320,
            BackgroundColor = Color.FromArgb(22,22,32),
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Font = new Font("Segoe UI", 9f),
            DataSource = detalleTable
        };
        gridDetalle.DefaultCellStyle.BackColor = Color.FromArgb(22,22,32);
        gridDetalle.DefaultCellStyle.ForeColor = TextLight;
        gridDetalle.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30,30,44);
        gridDetalle.ColumnHeadersDefaultCellStyle.ForeColor = Accent;
        gridDetalle.EnableHeadersVisualStyles = false;

        LoadCombos();

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
            flow.Controls.Add(MakeLabel(lbl));
            flow.Controls.Add(ctrl);
            flow.Controls.Add(new Panel { Height=5, Width=320, BackColor=Color.Transparent });
        }

        AddRow("Proveedor *", cboProveedor);
        AddRow("Sucursal *",  cboSucursal);

        var rowInsumo = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            BackColor = Color.Transparent
        };
        rowInsumo.Controls.AddRange([
            cboInsumo, numCantidad, numPrecio,
            MakeButton("+", Color.FromArgb(60,120,60), (s,e) => AgregarItem())
        ]);

        flow.Controls.Add(MakeLabel("Insumo / Cantidad / Precio"));
        flow.Controls.Add(rowInsumo);
        flow.Controls.Add(new Panel { Height=5, Width=320, BackColor=Color.Transparent });
        flow.Controls.Add(gridDetalle);
        flow.Controls.Add(new Panel { Height=5, Width=320, BackColor=Color.Transparent });
        flow.Controls.Add(MakeButton("✕ Quitar", Color.FromArgb(120,60,60), (s,e) =>
        {
            if (gridDetalle.CurrentRow != null && gridDetalle.CurrentRow.Index >= 0)
                detalleTable.Rows.RemoveAt(gridDetalle.CurrentRow.Index);
            UpdateTotal();
        }));
        flow.Controls.Add(new Panel { Height=8, Width=320, BackColor=Color.Transparent });
        flow.Controls.Add(lblTotal);
        flow.Controls.Add(new Panel { Height=8, Width=320, BackColor=Color.Transparent });
        flow.Controls.Add(MakeButton("💾 Registrar", Accent, (s,e) => Registrar()));
        flow.Controls.Add(new Panel { Height=6, Width=320, BackColor=Color.Transparent });
        flow.Controls.Add(MakeButton("✕ Cancelar", Color.FromArgb(60,60,80), (s,e) => panelForm.Visible=false));

        p.Controls.AddRange([flow, title]);
        flow.Dock = DockStyle.Fill;
    }

    void LoadCombos()
    {
        try
        {
            using var conn = DBConnection.GetConnection();
            void Fill(ComboBox cb, string sql)
            {
                var dt = new System.Data.DataTable();
                using var da = new SqlDataAdapter(sql, conn);
                da.Fill(dt);
                cb.DataSource = dt;
            }
            Fill(cboProveedor, "SELECT id_proveedor AS id, nombre FROM Proveedores WHERE activo=1");
            Fill(cboSucursal,  "SELECT id_sucursal AS id, nombre FROM Sucursales WHERE activo=1");
            Fill(cboInsumo,    "SELECT id_insumo AS id, nombre+' ('+unidad+')' AS nombre FROM Insumos WHERE activo=1");
        }
        catch { }
    }

    void AgregarItem()
    {
        if (cboInsumo.SelectedValue == null) return;
        int     id   = Convert.ToInt32(cboInsumo.SelectedValue);
        string  nm   = cboInsumo.Text;
        decimal cant = (decimal)numCantidad.Value;
        decimal prec = (decimal)numPrecio.Value;
        var row = detalleTable.NewRow();
        row["id_insumo"] = id;
        row["Insumo"]    = nm;
        row["Cantidad"]  = cant;
        row["Precio"]    = prec;
        row["Subtotal"]  = cant * prec;
        detalleTable.Rows.Add(row);
        UpdateTotal();
    }

    void UpdateTotal()
    {
        decimal t = 0;
        foreach (System.Data.DataRow r in detalleTable.Rows)
            t += Convert.ToDecimal(r["Subtotal"]);
        lblTotal.Text = $"Total: ${t:N2}";
    }

    void ShowForm()
    {
        LoadCombos();
        detalleTable?.Rows.Clear();
        UpdateTotal();
        panelForm.Visible = true;
    }

    void Registrar()
    {
        if (cboProveedor.SelectedValue == null || cboSucursal.SelectedValue == null)
        { ShowMessage("Seleccione proveedor y sucursal.", true); return; }
        if (detalleTable.Rows.Count == 0)
        { ShowMessage("Agregue al menos un insumo.", true); return; }

        try
        {
            using var conn = DBConnection.GetConnection();
            using var tran = conn.BeginTransaction();

            decimal total = detalleTable.Rows
                .Cast<System.Data.DataRow>()
                .Sum(r => Convert.ToDecimal(r["Subtotal"]));

            var cmd = new SqlCommand(@"
                DECLARE @NuevaCompra TABLE (id_compra INT);
                INSERT INTO Compras(id_proveedor,id_sucursal,total)
                OUTPUT INSERTED.id_compra INTO @NuevaCompra
                VALUES(@p,@s,@t);
                SELECT id_compra FROM @NuevaCompra;", conn, tran);

            cmd.Parameters.AddWithValue("@p", cboProveedor.SelectedValue);
            cmd.Parameters.AddWithValue("@s", cboSucursal.SelectedValue);
            cmd.Parameters.AddWithValue("@t", total);
            int id = Convert.ToInt32(cmd.ExecuteScalar());

            foreach (System.Data.DataRow r in detalleTable.Rows)
            {
                var c = new SqlCommand(@"
                    INSERT INTO ComprasDetalle(id_compra,id_insumo,cantidad,precio)
                    VALUES(@c,@i,@ca,@p)", conn, tran);
                c.Parameters.AddWithValue("@c",  id);
                c.Parameters.AddWithValue("@i",  r["id_insumo"]);
                c.Parameters.AddWithValue("@ca", r["Cantidad"]);
                c.Parameters.AddWithValue("@p",  r["Precio"]);
                c.ExecuteNonQuery();
            }

            tran.Commit();
            DBConnection.RegistrarAuditoria("Compras", $"Registro compra #{id} por ${total:N2}");
            ShowMessage($"✅ Compra #{id} registrada correctamente.");
            detalleTable.Rows.Clear();
            UpdateTotal();
            panelForm.Visible = false;
            LoadData();
        }
        catch (Exception ex) { ShowMessage(ex.Message, true); }
    }

    void VerDetalle()
    {
        if (grid.CurrentRow == null) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        var dt = FetchTable(@"
            SELECT ins.nombre AS Insumo, cd.cantidad AS Cantidad,
                cd.precio AS Precio, (cd.cantidad*cd.precio) AS Subtotal
            FROM ComprasDetalle cd
            JOIN Insumos ins ON ins.id_insumo=cd.id_insumo
            WHERE cd.id_compra=@id",
            cmd => cmd.Parameters.AddWithValue("@id", id));

        var form = new Form
        {
            Text = $"Detalle Compra #{id}",
            Size = new Size(500, 350),
            BackColor = Color.FromArgb(22,22,32),
            StartPosition = FormStartPosition.CenterParent
        };
        var dg = new DataGridView
        {
            Dock = DockStyle.Fill, DataSource = dt,
            BackgroundColor = Color.FromArgb(22,22,32),
            BorderStyle = BorderStyle.None,
            ReadOnly = true, AllowUserToAddRows = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dg.DefaultCellStyle.BackColor = Color.FromArgb(22,22,32);
        dg.DefaultCellStyle.ForeColor = TextLight;
        dg.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30,30,44);
        dg.ColumnHeadersDefaultCellStyle.ForeColor = Accent;
        dg.EnableHeadersVisualStyles = false;
        form.Controls.Add(dg);
        form.ShowDialog();
    }

    protected override void LoadData()
    {
        var dt = FetchTable(@"
            SELECT c.id_compra AS ID, p.nombre AS Proveedor,
                s.nombre AS Sucursal, c.total AS Total,
                FORMAT(c.fecha,'dd/MM/yyyy HH:mm') AS Fecha
            FROM Compras c
            LEFT JOIN Proveedores p ON p.id_proveedor=c.id_proveedor
            LEFT JOIN Sucursales s ON s.id_sucursal=c.id_sucursal
            ORDER BY c.fecha DESC");
        grid.DataSource = dt;
        if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;
    }
}
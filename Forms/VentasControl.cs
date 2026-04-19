using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class VentasControl : BaseGridControl
{
    protected override string ModuleTitle => "🛒  Ventas";

    ComboBox cboCliente=null!, cboEmpleado=null!, cboSucursal=null!,
             cboMetodo=null!, cboProducto=null!;
    NumericUpDown numCantidad=null!;
    DataGridView gridDetalle=null!;
    Label lblTotal=null!;
    System.Data.DataTable detalleTable=null!;

    protected override void BuildToolbar(Panel p)
    {
        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 8, 0, 0),
            BackColor = Color.Transparent
        };
        txtSearch = MakeTextBox("🔍  Buscar venta...");
        txtSearch.Width = 220;
        txtSearch.TextChanged += (s, e) => LoadData();

        layout.Controls.AddRange([
            txtSearch,
            MakeButton("+ Nueva Venta", Accent, (s,e) => ShowNuevaVenta()),
            MakeButton("👁 Ver Detalle", Color.FromArgb(60,100,200), (s,e) => VerDetalle()),
            MakeButton("↻", Color.FromArgb(40,40,55), (s,e) => LoadData())
        ]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p)
    {
        p.Width = 380;
        var title = new Label
        {
            Text = "🛒  Nueva Venta",
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = Accent,
            Dock = DockStyle.Top,
            Height = 36
        };

        cboCliente  = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat, DisplayMember="nombre", ValueMember="id", Width=340 };
        cboEmpleado = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat, DisplayMember="nombre", ValueMember="id", Width=340 };
        cboSucursal = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat, DisplayMember="nombre", ValueMember="id", Width=340 };
        cboMetodo   = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat, DisplayMember="nombre", ValueMember="id", Width=340 };
        cboProducto = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat, DisplayMember="nombre", ValueMember="id", Width=220 };
        numCantidad = new NumericUpDown { Minimum=1, Maximum=9999, Value=1, BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, Width=80 };
        lblTotal    = new Label { Text="Total: $0.00", Font=new Font("Segoe UI",12f,FontStyle.Bold), ForeColor=Accent, AutoSize=true };

        detalleTable = new System.Data.DataTable();
        detalleTable.Columns.Add(new System.Data.DataColumn("id_producto"));
        detalleTable.Columns.Add(new System.Data.DataColumn("Producto"));
        detalleTable.Columns.Add(new System.Data.DataColumn("Cantidad"));
        detalleTable.Columns.Add(new System.Data.DataColumn("Precio"));
        detalleTable.Columns.Add(new System.Data.DataColumn("Subtotal"));

        gridDetalle = new DataGridView
        {
            Height = 150, Width = 350,
            BackgroundColor = Color.FromArgb(22,22,32),
            BorderStyle = BorderStyle.None,
            GridColor = Color.FromArgb(40,40,55),
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Font = new Font("Segoe UI", 9f),
            DataSource = detalleTable
        };
        gridDetalle.DefaultCellStyle.BackColor = Color.FromArgb(22,22,32);
        gridDetalle.DefaultCellStyle.ForeColor = TextLight;
        gridDetalle.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30,30,44);
        gridDetalle.ColumnHeadersDefaultCellStyle.ForeColor = Accent;
        gridDetalle.EnableHeadersVisualStyles = false;

        var btnAgregar  = MakeButton("+ Agregar", Color.FromArgb(60,120,60), (s,e) => AgregarItem());
        var btnQuitar   = MakeButton("✕ Quitar",  Color.FromArgb(120,60,60), (s,e) => QuitarItem());
        var btnGuardar  = MakeButton("💾 Registrar Venta", Accent, (s,e) => RegistrarVenta());
        var btnCancelar = MakeButton("✕ Cancelar", Color.FromArgb(60,60,80), (s,e) => panelForm.Visible=false);

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 4, 0, 0)
        };

        void AddRow(string lbl, Control ctrl)
        {
            flow.Controls.Add(MakeLabel(lbl));
            flow.Controls.Add(ctrl);
            flow.Controls.Add(new Panel { Height=5, Width=350, BackColor=Color.Transparent });
        }

        AddRow("Cliente", cboCliente);
        AddRow("Empleado *", cboEmpleado);
        AddRow("Sucursal *", cboSucursal);
        AddRow("Metodo de Pago *", cboMetodo);

        var rowProd = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            BackColor = Color.Transparent
        };
        rowProd.Controls.AddRange([cboProducto, numCantidad, btnAgregar]);

        flow.Controls.Add(MakeLabel("Producto / Cantidad"));
        flow.Controls.Add(rowProd);
        flow.Controls.Add(new Panel { Height=5, Width=350, BackColor=Color.Transparent });
        flow.Controls.Add(gridDetalle);
        flow.Controls.Add(new Panel { Height=5, Width=350, BackColor=Color.Transparent });
        flow.Controls.Add(btnQuitar);
        flow.Controls.Add(new Panel { Height=8, Width=350, BackColor=Color.Transparent });
        flow.Controls.Add(lblTotal);
        flow.Controls.Add(new Panel { Height=8, Width=350, BackColor=Color.Transparent });
        flow.Controls.Add(btnGuardar);
        flow.Controls.Add(new Panel { Height=6, Width=350, BackColor=Color.Transparent });
        flow.Controls.Add(btnCancelar);

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
            Fill(cboCliente,  "SELECT id_cliente AS id, nombre+' '+apellido AS nombre FROM Clientes WHERE activo=1");
            Fill(cboEmpleado, "SELECT id_empleado AS id, nombre+' '+apellido AS nombre FROM Empleados WHERE estado='Activo'");
            Fill(cboSucursal, "SELECT id_sucursal AS id, nombre FROM Sucursales WHERE activo=1");
            Fill(cboMetodo,   "SELECT id_metodo AS id, nombre FROM MetodosPago");
            Fill(cboProducto, "SELECT id_producto AS id, nombre FROM Productos WHERE activo=1");
        }
        catch { }
    }

    void AgregarItem()
    {
        if (cboProducto.SelectedValue == null) return;
        int idProd = Convert.ToInt32(cboProducto.SelectedValue);
        int cant   = (int)numCantidad.Value;

        try
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand(
                "SELECT nombre, precio FROM Productos WHERE id_producto=@id", conn);
            cmd.Parameters.AddWithValue("@id", idProd);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                string  nm     = r["nombre"].ToString()!;
                decimal precio = Convert.ToDecimal(r["precio"]);
                var row = detalleTable.NewRow();
                row["id_producto"] = idProd;
                row["Producto"]    = nm;
                row["Cantidad"]    = cant;
                row["Precio"]      = precio;
                row["Subtotal"]    = precio * cant;
                detalleTable.Rows.Add(row);
                UpdateTotal();
            }
        }
        catch (Exception ex) { ShowMessage(ex.Message, true); }
    }

    void QuitarItem()
    {
        if (gridDetalle.CurrentRow == null || gridDetalle.CurrentRow.Index < 0) return;
        detalleTable.Rows.RemoveAt(gridDetalle.CurrentRow.Index);
        UpdateTotal();
    }

    void UpdateTotal()
    {
        decimal total = 0;
        foreach (System.Data.DataRow r in detalleTable.Rows)
            total += Convert.ToDecimal(r["Subtotal"]);
        lblTotal.Text = $"Total: ${total:N2}";
    }

    void ShowNuevaVenta()
    {
        LoadCombos();
        detalleTable?.Rows.Clear();
        UpdateTotal();
        panelForm.Visible = true;
    }

    void RegistrarVenta()
    {
        if (cboEmpleado.SelectedValue == null || cboSucursal.SelectedValue == null || cboMetodo.SelectedValue == null)
        { ShowMessage("Complete los campos requeridos.", true); return; }
        if (detalleTable.Rows.Count == 0)
        { ShowMessage("Agregue al menos un producto.", true); return; }

        try
        {
            using var conn = DBConnection.GetConnection();
            using var tran = conn.BeginTransaction();
            try
            {
                decimal total = detalleTable.Rows
                    .Cast<System.Data.DataRow>()
                    .Sum(r => Convert.ToDecimal(r["Subtotal"]));

                var cmd = new SqlCommand(@"
                    DECLARE @NuevaVenta TABLE (id_venta INT);
                    INSERT INTO Ventas(id_cliente,id_empleado,id_sucursal,id_metodo,total)
                    OUTPUT INSERTED.id_venta INTO @NuevaVenta
                    VALUES(@c,@e,@s,@m,@t);
                    SELECT id_venta FROM @NuevaVenta;", conn, tran);

                cmd.Parameters.AddWithValue("@c", cboCliente.SelectedValue ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@e", cboEmpleado.SelectedValue);
                cmd.Parameters.AddWithValue("@s", cboSucursal.SelectedValue);
                cmd.Parameters.AddWithValue("@m", cboMetodo.SelectedValue);
                cmd.Parameters.AddWithValue("@t", total);
                int idVenta = Convert.ToInt32(cmd.ExecuteScalar());

                foreach (System.Data.DataRow r in detalleTable.Rows)
                {
                    var cmd2 = new SqlCommand(@"
                        INSERT INTO VentasDetalle(id_venta,id_producto,cantidad,precio)
                        VALUES(@v,@p,@c,@pr)", conn, tran);
                    cmd2.Parameters.AddWithValue("@v",  idVenta);
                    cmd2.Parameters.AddWithValue("@p",  Convert.ToInt32(r["id_producto"]));
                    cmd2.Parameters.AddWithValue("@c",  Convert.ToInt32(r["Cantidad"]));
                    cmd2.Parameters.AddWithValue("@pr", Convert.ToDecimal(r["Precio"]));
                    cmd2.ExecuteNonQuery();
                }

                tran.Commit();
                DBConnection.RegistrarAuditoria("Ventas", $"Venta #{idVenta} por ${total:N2}");
                detalleTable.Rows.Clear();
                UpdateTotal();
                ShowMessage($"✅ Venta #{idVenta} registrada exitosamente.");
                panelForm.Visible = false;

                // Recargar datos limpiamente
                var newDt = new System.Data.DataTable();
                using var conn2 = DBConnection.GetConnection();
                using var da = new SqlDataAdapter(@"
                    SELECT
                        v.id_venta AS ID,
                        ISNULL(c.nombre+' '+c.apellido,'Mostrador') AS Cliente,
                        ISNULL(e.nombre+' '+e.apellido,'—') AS Empleado,
                        ISNULL(s.nombre,'—') AS Sucursal,
                        ISNULL(m.nombre,'—') AS Metodo,
                        CAST(v.total AS DECIMAL(10,2)) AS Total,
                        FORMAT(v.fecha,'dd/MM/yyyy HH:mm') AS Fecha
                    FROM Ventas v
                    LEFT JOIN Clientes c ON c.id_cliente=v.id_cliente
                    LEFT JOIN Empleados e ON e.id_empleado=v.id_empleado
                    LEFT JOIN Sucursales s ON s.id_sucursal=v.id_sucursal
                    LEFT JOIN MetodosPago m ON m.id_metodo=v.id_metodo
                    ORDER BY v.fecha DESC", conn2);
                da.Fill(newDt);
                grid.DataSource = null;
                grid.DataSource = newDt;
                if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                ShowMessage($"Error al registrar venta: {ex.Message}", true);
            }
        }
        catch (Exception ex) { ShowMessage(ex.Message, true); }
    }

    void VerDetalle()
    {
        if (grid.CurrentRow == null) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        var dt = FetchTable(@"
            SELECT
                p.nombre AS Producto,
                vd.cantidad AS Cantidad,
                vd.precio AS Precio,
                (vd.cantidad*vd.precio) AS Subtotal
            FROM VentasDetalle vd
            JOIN Productos p ON p.id_producto=vd.id_producto
            WHERE vd.id_venta=@id",
            cmd => cmd.Parameters.AddWithValue("@id", id));

        var form = new Form
        {
            Text = $"Detalle Venta #{id}",
            Size = new Size(550, 400),
            BackColor = Color.FromArgb(22,22,32),
            StartPosition = FormStartPosition.CenterParent
        };
        var dg = new DataGridView
        {
            Dock = DockStyle.Fill,
            DataSource = dt,
            BackgroundColor = Color.FromArgb(22,22,32),
            BorderStyle = BorderStyle.None,
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeight = 36,
            RowTemplate = { Height = 34 },
            Font = new Font("Segoe UI", 9.5f)
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
        try
        {
            var newDt = new System.Data.DataTable();
            using var conn = DBConnection.GetConnection();
            using var da = new SqlDataAdapter(@"
                SELECT
                    v.id_venta AS ID,
                    ISNULL(c.nombre+' '+c.apellido,'Mostrador') AS Cliente,
                    ISNULL(e.nombre+' '+e.apellido,'—') AS Empleado,
                    ISNULL(s.nombre,'—') AS Sucursal,
                    ISNULL(m.nombre,'—') AS Metodo,
                    CAST(v.total AS DECIMAL(10,2)) AS Total,
                    FORMAT(v.fecha,'dd/MM/yyyy HH:mm') AS Fecha
                FROM Ventas v
                LEFT JOIN Clientes c ON c.id_cliente=v.id_cliente
                LEFT JOIN Empleados e ON e.id_empleado=v.id_empleado
                LEFT JOIN Sucursales s ON s.id_sucursal=v.id_sucursal
                LEFT JOIN MetodosPago m ON m.id_metodo=v.id_metodo
                ORDER BY v.fecha DESC", conn);
            da.Fill(newDt);

            // Limpiar y reasignar para evitar solapamientos
            grid.DataSource = null;
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.DataSource = newDt;
            if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;
        }
        catch (Exception ex) { ShowMessage(ex.Message, true); }
    }
}
using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class InventarioControl : BaseGridControl
{
    protected override string ModuleTitle => "📦  Inventario";

    ComboBox cboVista=null!;

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

        cboVista = new ComboBox
        {
            Width = 200,
            BackColor = Color.FromArgb(32,32,46),
            ForeColor = TextLight,
            FlatStyle = FlatStyle.Flat,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboVista.Items.AddRange(["Productos por Sucursal","Insumos por Sucursal"]);
        cboVista.SelectedIndex = 0;
        cboVista.SelectedIndexChanged += (s, e) => LoadData();

        layout.Controls.AddRange([
            cboVista,
            txtSearch,
            MakeButton("✏ Ajustar Stock", Accent, (s,e) => AjustarStock()),
            MakeButton("+ Agregar Registro", Color.FromArgb(60,120,60), (s,e) => AgregarRegistro()),
            MakeButton("↻ Refrescar", Color.FromArgb(40,40,55), (s,e) => LoadData())
        ]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p) { }

    protected override void LoadData()
    {
        var t = txtSearch?.Text.Trim().ToLower() ?? "";

        if (cboVista?.SelectedIndex == 1)
        {
            var dt = FetchTable(@"
                SELECT
                    ii.id_inventario AS ID,
                    i.nombre AS Insumo,
                    i.unidad AS Unidad,
                    s.nombre AS Sucursal,
                    ii.stock_actual AS Stock,
                    ii.stock_minimo AS Minimo,
                    CASE WHEN ii.stock_actual < ii.stock_minimo
                        THEN '⚠ Bajo'
                        ELSE '✓ OK'
                    END AS Estado
                FROM InventarioInsumos ii
                JOIN Insumos i ON i.id_insumo=ii.id_insumo
                JOIN Sucursales s ON s.id_sucursal=ii.id_sucursal
                WHERE @t='' OR LOWER(i.nombre) LIKE '%'+@t+'%'
                ORDER BY Estado DESC, i.nombre",
                cmd => cmd.Parameters.AddWithValue("@t", t));

            grid.DataSource = dt;
            if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;

            foreach (DataGridViewRow row in grid.Rows)
            {
                var estado = row.Cells["Estado"]?.Value?.ToString();
                row.DefaultCellStyle.ForeColor = estado != null && estado.Contains("Bajo")
                    ? Color.FromArgb(255, 160, 50)
                    : Color.FromArgb(80, 200, 120);
            }
        }
        else
        {
            var dt = FetchTable(@"
                SELECT
                    ip.id_inventario AS ID,
                    p.nombre AS Producto,
                    c.nombre AS Categoria,
                    s.nombre AS Sucursal,
                    ip.cantidad AS Cantidad,
                    CASE
                        WHEN ip.cantidad = 0 THEN '⚠ Agotado'
                        WHEN ip.cantidad < 5 THEN '⚠ Bajo'
                        ELSE '✓ OK'
                    END AS Estado
                FROM InventarioProductos ip
                JOIN Productos p ON p.id_producto=ip.id_producto
                LEFT JOIN Categorias c ON c.id_categoria=p.id_categoria
                JOIN Sucursales s ON s.id_sucursal=ip.id_sucursal
                WHERE @t='' OR LOWER(p.nombre) LIKE '%'+@t+'%'
                ORDER BY Estado DESC, p.nombre",
                cmd => cmd.Parameters.AddWithValue("@t", t));

            grid.DataSource = dt;
            if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;

            foreach (DataGridViewRow row in grid.Rows)
            {
                var estado = row.Cells["Estado"]?.Value?.ToString();
                row.DefaultCellStyle.ForeColor = estado switch
                {
                    var e when e != null && e.Contains("Agotado") => Color.FromArgb(220, 80, 80),
                    var e when e != null && e.Contains("Bajo")    => Color.FromArgb(255, 160, 50),
                    _                                              => Color.FromArgb(80, 200, 120)
                };
            }
        }
    }

    void AjustarStock()
    {
        if (grid.CurrentRow == null) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);

        // Obtener valor actual
        string nombreItem = grid.CurrentRow.Cells[1].Value?.ToString() ?? "";
        string stockActual = cboVista.SelectedIndex == 1
            ? grid.CurrentRow.Cells["Stock"]?.Value?.ToString() ?? "0"
            : grid.CurrentRow.Cells["Cantidad"]?.Value?.ToString() ?? "0";

        // Form de ajuste
        var form = new Form
        {
            Text = $"Ajustar Stock — {nombreItem}",
            Size = new Size(380, 280),
            BackColor = Color.FromArgb(22,22,32),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false
        };

        var lblActual = new Label
        {
            Text = $"Stock actual: {stockActual}",
            ForeColor = Color.FromArgb(194,139,74),
            Font = new Font("Segoe UI",11f,FontStyle.Bold),
            Location = new Point(20,20), AutoSize = true
        };

        var lblTipo = new Label { Text="Tipo de ajuste:", ForeColor=Color.FromArgb(140,140,155), Location=new Point(20,60), AutoSize=true };
        var cboTipo = new ComboBox
        {
            Location = new Point(20,80), Width=320,
            BackColor = Color.FromArgb(32,32,46),
            ForeColor = Color.FromArgb(230,230,235),
            FlatStyle = FlatStyle.Flat,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboTipo.Items.AddRange(["Establecer valor exacto","Sumar al stock actual","Restar al stock actual"]);
        cboTipo.SelectedIndex = 0;

        var lblCant = new Label { Text="Cantidad:", ForeColor=Color.FromArgb(140,140,155), Location=new Point(20,120), AutoSize=true };
        var numVal = new NumericUpDown
        {
            Location = new Point(20,140), Width=200,
            Maximum=999999, DecimalPlaces=2,
            BackColor = Color.FromArgb(32,32,46),
            ForeColor = Color.FromArgb(230,230,235)
        };

        var btnAplicar = new Button
        {
            Text = "✅ Aplicar Ajuste",
            Location = new Point(20,190), Width=160, Height=38,
            BackColor = Color.FromArgb(194,139,74),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI",10f,FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnAplicar.FlatAppearance.BorderSize = 0;

        var btnCancelar = new Button
        {
            Text = "Cancelar",
            Location = new Point(190,190), Width=100, Height=38,
            BackColor = Color.FromArgb(60,60,80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnCancelar.FlatAppearance.BorderSize = 0;
        btnCancelar.Click += (s,e) => form.Close();

        btnAplicar.Click += (s,e) =>
        {
            decimal val = numVal.Value;
            decimal actual = Convert.ToDecimal(stockActual);
            decimal nuevoStock = cboTipo.SelectedIndex switch
            {
                0 => val,
                1 => actual + val,
                2 => actual - val,
                _ => val
            };

            if (nuevoStock < 0)
            {
                MessageBox.Show("El stock no puede ser negativo.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using var conn = DBConnection.GetConnection();
                string sql = cboVista.SelectedIndex == 1
                    ? "UPDATE InventarioInsumos SET stock_actual=@v WHERE id_inventario=@id"
                    : "UPDATE InventarioProductos SET cantidad=@v WHERE id_inventario=@id";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@v",  nuevoStock);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();

                string tipoAjuste = cboTipo.SelectedItem?.ToString() ?? "";
                DBConnection.RegistrarAuditoria("Inventario",
                    $"Ajuste manual de stock en {nombreItem}: {tipoAjuste} {val} → nuevo stock: {nuevoStock}");

                MessageBox.Show($"✅ Stock actualizado a {nuevoStock}.", "Listo");
                form.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        form.Controls.AddRange([lblActual, lblTipo, cboTipo, lblCant, numVal, btnAplicar, btnCancelar]);
        form.ShowDialog();
    }

    void AgregarRegistro()
    {
        var form = new Form
        {
            Text = "Agregar Registro de Inventario",
            Size = new Size(400, 320),
            BackColor = Color.FromArgb(22,22,32),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false
        };

        bool esInsumo = cboVista.SelectedIndex == 1;

        var lblItem = new Label { Text=esInsumo?"Insumo:":"Producto:", ForeColor=Color.FromArgb(140,140,155), Location=new Point(20,20), AutoSize=true };
        var cboItem = new ComboBox
        {
            Location=new Point(20,40), Width=340,
            BackColor=Color.FromArgb(32,32,46), ForeColor=Color.FromArgb(230,230,235),
            FlatStyle=FlatStyle.Flat, DisplayMember="nombre", ValueMember="id"
        };

        var lblSuc = new Label { Text="Sucursal:", ForeColor=Color.FromArgb(140,140,155), Location=new Point(20,90), AutoSize=true };
        var cboSuc = new ComboBox
        {
            Location=new Point(20,110), Width=340,
            BackColor=Color.FromArgb(32,32,46), ForeColor=Color.FromArgb(230,230,235),
            FlatStyle=FlatStyle.Flat, DisplayMember="nombre", ValueMember="id"
        };

        var lblStock = new Label { Text=esInsumo?"Stock Actual:":"Cantidad:", ForeColor=Color.FromArgb(140,140,155), Location=new Point(20,160), AutoSize=true };
        var numStock = new NumericUpDown { Location=new Point(20,180), Width=150, Maximum=999999, DecimalPlaces=2, BackColor=Color.FromArgb(32,32,46), ForeColor=Color.FromArgb(230,230,235) };

        var lblMin = new Label { Text="Stock Mínimo:", ForeColor=Color.FromArgb(140,140,155), Location=new Point(190,160), AutoSize=true, Visible=esInsumo };
        var numMin = new NumericUpDown { Location=new Point(190,180), Width=150, Maximum=999999, DecimalPlaces=2, BackColor=Color.FromArgb(32,32,46), ForeColor=Color.FromArgb(230,230,235), Visible=esInsumo };

        try
        {
            using var conn = DBConnection.GetConnection();
            var dt1 = new System.Data.DataTable();
            var dt2 = new System.Data.DataTable();
            string sqlItem = esInsumo
                ? "SELECT id_insumo AS id, nombre FROM Insumos WHERE activo=1"
                : "SELECT id_producto AS id, nombre FROM Productos WHERE activo=1";
            using var da1 = new SqlDataAdapter(sqlItem, conn);
            using var da2 = new SqlDataAdapter("SELECT id_sucursal AS id, nombre FROM Sucursales WHERE activo=1", conn);
            da1.Fill(dt1); da2.Fill(dt2);
            cboItem.DataSource = dt1;
            cboSuc.DataSource  = dt2;
        }
        catch { }

        var btnGuardar = new Button
        {
            Text="💾 Guardar", Location=new Point(20,240), Width=160, Height=38,
            BackColor=Color.FromArgb(194,139,74), ForeColor=Color.White,
            FlatStyle=FlatStyle.Flat, Font=new Font("Segoe UI",10f,FontStyle.Bold), Cursor=Cursors.Hand
        };
        btnGuardar.FlatAppearance.BorderSize = 0;

        var btnCancelar = new Button
        {
            Text="Cancelar", Location=new Point(190,240), Width=100, Height=38,
            BackColor=Color.FromArgb(60,60,80), ForeColor=Color.White,
            FlatStyle=FlatStyle.Flat, Cursor=Cursors.Hand
        };
        btnCancelar.FlatAppearance.BorderSize = 0;
        btnCancelar.Click += (s,e) => form.Close();

        btnGuardar.Click += (s,e) =>
        {
            if (cboItem.SelectedValue == null || cboSuc.SelectedValue == null)
            { MessageBox.Show("Seleccione todos los campos.", "Error"); return; }

            try
            {
                using var conn = DBConnection.GetConnection();
                string sql = esInsumo
                    ? @"IF NOT EXISTS(SELECT 1 FROM InventarioInsumos WHERE id_insumo=@item AND id_sucursal=@suc)
                        INSERT INTO InventarioInsumos(id_insumo,id_sucursal,stock_actual,stock_minimo)
                        VALUES(@item,@suc,@stock,@min)
                        ELSE UPDATE InventarioInsumos SET stock_actual=@stock,stock_minimo=@min
                        WHERE id_insumo=@item AND id_sucursal=@suc"
                    : @"IF NOT EXISTS(SELECT 1 FROM InventarioProductos WHERE id_producto=@item AND id_sucursal=@suc)
                        INSERT INTO InventarioProductos(id_producto,id_sucursal,cantidad)
                        VALUES(@item,@suc,@stock)
                        ELSE UPDATE InventarioProductos SET cantidad=@stock
                        WHERE id_producto=@item AND id_sucursal=@suc";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@item",  cboItem.SelectedValue);
                cmd.Parameters.AddWithValue("@suc",   cboSuc.SelectedValue);
                cmd.Parameters.AddWithValue("@stock", numStock.Value);
                if (esInsumo) cmd.Parameters.AddWithValue("@min", numMin.Value);
                cmd.ExecuteNonQuery();

                DBConnection.RegistrarAuditoria("Inventario", $"Registro manual de inventario: {cboItem.Text}");
                MessageBox.Show("✅ Inventario actualizado.", "Listo");
                form.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };

        form.Controls.AddRange([lblItem, cboItem, lblSuc, cboSuc,
            lblStock, numStock, lblMin, numMin, btnGuardar, btnCancelar]);
        form.ShowDialog();
    }
}
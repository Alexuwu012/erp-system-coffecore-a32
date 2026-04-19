using Microsoft.Data.SqlClient;
using CoffeeERP.Database;
using System.Text;

namespace CoffeeERP.Forms;

public class ReportesControl : UserControl
{
    static readonly Color BgDark    = Color.FromArgb(12, 12, 18);
    static readonly Color BgCard    = Color.FromArgb(22, 22, 32);
    static readonly Color Accent    = Color.FromArgb(194, 139, 74);
    static readonly Color TextLight = Color.FromArgb(230, 230, 235);
    static readonly Color TextMuted = Color.FromArgb(120, 120, 140);

    DataGridView grid = null!;
    ComboBox cboReporte = null!;
    DateTimePicker dtDesde = null!, dtHasta = null!;
    Label lblInfo = null!;

    public ReportesControl()
    {
        BackColor = BgDark;
        Font = new Font("Segoe UI", 9.5f);
        BuildUI();
    }

    void BuildUI()
    {
        var header = new Panel { Dock=DockStyle.Top, Height=60, BackColor=BgCard };
        var lblTitle = new Label { Text="📋  Reportes", Font=new Font("Segoe UI",14f,FontStyle.Bold), ForeColor=Accent, AutoSize=true, Location=new Point(20,16) };
        header.Controls.Add(lblTitle);

        var toolbar = new Panel { Dock=DockStyle.Top, Height=56, BackColor=Color.FromArgb(16,16,26), Padding=new Padding(12,0,12,0) };
        var flow = new FlowLayoutPanel { Dock=DockStyle.Fill, FlowDirection=FlowDirection.LeftToRight, Padding=new Padding(0,10,0,0), BackColor=Color.Transparent };

        cboReporte = new ComboBox { Width=220, BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat, DropDownStyle=ComboBoxStyle.DropDownList };
        cboReporte.Items.AddRange(["Ventas por período","Top 10 productos","Clientes frecuentes","Stock crítico","Ingresos vs Gastos","Empleados activos"]);
        cboReporte.SelectedIndex = 0;

        dtDesde = new DateTimePicker { Format=DateTimePickerFormat.Short, Width=120, Value=DateTime.Today.AddMonths(-1) };
        dtHasta = new DateTimePicker { Format=DateTimePickerFormat.Short, Width=120, Value=DateTime.Today };

        var btnGenerar  = MakeBtn("▶ Generar", Accent, (s,e) => GenerarReporte());
        var btnExportar = MakeBtn("📥 Exportar CSV", Color.FromArgb(60,120,60), (s,e) => ExportarCSV());

        flow.Controls.AddRange([cboReporte, new Label{Text="  Desde:",ForeColor=TextMuted,AutoSize=true,Padding=new Padding(0,8,0,0)}, dtDesde, new Label{Text="  Hasta:",ForeColor=TextMuted,AutoSize=true,Padding=new Padding(0,8,0,0)}, dtHasta, btnGenerar, btnExportar]);
        toolbar.Controls.Add(flow);

        lblInfo = new Label { Dock=DockStyle.Bottom, Height=28, BackColor=Color.FromArgb(20,20,30), ForeColor=TextMuted, Font=new Font("Segoe UI",8.5f), TextAlign=ContentAlignment.MiddleLeft, Padding=new Padding(16,0,0,0) };

        grid = new DataGridView
        {
            Dock=DockStyle.Fill,
            BackgroundColor=Color.FromArgb(16,16,26),
            BorderStyle=BorderStyle.None,
            GridColor=Color.FromArgb(35,35,50),
            RowHeadersVisible=false,
            AllowUserToAddRows=false,
            ReadOnly=true,
            SelectionMode=DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeight=38,
            RowTemplate={Height=34},
            Font=new Font("Segoe UI",9.5f)
        };
        grid.DefaultCellStyle.BackColor=Color.FromArgb(16,16,26);
        grid.DefaultCellStyle.ForeColor=TextLight;
        grid.DefaultCellStyle.SelectionBackColor=Color.FromArgb(60,194,139,74);
        grid.ColumnHeadersDefaultCellStyle.BackColor=Color.FromArgb(24,24,38);
        grid.ColumnHeadersDefaultCellStyle.ForeColor=Accent;
        grid.ColumnHeadersDefaultCellStyle.Font=new Font("Segoe UI",9.5f,FontStyle.Bold);
        grid.EnableHeadersVisualStyles=false;
        grid.AlternatingRowsDefaultCellStyle.BackColor=Color.FromArgb(20,20,32);

        Controls.AddRange([grid, lblInfo, toolbar, header]);
        GenerarReporte();
    }

    Button MakeBtn(string text, Color bg, EventHandler onClick)
    {
        var btn = new Button { Text=text, BackColor=bg, ForeColor=Color.White, FlatStyle=FlatStyle.Flat, Font=new Font("Segoe UI",9f,FontStyle.Bold), Height=34, AutoSize=true, Padding=new Padding(10,0,10,0), Cursor=Cursors.Hand };
        btn.FlatAppearance.BorderSize=0;
        btn.Click+=onClick;
        return btn;
    }

    void GenerarReporte()
    {
        string sql = cboReporte.SelectedIndex switch
        {
            0 => $@"SELECT FORMAT(v.fecha,'dd/MM/yyyy') AS Fecha,
                    COUNT(*) AS NumVentas,
                    SUM(v.total) AS TotalVentas,
                    AVG(v.total) AS PromedioVenta
                    FROM Ventas v WHERE v.fecha BETWEEN '{dtDesde.Value:yyyy-MM-dd}' AND '{dtHasta.Value:yyyy-MM-dd} 23:59:59'
                    GROUP BY FORMAT(v.fecha,'dd/MM/yyyy') ORDER BY MIN(v.fecha) DESC",
            1 => @"SELECT TOP 10 p.nombre AS Producto,
                    SUM(vd.cantidad) AS UnidadesVendidas,
                    SUM(vd.cantidad*vd.precio) AS TotalGenerado
                    FROM VentasDetalle vd JOIN Productos p ON p.id_producto=vd.id_producto
                    GROUP BY p.nombre ORDER BY TotalGenerado DESC",
            2 => @"SELECT TOP 10 c.nombre+' '+c.apellido AS Cliente,
                    COUNT(*) AS NumCompras,
                    SUM(v.total) AS TotalGastado,
                    c.puntos_fidelidad AS Puntos
                    FROM Ventas v JOIN Clientes c ON c.id_cliente=v.id_cliente
                    GROUP BY c.nombre,c.apellido,c.puntos_fidelidad ORDER BY TotalGastado DESC",
            3 => @"SELECT i.nombre AS Insumo, i.unidad AS Unidad,
                    s.nombre AS Sucursal,
                    ii.stock_actual AS StockActual,
                    ii.stock_minimo AS StockMinimo,
                    (ii.stock_minimo-ii.stock_actual) AS Faltante
                    FROM InventarioInsumos ii
                    JOIN Insumos i ON i.id_insumo=ii.id_insumo
                    JOIN Sucursales s ON s.id_sucursal=ii.id_sucursal
                    WHERE ii.stock_actual < ii.stock_minimo ORDER BY Faltante DESC",
            4 => $@"SELECT tipo AS Tipo,
                    COUNT(*) AS Movimientos,
                    SUM(monto) AS Total,
                    FORMAT(MIN(fecha),'dd/MM/yyyy') AS Desde,
                    FORMAT(MAX(fecha),'dd/MM/yyyy') AS Hasta
                    FROM MovimientosFinancieros
                    WHERE fecha BETWEEN '{dtDesde.Value:yyyy-MM-dd}' AND '{dtHasta.Value:yyyy-MM-dd} 23:59:59'
                    GROUP BY tipo",
            5 => @"SELECT e.nombre+' '+e.apellido AS Empleado,
                    e.cargo AS Cargo, e.tipo_contrato AS Contrato,
                    e.salario AS Salario, s.nombre AS Sucursal,
                    e.fecha_contrato AS FechaIngreso
                    FROM Empleados e LEFT JOIN Sucursales s ON s.id_sucursal=e.id_sucursal
                    WHERE e.estado='Activo' ORDER BY e.nombre",
            _ => "SELECT 1"
        };

        try
        {
            var dt = new System.Data.DataTable();
            using var conn = DBConnection.GetConnection();
            using var da = new SqlDataAdapter(sql, conn);
            da.Fill(dt);
            grid.DataSource = dt;
            lblInfo.Text = $"  {dt.Rows.Count} registros encontrados  |  Reporte: {cboReporte.SelectedItem}  |  Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    void ExportarCSV()
    {
        if (grid.DataSource == null) { MessageBox.Show("Genera un reporte primero.", "Aviso"); return; }
        var sfd = new SaveFileDialog { Filter="CSV|*.csv", FileName=$"reporte_{DateTime.Now:yyyyMMdd_HHmm}.csv" };
        if (sfd.ShowDialog() != DialogResult.OK) return;
        try
        {
            var sb = new StringBuilder();
            var headers = grid.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText);
            sb.AppendLine(string.Join(",", headers));
            foreach (DataGridViewRow row in grid.Rows)
            {
                var cells = row.Cells.Cast<DataGridViewCell>().Select(c => $"\"{c.Value}\"");
                sb.AppendLine(string.Join(",", cells));
            }
            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show($"✅ Exportado exitosamente:\n{sfd.FileName}", "Listo");
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
    }
}
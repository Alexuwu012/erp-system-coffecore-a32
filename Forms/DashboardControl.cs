using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class DashboardControl : UserControl
{
    static readonly Color BgDark    = Color.FromArgb(12, 12, 18);
    static readonly Color BgCard    = Color.FromArgb(22, 22, 32);
    static readonly Color BgCard2   = Color.FromArgb(28, 28, 42);
    static readonly Color Accent    = Color.FromArgb(194, 139, 74);
    static readonly Color TextLight = Color.FromArgb(230, 230, 235);
    static readonly Color TextMuted = Color.FromArgb(120, 120, 140);

    System.Windows.Forms.Timer refreshTimer = null!;

    // Referencias a los controles que se actualizan
    Label[] kpiValues = null!;
    DataGridView gridVentas = null!;
    DataGridView gridStock  = null!;

    public DashboardControl()
    {
        BackColor = BgDark;
        Font = new Font("Segoe UI", 9.5f);
        Dock = DockStyle.Fill;
        BuildUI();
        StartAutoRefresh();
    }

    void StartAutoRefresh()
    {
        refreshTimer = new System.Windows.Forms.Timer { Interval = 15000 };
        refreshTimer.Tick += (s, e) => RefreshData();
        refreshTimer.Start();
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        refreshTimer?.Stop();
        refreshTimer?.Dispose();
        base.OnHandleDestroyed(e);
    }

    readonly (string Label, string Query, Color Color)[] kpis =
    [
        ("Ventas Hoy",
            "SELECT ISNULL(SUM(total),0) FROM Ventas WHERE CAST(fecha AS DATE)=CAST(GETDATE() AS DATE)",
            Color.FromArgb(80,200,120)),
        ("Ventas del Mes",
            "SELECT ISNULL(SUM(total),0) FROM Ventas WHERE MONTH(fecha)=MONTH(GETDATE()) AND YEAR(fecha)=YEAR(GETDATE())",
            Color.FromArgb(194,139,74)),
        ("Transacciones Hoy",
            "SELECT COUNT(*) FROM Ventas WHERE CAST(fecha AS DATE)=CAST(GETDATE() AS DATE)",
            Color.FromArgb(100,180,255)),
        ("Clientes Activos",
            "SELECT COUNT(*) FROM Clientes WHERE activo=1",
            Color.FromArgb(140,120,255)),
        ("Productos Activos",
            "SELECT COUNT(*) FROM Productos WHERE activo=1",
            Color.FromArgb(200,100,255)),
        ("Stock Critico",
            "SELECT COUNT(*) FROM InventarioInsumos WHERE stock_actual < stock_minimo",
            Color.FromArgb(255,160,50)),
        ("Empleados Activos",
            "SELECT COUNT(*) FROM Empleados WHERE estado='Activo'",
            Color.FromArgb(80,200,200)),
        ("Gastos del Mes",
            "SELECT ISNULL(SUM(monto),0) FROM MovimientosFinancieros WHERE tipo='GASTO' AND MONTH(fecha)=MONTH(GETDATE()) AND YEAR(fecha)=YEAR(GETDATE())",
            Color.FromArgb(220,80,80)),
    ];

    void BuildUI()
    {
        Controls.Clear();

        // ── Header ──────────────────────────────────────────────
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 68,
            BackColor = BgCard
        };
        var hdrLine = new Panel { Dock=DockStyle.Bottom, Height=1, BackColor=Color.FromArgb(40,40,60) };
        var lblTitle = new Label
        {
            Text = "Dashboard",
            Font = new Font("Segoe UI", 17f, FontStyle.Bold),
            ForeColor = Accent,
            Location = new Point(24, 10),
            Size = new Size(300, 32)
        };
        var lblSub = new Label
        {
            Text = $"Bienvenido {LoginForm.CurrentUser}  —  {DateTime.Now:dddd, dd MMMM yyyy}",
            Font = new Font("Segoe UI", 9f),
            ForeColor = TextMuted,
            Location = new Point(26, 42),
            Size = new Size(500, 18)
        };
        var btnRef = new Button
        {
            Text = "Actualizar",
            BackColor = Color.FromArgb(40,194,139,74),
            ForeColor = Accent,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Size = new Size(110, 32),
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.Right | AnchorStyles.Top
        };
        btnRef.FlatAppearance.BorderColor = Accent;
        btnRef.FlatAppearance.BorderSize = 1;
        btnRef.Click += (s, e) => RefreshData();
        header.Controls.AddRange([hdrLine, lblTitle, lblSub, btnRef]);
        header.Resize += (s, e) => btnRef.Location = new Point(header.Width - 130, 18);

        // ── Layout principal ────────────────────────────────────
        var main = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            BackColor = BgDark,
            Padding = new Padding(20, 16, 20, 16)
        };
        main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 240f));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        // ── KPI Grid ────────────────────────────────────────────
        var kpiTable = new TableLayoutPanel
        {
            ColumnCount = 4,
            RowCount = 2,
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 0, 0, 12)
        };
        for (int i = 0; i < 4; i++)
            kpiTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        kpiTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
        kpiTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

        kpiValues = new Label[kpis.Length];

        for (int i = 0; i < kpis.Length; i++)
        {
            var (label, query, color) = kpis[i];
            string value = GetKpiValue(query);

            var card = new Panel
            {
                BackColor = BgCard,
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
            var topBar = new Panel { Dock=DockStyle.Top, Height=3, BackColor=color };
            var lblVal = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 19f, FontStyle.Bold),
                ForeColor = TextLight,
                Location = new Point(16, 16),
                Size = new Size(220, 36)
            };
            var lblLine = new Panel
            {
                Location = new Point(16, 56),
                Size = new Size(32, 2),
                BackColor = color
            };
            var lblLbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9f),
                ForeColor = TextMuted,
                Location = new Point(16, 62),
                Size = new Size(200, 18)
            };

            kpiValues[i] = lblVal;
            card.Controls.AddRange([topBar, lblVal, lblLine, lblLbl]);
            kpiTable.Controls.Add(card, i % 4, i / 4);
        }

        // ── Tablas ──────────────────────────────────────────────
        var tablasPanel = new TableLayoutPanel
        {
            ColumnCount = 2,
            RowCount = 1,
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            Padding = new Padding(0)
        };
        tablasPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62f));
        tablasPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38f));
        tablasPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        // Panel ventas
        var pVentas = new Panel { BackColor=BgCard, Dock=DockStyle.Fill, Margin=new Padding(0,0,8,0) };
        var hdrVentas = new Panel { Dock=DockStyle.Top, Height=42, BackColor=BgCard2 };
        var lblVentas = new Label
        {
            Text = "Ultimas Ventas",
            Font = new Font("Segoe UI",11f,FontStyle.Bold),
            ForeColor = Accent,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16,0,0,0)
        };
        hdrVentas.Controls.Add(lblVentas);
        gridVentas = BuildGrid(Accent);
        pVentas.Controls.AddRange([gridVentas, hdrVentas]);

        // Panel stock
        var pStock = new Panel { BackColor=BgCard, Dock=DockStyle.Fill, Margin=new Padding(8,0,0,0) };
        var hdrStock = new Panel { Dock=DockStyle.Top, Height=42, BackColor=BgCard2 };
        var lblStock = new Label
        {
            Text = "Stock Critico",
            Font = new Font("Segoe UI",11f,FontStyle.Bold),
            ForeColor = Color.FromArgb(255,160,50),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16,0,0,0)
        };
        hdrStock.Controls.Add(lblStock);
        gridStock = BuildGrid(Color.FromArgb(255,160,50));
        gridStock.DefaultCellStyle.ForeColor = Color.FromArgb(255,160,50);
        pStock.Controls.AddRange([gridStock, hdrStock]);

        tablasPanel.Controls.Add(pVentas, 0, 0);
        tablasPanel.Controls.Add(pStock,  1, 0);

        main.Controls.Add(kpiTable,    0, 0);
        main.Controls.Add(tablasPanel, 0, 1);

        Controls.Add(main);
        Controls.Add(header);

        // Cargar datos iniciales
        RefreshData();
    }

    string GetKpiValue(string query)
    {
        try
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand(query, conn);
            var res = cmd.ExecuteScalar();
            return res is decimal d ? $"${d:N0}" : res?.ToString() ?? "0";
        }
        catch { return "—"; }
    }

    void RefreshData()
    {
        try
        {
            // Actualizar KPIs sin reconstruir la UI
            for (int i = 0; i < kpis.Length; i++)
            {
                var val = GetKpiValue(kpis[i].Query);
                if (kpiValues[i].InvokeRequired)
                    kpiValues[i].Invoke(() => kpiValues[i].Text = val);
                else
                    kpiValues[i].Text = val;
            }

            // Actualizar grid ventas
            var dtV = new System.Data.DataTable();
            using (var conn = DBConnection.GetConnection())
            using (var da = new SqlDataAdapter(@"
                SELECT TOP 15
                    ISNULL(c.nombre+' '+c.apellido,'Mostrador') AS Cliente,
                    e.nombre+' '+e.apellido AS Empleado,
                    s.nombre AS Sucursal,
                    m.nombre AS Metodo,
                    '$'+FORMAT(v.total,'N0') AS Total,
                    FORMAT(v.fecha,'dd/MM/yy HH:mm') AS Fecha
                FROM Ventas v
                LEFT JOIN Clientes c ON c.id_cliente=v.id_cliente
                LEFT JOIN Empleados e ON e.id_empleado=v.id_empleado
                LEFT JOIN Sucursales s ON s.id_sucursal=v.id_sucursal
                LEFT JOIN MetodosPago m ON m.id_metodo=v.id_metodo
                ORDER BY v.fecha DESC", conn))
                da.Fill(dtV);

            // Actualizar grid stock
            var dtS = new System.Data.DataTable();
            using (var conn2 = DBConnection.GetConnection())
            using (var da2 = new SqlDataAdapter(@"
                SELECT
                    i.nombre AS Insumo,
                    s.nombre AS Sucursal,
                    ii.stock_actual AS Actual,
                    ii.stock_minimo AS Minimo
                FROM InventarioInsumos ii
                JOIN Insumos i ON i.id_insumo=ii.id_insumo
                JOIN Sucursales s ON s.id_sucursal=ii.id_sucursal
                WHERE ii.stock_actual < ii.stock_minimo
                ORDER BY (ii.stock_minimo-ii.stock_actual) DESC", conn2))
                da2.Fill(dtS);

            if (gridVentas.InvokeRequired)
            {
                gridVentas.Invoke(() => gridVentas.DataSource = dtV);
                gridStock.Invoke(() => gridStock.DataSource = dtS);
            }
            else
            {
                gridVentas.DataSource = dtV;
                gridStock.DataSource  = dtS;
            }
        }
        catch { }
    }

    DataGridView BuildGrid(Color accentColor)
    {
        var g = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = BgCard,
            BorderStyle = BorderStyle.None,
            GridColor = Color.FromArgb(35,35,52),
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeight = 36,
            RowTemplate = { Height = 32 },
            Font = new Font("Segoe UI", 9f),
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        };
        g.DefaultCellStyle.BackColor = BgCard;
        g.DefaultCellStyle.ForeColor = TextLight;
        g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(50,194,139,74);
        g.DefaultCellStyle.SelectionForeColor = Color.White;
        g.DefaultCellStyle.Padding = new Padding(6,0,6,0);
        g.AlternatingRowsDefaultCellStyle.BackColor = BgCard2;
        g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24,24,38);
        g.ColumnHeadersDefaultCellStyle.ForeColor = accentColor;
        g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI",9f,FontStyle.Bold);
        g.ColumnHeadersDefaultCellStyle.Padding = new Padding(6,0,6,0);
        g.EnableHeadersVisualStyles = false;
        return g;
    }
}
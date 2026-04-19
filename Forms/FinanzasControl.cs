using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class FinanzasControl : BaseGridControl
{
    protected override string ModuleTitle => "💰  Finanzas & Caja";

    ComboBox cboTipo=null!, cboPeriodo=null!;

    protected override void BuildToolbar(Panel p)
    {
        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 8, 0, 0),
            BackColor = Color.Transparent
        };

        cboTipo = new ComboBox
        {
            Width = 130,
            BackColor = Color.FromArgb(32,32,46),
            ForeColor = TextLight,
            FlatStyle = FlatStyle.Flat,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboTipo.Items.AddRange(["Todos","INGRESO","GASTO"]);
        cboTipo.SelectedIndex = 0;
        cboTipo.SelectedIndexChanged += (s, e) => LoadData();

        cboPeriodo = new ComboBox
        {
            Width = 130,
            BackColor = Color.FromArgb(32,32,46),
            ForeColor = TextLight,
            FlatStyle = FlatStyle.Flat,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboPeriodo.Items.AddRange(["Hoy","Esta semana","Este mes","Todo"]);
        cboPeriodo.SelectedIndex = 2;
        cboPeriodo.SelectedIndexChanged += (s, e) => LoadData();

        layout.Controls.AddRange([
            MakeLabel("  Tipo: "),
            cboTipo,
            MakeLabel("  Periodo: "),
            cboPeriodo,
            MakeButton("📋 Ver Caja", Color.FromArgb(60,100,200), (s,e) => VerCaja()),
            MakeButton("↻", Color.FromArgb(40,40,55), (s,e) => LoadData())
        ]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p) { }

    protected override void LoadData()
    {
        string tipo    = cboTipo?.SelectedItem?.ToString() ?? "Todos";
        string periodo = cboPeriodo?.SelectedItem?.ToString() ?? "Este mes";

        string dateFilter = periodo switch
        {
            "Hoy"         => "CAST(fecha AS DATE)=CAST(GETDATE() AS DATE)",
            "Esta semana" => "fecha>=DATEADD(DAY,-7,GETDATE())",
            "Este mes"    => "MONTH(fecha)=MONTH(GETDATE()) AND YEAR(fecha)=YEAR(GETDATE())",
            _             => "1=1"
        };

        string tipoFilter = tipo == "Todos" ? "1=1" : $"tipo='{tipo}'";

        var dt = FetchTable($@"
            SELECT
                m.tipo AS Tipo,
                m.descripcion AS Descripcion,
                m.monto AS Monto,
                s.nombre AS Sucursal,
                FORMAT(m.fecha,'dd/MM/yyyy HH:mm') AS Fecha
            FROM MovimientosFinancieros m
            LEFT JOIN Sucursales s ON s.id_sucursal=m.id_sucursal
            WHERE {dateFilter} AND {tipoFilter}
            ORDER BY m.fecha DESC");

        grid.DataSource = dt;

        // Colorear por tipo
        foreach (DataGridViewRow row in grid.Rows)
        {
            var t = row.Cells[0].Value?.ToString();
            row.DefaultCellStyle.ForeColor = t == "INGRESO"
                ? Color.FromArgb(80, 200, 120)
                : Color.FromArgb(220, 80, 80);
        }
    }

    void VerCaja()
    {
        var dt = FetchTable(@"
            SELECT
                c.id_caja AS ID,
                s.nombre AS Sucursal,
                c.fecha AS Fecha,
                c.apertura AS Apertura,
                c.cierre AS Cierre,
                (c.cierre - c.apertura) AS Diferencia
            FROM Caja c
            LEFT JOIN Sucursales s ON s.id_sucursal=c.id_sucursal
            ORDER BY c.fecha DESC");

        var form = new Form
        {
            Text = "Caja Diaria",
            Size = new Size(650, 420),
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
        dg.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24,24,34);

        form.Controls.Add(dg);
        form.ShowDialog();
    }
}
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class AuditoriaControl : BaseGridControl
{
    protected override string ModuleTitle => "📜  Auditoria del Sistema";

    ComboBox cboModulo=null!, cboPeriodo=null!;

    protected override void BuildToolbar(Panel p)
    {
        var layout = new FlowLayoutPanel
        {
            Dock=DockStyle.Fill, FlowDirection=FlowDirection.LeftToRight,
            Padding=new Padding(0,8,0,0), BackColor=Color.Transparent
        };
        txtSearch = MakeTextBox("🔍  Buscar usuario...");
        txtSearch.Width = 200;
        txtSearch.TextChanged += (s,e) => LoadData();

        cboModulo = new ComboBox
        {
            Width=140, BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight,
            FlatStyle=FlatStyle.Flat, DropDownStyle=ComboBoxStyle.DropDownList
        };
        cboModulo.Items.AddRange(["Todos","Ventas","Compras","Clientes",
            "Empleados","Productos","Inventario","Finanzas"]);
        cboModulo.SelectedIndex=0;
        cboModulo.SelectedIndexChanged += (s,e) => LoadData();

        cboPeriodo = new ComboBox
        {
            Width=130, BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight,
            FlatStyle=FlatStyle.Flat, DropDownStyle=ComboBoxStyle.DropDownList
        };
        cboPeriodo.Items.AddRange(["Hoy","Esta semana","Este mes","Todo"]);
        cboPeriodo.SelectedIndex=0;
        cboPeriodo.SelectedIndexChanged += (s,e) => LoadData();

        layout.Controls.AddRange([
            txtSearch,
            MakeLabel("  Modulo: "), cboModulo,
            MakeLabel("  Periodo: "), cboPeriodo,
            MakeButton("🗑 Limpiar", Color.FromArgb(120,60,60), (s,e) => LimpiarAuditoria()),
            MakeButton("↻", Color.FromArgb(40,40,55), (s,e) => LoadData())
        ]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p) { }

    protected override void LoadData()
    {
        var t       = txtSearch?.Text.Trim().ToLower() ?? "";
        var modulo  = cboModulo?.SelectedItem?.ToString() ?? "Todos";
        var periodo = cboPeriodo?.SelectedItem?.ToString() ?? "Hoy";

        string dateFilter = periodo switch
        {
            "Hoy"         => "CAST(fecha AS DATE)=CAST(GETDATE() AS DATE)",
            "Esta semana" => "fecha>=DATEADD(DAY,-7,GETDATE())",
            "Este mes"    => "MONTH(fecha)=MONTH(GETDATE()) AND YEAR(fecha)=YEAR(GETDATE())",
            _             => "1=1"
        };
        string moduloFilter = modulo == "Todos" ? "1=1" : $"modulo='{modulo}'";

        var dt = FetchTable($@"
            SELECT
                usuario AS Usuario,
                rol AS Rol,
                modulo AS Modulo,
                accion AS Accion,
                FORMAT(fecha,'dd/MM/yyyy HH:mm:ss') AS Fecha
            FROM Auditoria
            WHERE {dateFilter} AND {moduloFilter}
            AND (@t='' OR LOWER(usuario) LIKE '%'+@t+'%')
            ORDER BY fecha DESC",
            cmd => cmd.Parameters.AddWithValue("@t", t));

        grid.DataSource = dt;

        // Colorear por modulo
        foreach (DataGridViewRow row in grid.Rows)
        {
            var mod = row.Cells[2].Value?.ToString();
            row.DefaultCellStyle.ForeColor = mod switch
            {
                "Ventas"    => Color.FromArgb(80,200,120),
                "Compras"   => Color.FromArgb(255,160,50),
                "Empleados" => Color.FromArgb(100,160,255),
                "Clientes"  => Color.FromArgb(200,120,255),
                _           => TextLight
            };
        }
    }

    void LimpiarAuditoria()
    {
        if (MessageBox.Show(
            "¿Eliminar registros de auditoria anteriores a 30 dias?",
            "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        RunQuery("DELETE FROM Auditoria WHERE fecha < DATEADD(DAY,-30,GETDATE())",
            cmd => { }, "✅ Auditoria limpiada.");
    }
}
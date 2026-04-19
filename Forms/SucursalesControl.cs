using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class SucursalesControl : BaseGridControl
{
    protected override string ModuleTitle => "🏢  Sucursales";

    TextBox txNombre=null!, txDir=null!, txCiudad=null!,
            txTel=null!, txEmail=null!, txGerente=null!;
    DateTimePicker dtApertura=null!;
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
        layout.Controls.AddRange([
            MakeButton("+ Nueva", Accent, (s,e) => ShowForm(-1)),
            MakeButton("✏ Editar", Color.FromArgb(60,100,200), (s,e) => EditSelected()),
            MakeButton("↻ Refrescar", Color.FromArgb(40,40,55), (s,e) => LoadData())
        ]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p)
    {
        p.Width = 310;
        var title = new Label
        {
            Text = "🏢  Sucursal",
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = Accent,
            Dock = DockStyle.Top,
            Height = 36
        };
        txNombre  = MakeTextBox("Nombre");
        txDir     = MakeTextBox("Direccion");
        txCiudad  = MakeTextBox("Ciudad");
        txTel     = MakeTextBox("Telefono");
        txEmail   = MakeTextBox("Email");
        txGerente = MakeTextBox("Gerente");
        dtApertura = new DateTimePicker { Format = DateTimePickerFormat.Short };

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
            flow.Controls.Add(new Panel { Height = 5, Width = 270, BackColor = Color.Transparent });
        }

        AddRow("Nombre *", txNombre);
        AddRow("Direccion", txDir);
        AddRow("Ciudad", txCiudad);
        AddRow("Telefono", txTel);
        AddRow("Email", txEmail);
        AddRow("Gerente", txGerente);
        AddRow("Fecha Apertura", dtApertura);

        flow.Controls.Add(MakeButton("💾 Guardar", Accent, (s,e) => Guardar()));
        flow.Controls.Add(new Panel { Height = 8, Width = 270, BackColor = Color.Transparent });
        flow.Controls.Add(MakeButton("✕ Cancelar", Color.FromArgb(60,60,80), (s,e) => { panelForm.Visible=false; editId=-1; }));

        p.Controls.AddRange([flow, title]);
        flow.Dock = DockStyle.Fill;
    }

    protected override void LoadData()
    {
        var dt = FetchTable(@"
            SELECT id_sucursal AS ID, nombre AS Nombre, ciudad AS Ciudad,
                telefono AS Telefono, gerente AS Gerente,
                CASE activo WHEN 1 THEN '✓ Activa' ELSE '✗ Inactiva' END AS Estado
            FROM Sucursales ORDER BY nombre");
        grid.DataSource = dt;
        if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;
    }

    void ShowForm(int id)
    {
        editId = id;
        if (id == -1)
            txNombre.Text = txDir.Text = txCiudad.Text = txTel.Text = txEmail.Text = txGerente.Text = "";
        panelForm.Visible = true;
    }

    void EditSelected()
    {
        if (grid.CurrentRow == null) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        try
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand("SELECT * FROM Sucursales WHERE id_sucursal=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                txNombre.Text  = r["nombre"].ToString()!;
                txDir.Text     = r["direccion"].ToString()!;
                txCiudad.Text  = r["ciudad"].ToString()!;
                txTel.Text     = r["telefono"].ToString()!;
                txEmail.Text   = r["email"].ToString()!;
                txGerente.Text = r["gerente"].ToString()!;
                if (r["fecha_apertura"] != DBNull.Value)
                    dtApertura.Value = (DateTime)r["fecha_apertura"];
                editId = id;
                panelForm.Visible = true;
            }
        }
        catch (Exception ex) { ShowMessage(ex.Message, true); }
    }

    void Guardar()
    {
        if (string.IsNullOrWhiteSpace(txNombre.Text))
        { ShowMessage("Nombre requerido.", true); return; }

        if (editId == -1)
            RunQuery(@"INSERT INTO Sucursales(nombre,direccion,ciudad,telefono,email,gerente,fecha_apertura)
                       VALUES(@n,@d,@c,@t,@e,@g,@f)",
                cmd => {
                    cmd.Parameters.AddWithValue("@n", txNombre.Text);
                    cmd.Parameters.AddWithValue("@d", txDir.Text);
                    cmd.Parameters.AddWithValue("@c", txCiudad.Text);
                    cmd.Parameters.AddWithValue("@t", txTel.Text);
                    cmd.Parameters.AddWithValue("@e", txEmail.Text);
                    cmd.Parameters.AddWithValue("@g", txGerente.Text);
                    cmd.Parameters.AddWithValue("@f", dtApertura.Value.Date);
                }, "✅ Sucursal creada.");
        else
            RunQuery(@"UPDATE Sucursales SET nombre=@n,direccion=@d,ciudad=@c,
                       telefono=@t,email=@e,gerente=@g,fecha_apertura=@f
                       WHERE id_sucursal=@id",
                cmd => {
                    cmd.Parameters.AddWithValue("@n", txNombre.Text);
                    cmd.Parameters.AddWithValue("@d", txDir.Text);
                    cmd.Parameters.AddWithValue("@c", txCiudad.Text);
                    cmd.Parameters.AddWithValue("@t", txTel.Text);
                    cmd.Parameters.AddWithValue("@e", txEmail.Text);
                    cmd.Parameters.AddWithValue("@g", txGerente.Text);
                    cmd.Parameters.AddWithValue("@f", dtApertura.Value.Date);
                    cmd.Parameters.AddWithValue("@id", editId);
                }, "✅ Sucursal actualizada.");

        panelForm.Visible = false;
        editId = -1;
    }
}
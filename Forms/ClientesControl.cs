using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class ClientesControl : BaseGridControl
{
    protected override string ModuleTitle => "👥  Clientes";

    TextBox txNombre=null!, txApellido=null!, txTel=null!, txEmail=null!,
            txDir=null!, txCiudad=null!;
    DateTimePicker dtNac=null!;
    ComboBox cboGenero=null!;
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
        txtSearch = MakeTextBox("🔍  Buscar por nombre o email...");
        txtSearch.Width = 280;
        txtSearch.TextChanged += (s, e) => FilterGrid();

        layout.Controls.AddRange([
            txtSearch,
            MakeButton("+ Nuevo Cliente", Accent, (s,e) => ShowForm(-1)),
            MakeButton("✏ Editar", Color.FromArgb(60,100,200), (s,e) => EditSelected()),
            MakeButton("✕ Desactivar", Color.FromArgb(180,50,50), (s,e) => DeleteSelected()),
            MakeButton("↻ Refrescar", Color.FromArgb(40,40,55), (s,e) => LoadData())
        ]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p)
    {
        p.Width = 320;
        var title = new Label
        {
            Text = "➕  Cliente",
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = Accent,
            Dock = DockStyle.Top,
            Height = 36
        };
        txNombre   = MakeTextBox("Nombre");
        txApellido = MakeTextBox("Apellido");
        txTel      = MakeTextBox("Telefono");
        txEmail    = MakeTextBox("Email");
        txDir      = MakeTextBox("Direccion");
        txCiudad   = MakeTextBox("Ciudad");
        dtNac = new DateTimePicker { Format = DateTimePickerFormat.Short };
        cboGenero = new ComboBox
        {
            BackColor = Color.FromArgb(32,32,46),
            ForeColor = TextLight,
            FlatStyle = FlatStyle.Flat
        };
        cboGenero.Items.AddRange(["Masculino","Femenino","Otro","Prefiero no decir"]);

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
            flow.Controls.Add(new Panel { Height = 6, Width = 270, BackColor = Color.Transparent });
        }

        AddRow("Nombre *", txNombre);
        AddRow("Apellido *", txApellido);
        AddRow("Telefono", txTel);
        AddRow("Email", txEmail);
        AddRow("Direccion", txDir);
        AddRow("Ciudad", txCiudad);
        AddRow("Fecha Nacimiento", dtNac);
        AddRow("Genero", cboGenero);

        flow.Controls.Add(MakeButton("💾 Guardar", Accent, (s,e) => GuardarCliente()));
        flow.Controls.Add(new Panel { Height = 8, Width = 270, BackColor = Color.Transparent });
        flow.Controls.Add(MakeButton("✕ Cancelar", Color.FromArgb(60,60,80), (s,e) => { panelForm.Visible=false; editId=-1; }));

        p.Controls.AddRange([flow, title]);
        flow.Dock = DockStyle.Fill;
    }

    protected override void LoadData()
    {
        var dt = FetchTable(@"
            SELECT id_cliente AS ID, nombre AS Nombre, apellido AS Apellido,
                telefono AS Telefono, email AS Email, ciudad AS Ciudad,
                puntos_fidelidad AS Puntos,
                CASE activo WHEN 1 THEN '✓ Activo' ELSE '✗ Inactivo' END AS Estado
            FROM Clientes ORDER BY nombre");
        grid.DataSource = dt;
        if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;
    }

    void FilterGrid()
    {
        var term = txtSearch.Text.Trim().ToLower();
        if (string.IsNullOrEmpty(term)) { LoadData(); return; }
        var dt = FetchTable(@"
            SELECT id_cliente AS ID, nombre AS Nombre, apellido AS Apellido,
                telefono AS Telefono, email AS Email, ciudad AS Ciudad,
                puntos_fidelidad AS Puntos,
                CASE activo WHEN 1 THEN '✓ Activo' ELSE '✗ Inactivo' END AS Estado
            FROM Clientes
            WHERE LOWER(nombre+' '+apellido) LIKE @t OR LOWER(email) LIKE @t
            ORDER BY nombre",
            cmd => cmd.Parameters.AddWithValue("@t", $"%{term}%"));
        grid.DataSource = dt;
        if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;
    }

    void ShowForm(int id)
    {
        editId = id;
        if (id == -1)
            txNombre.Text = txApellido.Text = txTel.Text = txEmail.Text = txDir.Text = txCiudad.Text = "";
        panelForm.Visible = true;
    }

    void EditSelected()
    {
        if (grid.CurrentRow == null) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        try
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand("SELECT * FROM Clientes WHERE id_cliente=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                txNombre.Text   = r["nombre"].ToString()!;
                txApellido.Text = r["apellido"].ToString()!;
                txTel.Text      = r["telefono"].ToString()!;
                txEmail.Text    = r["email"].ToString()!;
                txDir.Text      = r["direccion"].ToString()!;
                txCiudad.Text   = r["ciudad"].ToString()!;
                if (r["fecha_nacimiento"] != DBNull.Value)
                    dtNac.Value = (DateTime)r["fecha_nacimiento"];
                cboGenero.Text = r["genero"].ToString()!;
                editId = id;
                panelForm.Visible = true;
            }
        }
        catch (Exception ex) { ShowMessage(ex.Message, true); }
    }

    void GuardarCliente()
    {
        if (string.IsNullOrWhiteSpace(txNombre.Text) || string.IsNullOrWhiteSpace(txApellido.Text))
        { ShowMessage("Nombre y apellido son obligatorios.", true); return; }

        if (editId == -1)
            RunQuery(@"INSERT INTO Clientes(nombre,apellido,telefono,email,direccion,ciudad,fecha_nacimiento,genero)
                       VALUES(@n,@a,@t,@e,@d,@c,@f,@g)",
                cmd => {
                    cmd.Parameters.AddWithValue("@n", txNombre.Text);
                    cmd.Parameters.AddWithValue("@a", txApellido.Text);
                    cmd.Parameters.AddWithValue("@t", txTel.Text);
                    cmd.Parameters.AddWithValue("@e", txEmail.Text);
                    cmd.Parameters.AddWithValue("@d", txDir.Text);
                    cmd.Parameters.AddWithValue("@c", txCiudad.Text);
                    cmd.Parameters.AddWithValue("@f", dtNac.Value.Date);
                    cmd.Parameters.AddWithValue("@g", cboGenero.Text);
                }, "✅ Cliente creado.");
        else
            RunQuery(@"UPDATE Clientes SET nombre=@n,apellido=@a,telefono=@t,email=@e,
                       direccion=@d,ciudad=@c,fecha_nacimiento=@f,genero=@g WHERE id_cliente=@id",
                cmd => {
                    cmd.Parameters.AddWithValue("@n", txNombre.Text);
                    cmd.Parameters.AddWithValue("@a", txApellido.Text);
                    cmd.Parameters.AddWithValue("@t", txTel.Text);
                    cmd.Parameters.AddWithValue("@e", txEmail.Text);
                    cmd.Parameters.AddWithValue("@d", txDir.Text);
                    cmd.Parameters.AddWithValue("@c", txCiudad.Text);
                    cmd.Parameters.AddWithValue("@f", dtNac.Value.Date);
                    cmd.Parameters.AddWithValue("@g", cboGenero.Text);
                    cmd.Parameters.AddWithValue("@id", editId);
                }, "✅ Cliente actualizado.");

        panelForm.Visible = false;
        editId = -1;
    }

    void DeleteSelected()
    {
        if (grid.CurrentRow == null) return;
        if (MessageBox.Show("¿Desactivar este cliente?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        RunQuery("UPDATE Clientes SET activo=0 WHERE id_cliente=@id",
            cmd => cmd.Parameters.AddWithValue("@id", id), "Cliente desactivado.");
    }
}
using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class EmpleadosControl : BaseGridControl
{
    protected override string ModuleTitle => "🧑  Empleados — Recursos Humanos";

    // Info personal
    TextBox txNombre=null!, txApellido=null!, txDoc=null!, txTel=null!,
            txEmail=null!, txDir=null!, txNacionalidad=null!;
    DateTimePicker dtNacimiento=null!;
    ComboBox cboGenero=null!, cboEstadoCivil=null!;
    NumericUpDown numHijos=null!;

    // Info laboral
    TextBox txCargo=null!, txEps=null!, txPension=null!;
    NumericUpDown numSalario=null!;
    DateTimePicker dtContrato=null!;
    ComboBox cboTipoContrato=null!, cboEstado=null!, cboSucursal=null!, cboNivelEdu=null!;

    // Info bancaria
    TextBox txCuentaBancaria=null!, txBanco=null!;
    ComboBox cboTipoCuenta=null!;

    // Contacto emergencia
    TextBox txContactoEmergencia=null!, txTelEmergencia=null!;

    TabControl tabForm=null!;
    int editId=-1;

    protected override void BuildToolbar(Panel p)
    {
        var layout = new FlowLayoutPanel
        {
            Dock=DockStyle.Fill,
            FlowDirection=FlowDirection.LeftToRight,
            Padding=new Padding(0,8,0,0),
            BackColor=Color.Transparent
        };
        txtSearch = MakeTextBox("🔍  Buscar por nombre, cargo o documento...");
        txtSearch.Width = 300;
        txtSearch.TextChanged += (s,e) => LoadData();

        layout.Controls.AddRange([
            txtSearch,
            MakeButton("+ Nuevo", Accent, (s,e) => ShowForm(-1)),
            MakeButton("✏ Editar", Color.FromArgb(60,100,200), (s,e) => EditSelected()),
            MakeButton("✕ Desactivar", Color.FromArgb(180,50,50), (s,e) => DeleteSelected()),
            MakeButton("↻ Refrescar", Color.FromArgb(40,40,55), (s,e) => LoadData())
        ]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p)
    {
        p.Width = 400;
        var title = new Label
        {
            Text = "➕  Empleado",
            Font = new Font("Segoe UI",11f,FontStyle.Bold),
            ForeColor = Accent,
            Dock = DockStyle.Top,
            Height = 36
        };

        // Inicializar controles
        txNombre              = MakeTextBox("Nombre");
        txApellido            = MakeTextBox("Apellido");
        txDoc                 = MakeTextBox("Numero de documento");
        txTel                 = MakeTextBox("Telefono");
        txEmail               = MakeTextBox("Email");
        txDir                 = MakeTextBox("Direccion");
        txNacionalidad        = MakeTextBox("Nacionalidad");
        txCargo               = MakeTextBox("Cargo");
        txEps                 = MakeTextBox("EPS");
        txPension             = MakeTextBox("Fondo de pension");
        txCuentaBancaria      = MakeTextBox("Numero de cuenta");
        txBanco               = MakeTextBox("Nombre del banco");
        txContactoEmergencia  = MakeTextBox("Nombre contacto");
        txTelEmergencia       = MakeTextBox("Telefono emergencia");

        dtNacimiento = new DateTimePicker { Format=DateTimePickerFormat.Short };
        dtContrato   = new DateTimePicker { Format=DateTimePickerFormat.Short };
        numSalario   = new NumericUpDown  { Maximum=99999999, DecimalPlaces=2, BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight };
        numHijos     = new NumericUpDown  { Maximum=20, BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight };

        cboGenero = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat };
        cboGenero.Items.AddRange(["Masculino","Femenino","No binario","Prefiero no decir"]);

        cboEstadoCivil = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat };
        cboEstadoCivil.Items.AddRange(["Soltero/a","Casado/a","Union libre","Divorciado/a","Viudo/a"]);

        cboNivelEdu = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat };
        cboNivelEdu.Items.AddRange(["Primaria","Bachillerato","Tecnico","Tecnologo","Universitario","Posgrado"]);

        cboTipoContrato = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat };
        cboTipoContrato.Items.AddRange(["Indefinido","Fijo","Prestacion de servicios","Aprendizaje","Temporal"]);

        cboEstado = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat };
        cboEstado.Items.AddRange(["Activo","Inactivo","Vacaciones","Incapacidad","Licencia"]);

        cboTipoCuenta = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat };
        cboTipoCuenta.Items.AddRange(["Ahorros","Corriente","Nequi","Daviplata"]);

        cboSucursal = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat, DisplayMember="nombre", ValueMember="id" };
        try {
            var dt=new System.Data.DataTable();
            using var conn=DBConnection.GetConnection();
            using var da=new SqlDataAdapter("SELECT id_sucursal AS id,nombre FROM Sucursales WHERE activo=1",conn);
            da.Fill(dt); cboSucursal.DataSource=dt;
        } catch {}

        // TabControl con pestañas
        tabForm = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI",9f),
        };
        tabForm.DrawMode = TabDrawMode.OwnerDrawFixed;
        tabForm.DrawItem += (s,e) =>
        {
            var g = e.Graphics;
            var tab = tabForm.TabPages[e.Index];
            var rect = tabForm.GetTabRect(e.Index);
            bool sel = e.Index == tabForm.SelectedIndex;
            g.FillRectangle(new SolidBrush(sel ? Color.FromArgb(40,194,139,74) : Color.FromArgb(26,26,36)), rect);
            g.DrawString(tab.Text, new Font("Segoe UI",8.5f,sel?FontStyle.Bold:FontStyle.Regular),
                new SolidBrush(sel ? Color.FromArgb(194,139,74) : Color.FromArgb(140,140,155)),
                rect, new StringFormat{Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Center});
        };

        // TAB 1: Personal
        var tabPersonal = new TabPage("👤 Personal") { BackColor=BgCard, Padding=new Padding(8) };
        var flowP = MakeFlow();
        void AP(string l, Control c) { c.Width=340; flowP.Controls.Add(MakeLabel(l)); flowP.Controls.Add(c); flowP.Controls.Add(Spacer()); }
        AP("Nombre *",txNombre); AP("Apellido *",txApellido);
        AP("Documento *",txDoc); AP("Fecha Nacimiento",dtNacimiento);
        AP("Genero",cboGenero); AP("Estado Civil",cboEstadoCivil);
        AP("Num. Hijos",numHijos); AP("Nacionalidad",txNacionalidad);
        AP("Telefono",txTel); AP("Email",txEmail); AP("Direccion",txDir);
        tabPersonal.Controls.Add(flowP);

        // TAB 2: Laboral
        var tabLaboral = new TabPage("💼 Laboral") { BackColor=BgCard, Padding=new Padding(8) };
        var flowL = MakeFlow();
        void AL(string l, Control c) { c.Width=340; flowL.Controls.Add(MakeLabel(l)); flowL.Controls.Add(c); flowL.Controls.Add(Spacer()); }
        AL("Cargo *",txCargo); AL("Sucursal",cboSucursal);
        AL("Salario",numSalario); AL("Fecha Contrato",dtContrato);
        AL("Tipo Contrato",cboTipoContrato); AL("Estado",cboEstado);
        AL("Nivel Educativo",cboNivelEdu);
        tabLaboral.Controls.Add(flowL);

        // TAB 3: Seguridad Social
        var tabSocial = new TabPage("🏥 Seguridad") { BackColor=BgCard, Padding=new Padding(8) };
        var flowS = MakeFlow();
        void AS(string l, Control c) { c.Width=340; flowS.Controls.Add(MakeLabel(l)); flowS.Controls.Add(c); flowS.Controls.Add(Spacer()); }
        AS("EPS",txEps); AS("Fondo de Pension",txPension);
        tabSocial.Controls.Add(flowS);

        // TAB 4: Bancario
        var tabBanco = new TabPage("🏦 Bancario") { BackColor=BgCard, Padding=new Padding(8) };
        var flowB = MakeFlow();
        void AB(string l, Control c) { c.Width=340; flowB.Controls.Add(MakeLabel(l)); flowB.Controls.Add(c); flowB.Controls.Add(Spacer()); }
        AB("Banco",txBanco); AB("Tipo Cuenta",cboTipoCuenta);
        AB("Numero Cuenta",txCuentaBancaria);
        tabBanco.Controls.Add(flowB);

        // TAB 5: Emergencia
        var tabEmergencia = new TabPage("🚨 Emergencia") { BackColor=BgCard, Padding=new Padding(8) };
        var flowE = MakeFlow();
        void AE(string l, Control c) { c.Width=340; flowE.Controls.Add(MakeLabel(l)); flowE.Controls.Add(c); flowE.Controls.Add(Spacer()); }
        AE("Nombre Contacto",txContactoEmergencia);
        AE("Telefono",txTelEmergencia);
        tabEmergencia.Controls.Add(flowE);

        tabForm.TabPages.AddRange([tabPersonal,tabLaboral,tabSocial,tabBanco,tabEmergencia]);

        var btnRow = new FlowLayoutPanel { Dock=DockStyle.Bottom, Height=46, FlowDirection=FlowDirection.LeftToRight, Padding=new Padding(0,6,0,0), BackColor=BgCard };
        btnRow.Controls.Add(MakeButton("💾 Guardar", Accent, (s,e) => Guardar()));
        btnRow.Controls.Add(new Panel{Width=8,Height=34,BackColor=Color.Transparent});
        btnRow.Controls.Add(MakeButton("✕ Cancelar", Color.FromArgb(60,60,80), (s,e) => { panelForm.Visible=false; editId=-1; }));

        p.Controls.AddRange([btnRow, tabForm, title]);
        tabForm.Dock = DockStyle.Fill;
    }

    FlowLayoutPanel MakeFlow() => new()
    {
        Dock=DockStyle.Fill,
        FlowDirection=FlowDirection.TopDown,
        WrapContents=false,
        AutoScroll=true,
        BackColor=Color.Transparent,
        Padding=new Padding(4)
    };

    Panel Spacer() => new() { Height=5, Width=340, BackColor=Color.Transparent };

    protected override void LoadData()
    {
        var t = txtSearch?.Text.Trim() ?? "";
        var dt = FetchTable(@"
            SELECT e.id_empleado AS ID,
                e.nombre AS Nombre,
                e.apellido AS Apellido,
                e.documento AS Documento,
                e.cargo AS Cargo,
                e.salario AS Salario,
                e.tipo_contrato AS Contrato,
                e.estado AS Estado,
                e.eps AS EPS,
                e.num_hijos AS Hijos,
                s.nombre AS Sucursal
            FROM Empleados e
            LEFT JOIN Sucursales s ON s.id_sucursal=e.id_sucursal
            WHERE @t='' OR
                LOWER(e.nombre+' '+e.apellido) LIKE '%'+@t+'%' OR
                LOWER(e.documento) LIKE '%'+@t+'%' OR
                LOWER(e.cargo) LIKE '%'+@t+'%'
            ORDER BY e.nombre",
            cmd => cmd.Parameters.AddWithValue("@t", t.ToLower()));
        grid.DataSource = dt;
        if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;

        // Colorear por estado
        foreach (DataGridViewRow row in grid.Rows)
        {
            var estado = row.Cells["Estado"]?.Value?.ToString();
            row.DefaultCellStyle.ForeColor = estado switch
            {
                "Activo"     => Color.FromArgb(80,200,120),
                "Vacaciones" => Color.FromArgb(100,160,255),
                "Incapacidad"=> Color.FromArgb(255,160,50),
                "Inactivo"   => Color.FromArgb(180,80,80),
                _            => TextLight
            };
        }
    }

    void ShowForm(int id)
    {
        editId = id;
        if (id == -1)
        {
            txNombre.Text=txApellido.Text=txDoc.Text=txTel.Text=txEmail.Text="";
            txDir.Text=txCargo.Text=txEps.Text=txPension.Text="";
            txCuentaBancaria.Text=txBanco.Text=txNacionalidad.Text="";
            txContactoEmergencia.Text=txTelEmergencia.Text="";
            numSalario.Value=numHijos.Value=0;
            cboEstado.SelectedIndex=-1;
            cboTipoContrato.SelectedIndex=-1;
        }
        panelForm.Visible = true;
        tabForm.SelectedIndex = 0;
    }

    void EditSelected()
    {
        if (grid.CurrentRow == null) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        try
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand("SELECT * FROM Empleados WHERE id_empleado=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                txNombre.Text   = r["nombre"].ToString()!;
                txApellido.Text = r["apellido"].ToString()!;
                txDoc.Text      = r["documento"].ToString()!;
                txTel.Text      = r["telefono"].ToString()!;
                txEmail.Text    = r["email"].ToString()!;
                txDir.Text      = r["direccion"].ToString()!;
                txCargo.Text    = r["cargo"].ToString()!;
                txEps.Text      = r["eps"]?.ToString()!;
                txPension.Text  = r["pension"]?.ToString()!;
                txBanco.Text    = r["banco"]?.ToString()!;
                txCuentaBancaria.Text   = r["cuenta_bancaria"]?.ToString()!;
                txContactoEmergencia.Text = r["contacto_emergencia"]?.ToString()!;
                txTelEmergencia.Text    = r["telefono_emergencia"]?.ToString()!;
                txNacionalidad.Text     = r["nacionalidad"]?.ToString()!;
                numSalario.Value = r["salario"] != DBNull.Value ? (decimal)r["salario"] : 0;
                numHijos.Value   = r["num_hijos"] != DBNull.Value ? Convert.ToDecimal(r["num_hijos"]) : 0;
                if (r["fecha_contrato"] != DBNull.Value) dtContrato.Value = (DateTime)r["fecha_contrato"];
                if (r["fecha_nacimiento"] != DBNull.Value) dtNacimiento.Value = (DateTime)r["fecha_nacimiento"];
                cboGenero.Text        = r["genero"]?.ToString()!;
                cboEstadoCivil.Text   = r["estado_civil"]?.ToString()!;
                cboNivelEdu.Text      = r["nivel_educativo"]?.ToString()!;
                cboTipoContrato.Text  = r["tipo_contrato"]?.ToString()!;
                cboEstado.Text        = r["estado"]?.ToString()!;
                cboTipoCuenta.Text    = r["tipo_cuenta"]?.ToString()!;
                editId = id;
                panelForm.Visible = true;
                tabForm.SelectedIndex = 0;
            }
        }
        catch (Exception ex) { ShowMessage(ex.Message, true); }
    }

    void Guardar()
    {
        if (string.IsNullOrWhiteSpace(txNombre.Text) || string.IsNullOrWhiteSpace(txApellido.Text))
        { ShowMessage("Nombre y apellido son obligatorios.", true); return; }
        if (string.IsNullOrWhiteSpace(txDoc.Text))
        { ShowMessage("El documento es obligatorio.", true); return; }

        var suc = cboSucursal.SelectedValue;

        string sql = editId == -1
            ? @"INSERT INTO Empleados(nombre,apellido,documento,telefono,email,direccion,cargo,
                salario,fecha_contrato,tipo_contrato,estado,id_sucursal,num_hijos,estado_civil,
                nivel_educativo,eps,pension,cuenta_bancaria,banco,tipo_cuenta,
                contacto_emergencia,telefono_emergencia,fecha_nacimiento,genero,nacionalidad)
                VALUES(@n,@a,@d,@t,@e,@dir,@c,@s,@f,@tc,@st,@sc,@nh,@ec,@ne,@eps,@pen,
                @cb,@ban,@tcu,@ce,@te,@fn,@gen,@nac)"
            : @"UPDATE Empleados SET nombre=@n,apellido=@a,documento=@d,telefono=@t,email=@e,
                direccion=@dir,cargo=@c,salario=@s,fecha_contrato=@f,tipo_contrato=@tc,estado=@st,
                id_sucursal=@sc,num_hijos=@nh,estado_civil=@ec,nivel_educativo=@ne,eps=@eps,
                pension=@pen,cuenta_bancaria=@cb,banco=@ban,tipo_cuenta=@tcu,
                contacto_emergencia=@ce,telefono_emergencia=@te,fecha_nacimiento=@fn,
                genero=@gen,nacionalidad=@nac WHERE id_empleado=@id";

        RunQuery(sql, cmd =>
        {
            cmd.Parameters.AddWithValue("@n",   txNombre.Text);
            cmd.Parameters.AddWithValue("@a",   txApellido.Text);
            cmd.Parameters.AddWithValue("@d",   txDoc.Text);
            cmd.Parameters.AddWithValue("@t",   txTel.Text);
            cmd.Parameters.AddWithValue("@e",   txEmail.Text);
            cmd.Parameters.AddWithValue("@dir", txDir.Text);
            cmd.Parameters.AddWithValue("@c",   txCargo.Text);
            cmd.Parameters.AddWithValue("@s",   numSalario.Value);
            cmd.Parameters.AddWithValue("@f",   dtContrato.Value.Date);
            cmd.Parameters.AddWithValue("@tc",  cboTipoContrato.Text);
            cmd.Parameters.AddWithValue("@st",  cboEstado.Text);
            cmd.Parameters.AddWithValue("@sc",  suc ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@nh",  (int)numHijos.Value);
            cmd.Parameters.AddWithValue("@ec",  cboEstadoCivil.Text);
            cmd.Parameters.AddWithValue("@ne",  cboNivelEdu.Text);
            cmd.Parameters.AddWithValue("@eps", txEps.Text);
            cmd.Parameters.AddWithValue("@pen", txPension.Text);
            cmd.Parameters.AddWithValue("@cb",  txCuentaBancaria.Text);
            cmd.Parameters.AddWithValue("@ban", txBanco.Text);
            cmd.Parameters.AddWithValue("@tcu", cboTipoCuenta.Text);
            cmd.Parameters.AddWithValue("@ce",  txContactoEmergencia.Text);
            cmd.Parameters.AddWithValue("@te",  txTelEmergencia.Text);
            cmd.Parameters.AddWithValue("@fn",  dtNacimiento.Value.Date);
            cmd.Parameters.AddWithValue("@gen", cboGenero.Text);
            cmd.Parameters.AddWithValue("@nac", txNacionalidad.Text);
            if (editId != -1) cmd.Parameters.AddWithValue("@id", editId);
        }, editId == -1 ? "✅ Empleado creado." : "✅ Empleado actualizado.");

        panelForm.Visible = false;
        editId = -1;
    }

    void DeleteSelected()
    {
        if (grid.CurrentRow == null) return;
        if (MessageBox.Show("¿Desactivar este empleado?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        RunQuery("UPDATE Empleados SET estado='Inactivo' WHERE id_empleado=@id",
            cmd => cmd.Parameters.AddWithValue("@id", id), "Empleado desactivado.");
    }
}
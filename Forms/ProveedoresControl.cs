using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class ProveedoresControl : BaseGridControl
{
    protected override string ModuleTitle => "🏭  Proveedores";

    TextBox txNombre=null!, txTel=null!, txEmail=null!,
            txDir=null!, txCiudad=null!, txContacto=null!;
    int editId=-1;

    protected override void BuildToolbar(Panel p)
    {
        var layout = new FlowLayoutPanel { Dock=DockStyle.Fill, FlowDirection=FlowDirection.LeftToRight, Padding=new Padding(0,8,0,0), BackColor=Color.Transparent };
        txtSearch = MakeTextBox("🔍  Buscar proveedor...");
        txtSearch.Width = 260;
        txtSearch.TextChanged += (s,e) => LoadData();
        layout.Controls.AddRange([txtSearch,
            MakeButton("+ Nuevo", Accent, (s,e) => ShowForm(-1)),
            MakeButton("✏ Editar", Color.FromArgb(60,100,200), (s,e) => EditSelected()),
            MakeButton("✕ Desactivar", Color.FromArgb(180,50,50), (s,e) => DeleteSelected()),
            MakeButton("↻", Color.FromArgb(40,40,55), (s,e) => LoadData())]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p)
    {
        p.Width = 310;
        var title = new Label { Text="🏭  Proveedor", Font=new Font("Segoe UI",11f,FontStyle.Bold), ForeColor=Accent, Dock=DockStyle.Top, Height=36 };
        txNombre   = MakeTextBox("Nombre empresa");
        txTel      = MakeTextBox("Teléfono");
        txEmail    = MakeTextBox("Email");
        txDir      = MakeTextBox("Dirección");
        txCiudad   = MakeTextBox("Ciudad");
        txContacto = MakeTextBox("Persona de contacto");

        var flow = new FlowLayoutPanel { Dock=DockStyle.Fill, FlowDirection=FlowDirection.TopDown, WrapContents=false, AutoScroll=true, BackColor=Color.Transparent };
        void A(string l, Control c) { c.Width=270; flow.Controls.Add(MakeLabel(l)); flow.Controls.Add(c); flow.Controls.Add(new Panel{Height=6,Width=270,BackColor=Color.Transparent}); }
        A("Nombre *", txNombre); A("Teléfono", txTel); A("Email", txEmail);
        A("Dirección", txDir); A("Ciudad", txCiudad); A("Contacto", txContacto);
        flow.Controls.Add(MakeButton("💾 Guardar", Accent, (s,e) => Guardar()));
        flow.Controls.Add(new Panel{Height=8,Width=270,BackColor=Color.Transparent});
        flow.Controls.Add(MakeButton("✕ Cancelar", Color.FromArgb(60,60,80), (s,e) => { panelForm.Visible=false; editId=-1; }));
        p.Controls.AddRange([flow, title]); flow.Dock=DockStyle.Fill;
    }

    protected override void LoadData()
    {
        var t = txtSearch?.Text.Trim() ?? "";
        var dt = FetchTable(@"SELECT id_proveedor AS ID, nombre AS Nombre, telefono AS Teléfono,
            email AS Email, ciudad AS Ciudad, contacto AS Contacto,
            CASE activo WHEN 1 THEN '✓ Activo' ELSE '✗ Inactivo' END AS Estado
            FROM Proveedores WHERE @t='' OR LOWER(nombre) LIKE '%'+@t+'%' ORDER BY nombre",
            cmd => cmd.Parameters.AddWithValue("@t", t.ToLower()));
        grid.DataSource = dt;
        if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;
    }

    void ShowForm(int id) { editId=id; if(id==-1) txNombre.Text=txTel.Text=txEmail.Text=txDir.Text=txCiudad.Text=txContacto.Text=""; panelForm.Visible=true; }

    void EditSelected()
    {
        if (grid.CurrentRow==null) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        try {
            using var conn=DBConnection.GetConnection();
            using var cmd=new SqlCommand("SELECT * FROM Proveedores WHERE id_proveedor=@id",conn);
            cmd.Parameters.AddWithValue("@id",id);
            using var r=cmd.ExecuteReader();
            if(r.Read()){txNombre.Text=r["nombre"].ToString()!;txTel.Text=r["telefono"].ToString()!;txEmail.Text=r["email"].ToString()!;txDir.Text=r["direccion"].ToString()!;txCiudad.Text=r["ciudad"].ToString()!;txContacto.Text=r["contacto"].ToString()!;editId=id;panelForm.Visible=true;}
        } catch(Exception ex){ShowMessage(ex.Message,true);}
    }

    void Guardar()
    {
        if(string.IsNullOrWhiteSpace(txNombre.Text)){ShowMessage("Nombre requerido.",true);return;}
        if(editId==-1)
            RunQuery("INSERT INTO Proveedores(nombre,telefono,email,direccion,ciudad,contacto)VALUES(@n,@t,@e,@d,@c,@co)",
                cmd=>{cmd.Parameters.AddWithValue("@n",txNombre.Text);cmd.Parameters.AddWithValue("@t",txTel.Text);cmd.Parameters.AddWithValue("@e",txEmail.Text);cmd.Parameters.AddWithValue("@d",txDir.Text);cmd.Parameters.AddWithValue("@c",txCiudad.Text);cmd.Parameters.AddWithValue("@co",txContacto.Text);},"✅ Proveedor creado.");
        else
            RunQuery("UPDATE Proveedores SET nombre=@n,telefono=@t,email=@e,direccion=@d,ciudad=@c,contacto=@co WHERE id_proveedor=@id",
                cmd=>{cmd.Parameters.AddWithValue("@n",txNombre.Text);cmd.Parameters.AddWithValue("@t",txTel.Text);cmd.Parameters.AddWithValue("@e",txEmail.Text);cmd.Parameters.AddWithValue("@d",txDir.Text);cmd.Parameters.AddWithValue("@c",txCiudad.Text);cmd.Parameters.AddWithValue("@co",txContacto.Text);cmd.Parameters.AddWithValue("@id",editId);},"✅ Proveedor actualizado.");
        panelForm.Visible=false; editId=-1;
    }

    void DeleteSelected()
    {
        if(grid.CurrentRow==null) return;
        if(MessageBox.Show("¿Desactivar proveedor?","Confirmar",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)!=DialogResult.Yes) return;
        int id=Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        RunQuery("UPDATE Proveedores SET activo=0 WHERE id_proveedor=@id",cmd=>cmd.Parameters.AddWithValue("@id",id),"Proveedor desactivado.");
    }
}
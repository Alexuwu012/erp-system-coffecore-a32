using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class TurnosControl : BaseGridControl
{
    protected override string ModuleTitle => "⏰  Turnos de Empleados";

    ComboBox cboEmpleado=null!;
    DateTimePicker dtFecha=null!, dtInicio=null!, dtFin=null!;
    int editId=-1;

    protected override void BuildToolbar(Panel p)
    {
        var layout = new FlowLayoutPanel { Dock=DockStyle.Fill, FlowDirection=FlowDirection.LeftToRight, Padding=new Padding(0,8,0,0), BackColor=Color.Transparent };
        txtSearch = MakeTextBox("🔍  Buscar empleado...");
        txtSearch.Width = 240;
        txtSearch.TextChanged += (s,e) => LoadData();
        layout.Controls.AddRange([txtSearch,
            MakeButton("+ Nuevo Turno", Accent, (s,e) => ShowForm(-1)),
            MakeButton("✏ Editar", Color.FromArgb(60,100,200), (s,e) => EditSelected()),
            MakeButton("✕ Eliminar", Color.FromArgb(180,50,50), (s,e) => DeleteSelected()),
            MakeButton("↻", Color.FromArgb(40,40,55), (s,e) => LoadData())]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p)
    {
        p.Width = 310;
        var title = new Label { Text="⏰  Turno", Font=new Font("Segoe UI",11f,FontStyle.Bold), ForeColor=Accent, Dock=DockStyle.Top, Height=36 };
        cboEmpleado = new ComboBox { BackColor=Color.FromArgb(32,32,46), ForeColor=TextLight, FlatStyle=FlatStyle.Flat, DisplayMember="nombre", ValueMember="id" };
        dtFecha   = new DateTimePicker { Format=DateTimePickerFormat.Short };
        dtInicio  = new DateTimePicker { Format=DateTimePickerFormat.Time, ShowUpDown=true };
        dtFin     = new DateTimePicker { Format=DateTimePickerFormat.Time, ShowUpDown=true };

        try {
            var dt=new System.Data.DataTable();
            using var conn=DBConnection.GetConnection();
            using var da=new SqlDataAdapter("SELECT id_empleado AS id,nombre+' '+apellido AS nombre FROM Empleados WHERE estado='Activo'",conn);
            da.Fill(dt); cboEmpleado.DataSource=dt;
        } catch {}

        var flow = new FlowLayoutPanel { Dock=DockStyle.Fill, FlowDirection=FlowDirection.TopDown, WrapContents=false, AutoScroll=true, BackColor=Color.Transparent };
        void A(string l,Control c){c.Width=270;flow.Controls.Add(MakeLabel(l));flow.Controls.Add(c);flow.Controls.Add(new Panel{Height=6,Width=270,BackColor=Color.Transparent});}
        A("Empleado *",cboEmpleado); A("Fecha",dtFecha); A("Hora Inicio",dtInicio); A("Hora Fin",dtFin);
        flow.Controls.Add(MakeButton("💾 Guardar",Accent,(s,e)=>Guardar()));
        flow.Controls.Add(new Panel{Height=8,Width=270,BackColor=Color.Transparent});
        flow.Controls.Add(MakeButton("✕ Cancelar",Color.FromArgb(60,60,80),(s,e)=>{panelForm.Visible=false;editId=-1;}));
        p.Controls.AddRange([flow,title]); flow.Dock=DockStyle.Fill;
    }

    protected override void LoadData()
    {
        var t = txtSearch?.Text.Trim() ?? "";
        var dt = FetchTable(@"SELECT t.id_turno AS ID, e.nombre+' '+e.apellido AS Empleado,
            t.fecha AS Fecha, CONVERT(VARCHAR,t.hora_inicio,108) AS Inicio,
            CONVERT(VARCHAR,t.hora_fin,108) AS Fin,
            s.nombre AS Sucursal
            FROM Turnos t
            JOIN Empleados e ON e.id_empleado=t.id_empleado
            LEFT JOIN Sucursales s ON s.id_sucursal=e.id_sucursal
            WHERE @t='' OR LOWER(e.nombre+' '+e.apellido) LIKE '%'+@t+'%'
            ORDER BY t.fecha DESC, t.hora_inicio",
            cmd=>cmd.Parameters.AddWithValue("@t",t.ToLower()));
        grid.DataSource=dt;
        if(grid.Columns.Count>0) grid.Columns[0].Visible=false;
    }

    void ShowForm(int id){editId=id;panelForm.Visible=true;}

    void EditSelected()
    {
        if(grid.CurrentRow==null) return;
        int id=Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        try{
            using var conn=DBConnection.GetConnection();
            using var cmd=new SqlCommand("SELECT * FROM Turnos WHERE id_turno=@id",conn);
            cmd.Parameters.AddWithValue("@id",id);
            using var r=cmd.ExecuteReader();
            if(r.Read()){
                if(r["fecha"]!=DBNull.Value) dtFecha.Value=(DateTime)r["fecha"];
                if(r["hora_inicio"]!=DBNull.Value) dtInicio.Value=DateTime.Today.Add((TimeSpan)r["hora_inicio"]);
                if(r["hora_fin"]!=DBNull.Value) dtFin.Value=DateTime.Today.Add((TimeSpan)r["hora_fin"]);
                editId=id; panelForm.Visible=true;
            }
        }catch(Exception ex){ShowMessage(ex.Message,true);}
    }

    void Guardar()
    {
        if(cboEmpleado.SelectedValue==null){ShowMessage("Seleccione empleado.",true);return;}
        if(editId==-1)
            RunQuery("INSERT INTO Turnos(id_empleado,fecha,hora_inicio,hora_fin)VALUES(@e,@f,@hi,@hf)",
                cmd=>{cmd.Parameters.AddWithValue("@e",cboEmpleado.SelectedValue);cmd.Parameters.AddWithValue("@f",dtFecha.Value.Date);cmd.Parameters.AddWithValue("@hi",dtInicio.Value.TimeOfDay);cmd.Parameters.AddWithValue("@hf",dtFin.Value.TimeOfDay);},"✅ Turno creado.");
        else
            RunQuery("UPDATE Turnos SET id_empleado=@e,fecha=@f,hora_inicio=@hi,hora_fin=@hf WHERE id_turno=@id",
                cmd=>{cmd.Parameters.AddWithValue("@e",cboEmpleado.SelectedValue);cmd.Parameters.AddWithValue("@f",dtFecha.Value.Date);cmd.Parameters.AddWithValue("@hi",dtInicio.Value.TimeOfDay);cmd.Parameters.AddWithValue("@hf",dtFin.Value.TimeOfDay);cmd.Parameters.AddWithValue("@id",editId);},"✅ Turno actualizado.");
        panelForm.Visible=false; editId=-1;
    }

    void DeleteSelected()
    {
        if(grid.CurrentRow==null) return;
        if(MessageBox.Show("¿Eliminar turno?","Confirmar",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)!=DialogResult.Yes) return;
        int id=Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        RunQuery("DELETE FROM Turnos WHERE id_turno=@id",cmd=>cmd.Parameters.AddWithValue("@id",id),"Turno eliminado.");
    }
}
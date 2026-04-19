using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class InsumosControl : BaseGridControl
{
    protected override string ModuleTitle => "🧪  Insumos";

    TextBox txNombre=null!, txUnidad=null!, txDesc=null!;
    int editId=-1;

    protected override void BuildToolbar(Panel p)
    {
        var layout = new FlowLayoutPanel
        {
            Dock=DockStyle.Fill, FlowDirection=FlowDirection.LeftToRight,
            Padding=new Padding(0,8,0,0), BackColor=Color.Transparent
        };
        txtSearch = MakeTextBox("🔍  Buscar insumo...");
        txtSearch.Width = 260;
        txtSearch.TextChanged += (s,e) => LoadData();
        layout.Controls.AddRange([
            txtSearch,
            MakeButton("+ Nuevo", Accent, (s,e) => ShowForm(-1)),
            MakeButton("✏ Editar", Color.FromArgb(60,100,200), (s,e) => EditSelected()),
            MakeButton("✕ Desactivar", Color.FromArgb(180,50,50), (s,e) => DeleteSelected()),
            MakeButton("↻", Color.FromArgb(40,40,55), (s,e) => LoadData())
        ]);
        p.Controls.Add(layout);
    }

    protected override void BuildFormPanel(Panel p)
    {
        p.Width = 310;
        var title = new Label
        {
            Text = "🧪  Insumo",
            Font = new Font("Segoe UI",11f,FontStyle.Bold),
            ForeColor = Accent, Dock = DockStyle.Top, Height = 36
        };
        txNombre = MakeTextBox("Nombre del insumo");
        txUnidad = MakeTextBox("Unidad (kg, L, und, ml...)");
        txDesc   = MakeTextBox("Descripcion");

        var flow = new FlowLayoutPanel
        {
            Dock=DockStyle.Fill, FlowDirection=FlowDirection.TopDown,
            WrapContents=false, AutoScroll=true, BackColor=Color.Transparent
        };
        void A(string l, Control c)
        {
            c.Width=270;
            flow.Controls.Add(MakeLabel(l));
            flow.Controls.Add(c);
            flow.Controls.Add(new Panel{Height=6,Width=270,BackColor=Color.Transparent});
        }
        A("Nombre *", txNombre);
        A("Unidad de medida *", txUnidad);
        A("Descripcion", txDesc);
        flow.Controls.Add(MakeButton("💾 Guardar", Accent, (s,e) => Guardar()));
        flow.Controls.Add(new Panel{Height=8,Width=270,BackColor=Color.Transparent});
        flow.Controls.Add(MakeButton("✕ Cancelar", Color.FromArgb(60,60,80), (s,e) => {
            panelForm.Visible=false; editId=-1;
        }));
        p.Controls.AddRange([flow, title]);
        flow.Dock = DockStyle.Fill;
    }

    protected override void LoadData()
    {
        var t = txtSearch?.Text.Trim() ?? "";
        var dt = FetchTable(@"
            SELECT id_insumo AS ID, nombre AS Nombre, unidad AS Unidad,
                descripcion AS Descripcion,
                CASE activo WHEN 1 THEN '✓ Activo' ELSE '✗ Inactivo' END AS Estado
            FROM Insumos
            WHERE @t='' OR LOWER(nombre) LIKE '%'+@t+'%'
            ORDER BY nombre",
            cmd => cmd.Parameters.AddWithValue("@t", t.ToLower()));
        grid.DataSource = dt;
        if (grid.Columns.Count > 0) grid.Columns[0].Visible = false;
    }

    void ShowForm(int id)
    {
        editId = id;
        if (id == -1) txNombre.Text = txUnidad.Text = txDesc.Text = "";
        panelForm.Visible = true;
    }

    void EditSelected()
    {
        if (grid.CurrentRow == null) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        try
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand("SELECT * FROM Insumos WHERE id_insumo=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                txNombre.Text = r["nombre"].ToString()!;
                txUnidad.Text = r["unidad"].ToString()!;
                txDesc.Text   = r["descripcion"].ToString()!;
                editId = id;
                panelForm.Visible = true;
            }
        }
        catch (Exception ex) { ShowMessage(ex.Message, true); }
    }

    void Guardar()
    {
        if (string.IsNullOrWhiteSpace(txNombre.Text) || string.IsNullOrWhiteSpace(txUnidad.Text))
        { ShowMessage("Nombre y unidad son obligatorios.", true); return; }

        if (editId == -1)
            RunQuery("INSERT INTO Insumos(nombre,unidad,descripcion,activo)VALUES(@n,@u,@d,1)",
                cmd => {
                    cmd.Parameters.AddWithValue("@n", txNombre.Text);
                    cmd.Parameters.AddWithValue("@u", txUnidad.Text);
                    cmd.Parameters.AddWithValue("@d", txDesc.Text);
                }, "✅ Insumo creado.");
        else
            RunQuery("UPDATE Insumos SET nombre=@n,unidad=@u,descripcion=@d WHERE id_insumo=@id",
                cmd => {
                    cmd.Parameters.AddWithValue("@n", txNombre.Text);
                    cmd.Parameters.AddWithValue("@u", txUnidad.Text);
                    cmd.Parameters.AddWithValue("@d", txDesc.Text);
                    cmd.Parameters.AddWithValue("@id", editId);
                }, "✅ Insumo actualizado.");

        panelForm.Visible = false;
        editId = -1;
    }

    void DeleteSelected()
    {
        if (grid.CurrentRow == null) return;
        if (MessageBox.Show("¿Desactivar insumo?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        int id = Convert.ToInt32(grid.CurrentRow.Cells[0].Value);
        RunQuery("UPDATE Insumos SET activo=0 WHERE id_insumo=@id",
            cmd => cmd.Parameters.AddWithValue("@id", id), "Insumo desactivado.");
    }
}
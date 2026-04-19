using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public abstract class BaseGridControl : UserControl
{
    protected static readonly Color BgDark    = Color.FromArgb(18, 18, 24);
    protected static readonly Color BgCard    = Color.FromArgb(26, 26, 36);
    protected static readonly Color Accent    = Color.FromArgb(194, 139, 74);
    protected static readonly Color TextLight = Color.FromArgb(230, 230, 235);
    protected static readonly Color TextMuted = Color.FromArgb(140, 140, 155);
    protected static readonly Color Success   = Color.FromArgb(80, 200, 120);
    protected static readonly Color Danger    = Color.FromArgb(220, 80, 80);
    protected static readonly Color GridBg    = Color.FromArgb(22, 22, 32);

    protected DataGridView grid = null!;
    protected TextBox txtSearch = null!;
    protected Panel panelTop;
    protected Panel panelForm;
    protected Label lblModTitle;

    protected BaseGridControl()
    {
        BackColor = BgDark;
        Font = new Font("Segoe UI", 9.5f);
        panelTop = new Panel();
        panelForm = new Panel();
        lblModTitle = new Label();
        grid = new DataGridView();
        txtSearch = new TextBox();
        BuildBase();
    }

    void BuildBase()
    {
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = BgCard,
            Padding = new Padding(20, 0, 20, 0)
        };
        lblModTitle = new Label
        {
            Text = ModuleTitle,
            Font = new Font("Segoe UI", 14f, FontStyle.Bold),
            ForeColor = Accent,
            AutoSize = true,
            Location = new Point(20, 16)
        };
        header.Controls.Add(lblModTitle);

        panelTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 52,
            BackColor = Color.FromArgb(22, 22, 32),
            Padding = new Padding(16, 0, 16, 0)
        };
        BuildToolbar(panelTop);

        grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = GridBg,
            BorderStyle = BorderStyle.None,
            GridColor = Color.FromArgb(40, 40, 55),
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeight = 38,
            RowTemplate = { Height = 36 },
            Font = new Font("Segoe UI", 9.5f),
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        };
        grid.DefaultCellStyle.BackColor = GridBg;
        grid.DefaultCellStyle.ForeColor = TextLight;
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(60, 194, 139, 74);
        grid.DefaultCellStyle.SelectionForeColor = Color.White;
        grid.DefaultCellStyle.NullValue = "—";
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 44);
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Accent;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 30, 44);
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 34);
        grid.EnableHeadersVisualStyles = false;
        grid.DoubleClick += (s, e) => OnRowDoubleClick();
        grid.SelectionChanged += (s, e) => OnSelectionChanged();

        panelForm = new Panel
        {
            Dock = DockStyle.Right,
            Width = 320,
            BackColor = BgCard,
            Padding = new Padding(16),
            Visible = false
        };
        BuildFormPanel(panelForm);

        var bottomLine = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 1,
            BackColor = Color.FromArgb(40, 40, 55)
        };

        Controls.Add(grid);
        Controls.Add(panelForm);
        Controls.Add(panelTop);
        Controls.Add(header);
        Controls.Add(bottomLine);

        LoadData();
    }

    protected abstract string ModuleTitle { get; }
    protected abstract void BuildToolbar(Panel p);
    protected abstract void BuildFormPanel(Panel p);
    protected abstract void LoadData();
    protected virtual void OnRowDoubleClick() { }
    protected virtual void OnSelectionChanged() { }

    protected Button MakeButton(string text, Color bg, EventHandler onClick)
    {
        var btn = new Button
        {
            Text = text,
            BackColor = bg,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Height = 34,
            Cursor = Cursors.Hand,
            AutoSize = true,
            Padding = new Padding(12, 0, 12, 0)
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.Click += onClick;
        return btn;
    }

    protected TextBox MakeTextBox(string placeholder = "")
    {
        return new TextBox
        {
            BackColor = Color.FromArgb(32, 32, 46),
            ForeColor = TextLight,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 10f),
            PlaceholderText = placeholder,
            Height = 32
        };
    }

    protected Label MakeLabel(string text)
    {
        return new Label
        {
            Text = text,
            ForeColor = TextMuted,
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = true
        };
    }

    protected void ShowMessage(string msg, bool error = false)
    {
        MessageBox.Show(msg, "Coffee ERP",
            MessageBoxButtons.OK,
            error ? MessageBoxIcon.Error : MessageBoxIcon.Information);
    }

    protected void RunQuery(string sql, Action<SqlCommand> setup, string successMsg)
    {
        try
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            setup(cmd);
            cmd.ExecuteNonQuery();
            ShowMessage(successMsg);
            LoadData();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    protected System.Data.DataTable FetchTable(string sql, Action<SqlCommand>? setup = null)
    {
        var dt = new System.Data.DataTable();
        try
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            setup?.Invoke(cmd);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
        return dt;
    }
}
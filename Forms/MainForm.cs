using CoffeeERP.Database;
using Microsoft.Data.SqlClient;

namespace CoffeeERP.Forms;

public class MainForm : Form
{
    private Panel panelSidebar  = null!;
    private Panel panelContent  = null!;
    private Panel panelHeader   = null!;
    private Label lblConStatus  = null!;
    private Label lblStockAlert = null!;
    private UserControl? currentControl;
    private System.Windows.Forms.Timer alertTimer = null!;

    static readonly Color BgDark      = Color.FromArgb(12, 12, 18);
    static readonly Color Sidebar      = Color.FromArgb(18, 18, 28);
    static readonly Color SidebarHover = Color.FromArgb(28, 28, 42);
    static readonly Color Accent       = Color.FromArgb(194, 139, 74);
    static readonly Color TextLight    = Color.FromArgb(230, 230, 235);
    static readonly Color TextMuted    = Color.FromArgb(120, 120, 140);
    static readonly Color Success      = Color.FromArgb(80, 200, 120);
    static readonly Color Danger       = Color.FromArgb(220, 80, 80);

    record NavItem(string Icon, string Label, Func<UserControl> Factory);

    readonly NavItem[] navItems = GetNavItems();

    static NavItem[] GetNavItems()
    {
        var rol = LoginForm.CurrentRole;
        var todos = new NavItem[]
        {
            new("📊", "Dashboard",    () => new DashboardControl()),
            new("👥", "Clientes",     () => new ClientesControl()),
            new("🧑", "Empleados",    () => new EmpleadosControl()),
            new("⏰", "Turnos",       () => new TurnosControl()),
            new("🏢", "Sucursales",   () => new SucursalesControl()),
            new("🍽", "Productos",    () => new ProductosControl()),
            new("🏷", "Categorias",   () => new CategoriasControl()),
            new("📖", "Recetas",      () => new RecetasControl()),
            new("📦", "Inventario",   () => new InventarioControl()),
            new("🧪", "Insumos",      () => new InsumosControl()),
            new("🛒", "Ventas",       () => new VentasControl()),
            new("🛍", "Compras",      () => new ComprasControl()),
            new("🏭", "Proveedores",  () => new ProveedoresControl()),
            new("💰", "Finanzas",     () => new FinanzasControl()),
            new("📋", "Reportes",     () => new ReportesControl()),
            new("📜", "Auditoria",    () => new AuditoriaControl()),
            new("⚙",  "Configuracion",() => new ConfiguracionControl()),
        };

        return rol switch
        {
            "Administrador" => todos,

            "Gerente" => todos.Where(n => n.Label is
                "Dashboard" or "Clientes" or "Empleados" or "Turnos" or
                "Productos" or "Categorias" or "Recetas" or
                "Inventario" or "Insumos" or "Ventas" or
                "Finanzas" or "Reportes").ToArray(),

            "RRHH" => todos.Where(n => n.Label is
                "Dashboard" or "Empleados" or "Turnos" or
                "Sucursales" or "Reportes").ToArray(),

            "Cajero" => todos.Where(n => n.Label is
                "Dashboard" or "Clientes" or "Ventas").ToArray(),

            "Inventario" => todos.Where(n => n.Label is
                "Dashboard" or "Inventario" or "Insumos" or
                "Productos" or "Categorias" or "Recetas" or
                "Compras" or "Proveedores").ToArray(),

            "Contador" => todos.Where(n => n.Label is
                "Dashboard" or "Finanzas" or "Reportes" or "Auditoria").ToArray(),

            _ => todos.Where(n => n.Label is "Dashboard").ToArray()
        };
    }

    public MainForm()
    {
        InitializeComponent();
        LoadSection(0);
        StartAlertTimer();
    }

    void InitializeComponent()
    {
        Size = new Size(1366, 850);
        MinimumSize = new Size(1200, 700);
        Text = "Coffee ERP";
        BackColor = BgDark;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5f);

        // Header
        panelHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 58,
            BackColor = Color.FromArgb(16, 16, 24)
        };
        var headerAccent = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 2,
            BackColor = Accent
        };
        var lblTitle = new Label
        {
            Text = "☕  Coffee ERP",
            ForeColor = Accent,
            Font = new Font("Segoe UI", 14f, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(250, 14)
        };
        var lblUser = new Label
        {
            Text = $"👤  {LoginForm.CurrentUser}  |  {LoginForm.CurrentRole}",
            ForeColor = TextMuted,
            Font = new Font("Segoe UI", 9f),
            AutoSize = true,
            Location = new Point(450, 20)
        };
        lblConStatus = new Label
        {
            Text = "● Conectando...",
            ForeColor = TextMuted,
            Font = new Font("Segoe UI", 9f),
            AutoSize = true,
            Anchor = AnchorStyles.Right | AnchorStyles.Top
        };
        lblStockAlert = new Label
        {
            Text = "",
            ForeColor = Color.FromArgb(255, 160, 50),
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            AutoSize = true,
            Anchor = AnchorStyles.Right | AnchorStyles.Top,
            Visible = false
        };
        panelHeader.Controls.AddRange([lblTitle, lblUser, lblConStatus, lblStockAlert, headerAccent]);
        panelHeader.Resize += (s, e) =>
        {
            lblConStatus.Location  = new Point(panelHeader.Width - lblConStatus.Width - 20, 20);
            lblStockAlert.Location = new Point(panelHeader.Width - lblStockAlert.Width - 200, 20);
        };

        // Sidebar
        panelSidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 220,
            BackColor = Sidebar
        };

        var logoPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 120,
            BackColor = Color.FromArgb(14, 14, 22)
        };
        var logoAccent = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 1,
            BackColor = Color.FromArgb(50, 50, 70)
        };
        var lblLogo = new Label
        {
            Text = "☕",
            Font = new Font("Segoe UI", 36f),
            ForeColor = Accent,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill
        };
        var lblAppName = new Label
        {
            Text = "COFFEE ERP",
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = TextLight,
            TextAlign = ContentAlignment.BottomCenter,
            Dock = DockStyle.Bottom,
            Height = 24
        };
        var lblAppSub = new Label
        {
            Text = "Sistema de Gestion",
            Font = new Font("Segoe UI", 7.5f),
            ForeColor = TextMuted,
            TextAlign = ContentAlignment.TopCenter,
            Dock = DockStyle.Bottom,
            Height = 18
        };
        logoPanel.Controls.AddRange([lblLogo, lblAppName, lblAppSub, logoAccent]);

        var navScroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.Transparent,
            Padding = new Padding(8, 8, 8, 8)
        };

        var navFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            Width = 200,
            BackColor = Color.Transparent,
            Padding = new Padding(0)
        };

        for (int i = 0; i < navItems.Length; i++)
        {
            int idx = i;
            var item = navItems[i];

            // Separadores visuales
            if (item.Label is "Ventas" or "Finanzas" or "Auditoria" or "Configuracion")
            {
                var sep = new Panel
                {
                    Width = 196, Height = 1,
                    BackColor = Color.FromArgb(40, 40, 60),
                    Margin = new Padding(0, 4, 0, 4)
                };
                navFlow.Controls.Add(sep);
            }

            var btn = new Panel
            {
                Width = 196, Height = 40,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 1, 0, 1),
                Tag = idx
            };
            var btnIcon = new Label
            {
                Text = item.Icon,
                Font = new Font("Segoe UI", 13f),
                ForeColor = TextMuted,
                Location = new Point(10, 8),
                Size = new Size(28, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };
            var btnText = new Label
            {
                Text = item.Label,
                Font = new Font("Segoe UI", 10f),
                ForeColor = TextMuted,
                Location = new Point(44, 10),
                Size = new Size(140, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            var btnAccent = new Panel
            {
                Width = 3, Height = 40,
                BackColor = Color.Transparent,
                Dock = DockStyle.Left
            };

            btn.Controls.AddRange([btnAccent, btnIcon, btnText]);

            void SetActive(Panel b, Label icon, Label text, Panel acc, bool active, bool hover = false)
            {
                b.BackColor   = active ? Color.FromArgb(35, 194, 139, 74) : hover ? SidebarHover : Color.Transparent;
                icon.ForeColor = active ? Accent : hover ? TextLight : TextMuted;
                text.ForeColor = active ? Accent : hover ? TextLight : TextMuted;
                text.Font      = new Font("Segoe UI", 10f, active ? FontStyle.Bold : FontStyle.Regular);
                acc.BackColor  = active ? Accent : Color.Transparent;
            }

            btn.MouseEnter += (s, e) => SetActive(btn, btnIcon, btnText, btnAccent, (int)btn.Tag! == activeIndex, true);
            btn.MouseLeave += (s, e) => SetActive(btn, btnIcon, btnText, btnAccent, (int)btn.Tag! == activeIndex, false);
            btn.Click      += (s, e) => LoadSection(idx);
            btnIcon.Click  += (s, e) => LoadSection(idx);
            btnText.Click  += (s, e) => LoadSection(idx);

            navFlow.Controls.Add(btn);
        }

        // Logout
        var sepLogout = new Panel
        {
            Width = 196, Height = 1,
            BackColor = Color.FromArgb(40, 40, 60),
            Margin = new Padding(0, 8, 0, 4)
        };
        var btnLogout = new Panel
        {
            Width = 196, Height = 40,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand
        };
        var lblLogout = new Label
        {
            Text = "🚪  Cerrar Sesion",
            Font = new Font("Segoe UI", 10f),
            ForeColor = Color.FromArgb(180, 80, 80),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };
        btnLogout.Controls.Add(lblLogout);
        btnLogout.Click  += (s, e) => CerrarSesion();
        lblLogout.Click  += (s, e) => CerrarSesion();
        navFlow.Controls.Add(sepLogout);
        navFlow.Controls.Add(btnLogout);

        navScroll.Controls.Add(navFlow);
        panelSidebar.Controls.AddRange([navScroll, logoPanel]);

        // Content
        panelContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = BgDark
        };

        Controls.AddRange([panelContent, panelSidebar, panelHeader]);

        // Check connection async
        Task.Run(async () =>
        {
            await Task.Delay(600);
            bool ok = DBConnection.TestConnection();
            Invoke(() =>
            {
                lblConStatus.Text     = ok ? "● Conectado" : "● Sin conexion";
                lblConStatus.ForeColor = ok ? Success : Danger;
                lblConStatus.Location  = new Point(panelHeader.Width - lblConStatus.Width - 20, 20);
            });
        });
    }

    void StartAlertTimer()
    {
        alertTimer = new System.Windows.Forms.Timer { Interval = 30000 };
        alertTimer.Tick += (s, e) => CheckStockAlerts();
        alertTimer.Start();
        Task.Run(async () => { await Task.Delay(2000); Invoke(CheckStockAlerts); });
    }

    void CheckStockAlerts()
    {
        try
        {
            using var conn = DBConnection.GetConnection();
            using var cmd  = new SqlCommand(
                "SELECT COUNT(*) FROM InventarioInsumos WHERE stock_actual < stock_minimo", conn);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            lblStockAlert.Text    = count > 0 ? $"⚠ {count} insumo(s) con stock bajo" : "";
            lblStockAlert.Visible = count > 0;
            lblStockAlert.Location = new Point(panelHeader.Width - lblStockAlert.Width - 200, 20);
        }
        catch { }
    }

    int activeIndex = -1;

    void LoadSection(int idx)
    {
        activeIndex = idx;

        var navScroll2 = panelSidebar.Controls.OfType<Panel>().First(p => p.Dock == DockStyle.Fill);
        var navFlow    = navScroll2.Controls.OfType<FlowLayoutPanel>().First();

        foreach (Control ctrl in navFlow.Controls)
        {
            if (ctrl is Panel btn && btn.Tag is int btnIdx)
            {
                bool active = btnIdx == idx;
                btn.BackColor = active ? Color.FromArgb(35, 194, 139, 74) : Color.Transparent;
                foreach (Control c in btn.Controls)
                {
                    if (c is Label lbl)
                    {
                        lbl.ForeColor = active ? Accent : TextMuted;
                        if (lbl.Width > 30)
                            lbl.Font = new Font("Segoe UI", 10f, active ? FontStyle.Bold : FontStyle.Regular);
                    }
                    if (c is Panel acc && acc.Width == 3)
                        acc.BackColor = active ? Accent : Color.Transparent;
                }
            }
        }

        panelContent.Controls.Clear();
        currentControl?.Dispose();

        try
        {
            currentControl = navItems[idx].Factory();
            currentControl.Dock = DockStyle.Fill;
            panelContent.Controls.Add(currentControl);

            // Registrar en auditoria
            DBConnection.RegistrarAuditoria(navItems[idx].Label, $"Accedio al modulo {navItems[idx].Label}");
        }
        catch (Exception ex)
        {
            var err = new Label
            {
                Text = $"Error cargando modulo:\n{ex.Message}",
                ForeColor = Danger,
                Font = new Font("Segoe UI", 11f),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelContent.Controls.Add(err);
        }
    }

    void CerrarSesion()
    {
        if (MessageBox.Show("¿Cerrar sesion?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        alertTimer?.Stop();
        DBConnection.RegistrarAuditoria("Sistema", "Cerro sesion");
        LoginForm.CurrentUser     = "";
        LoginForm.CurrentRole     = "";
        LoginForm.CurrentSucursal = 0;
        Close();
    }
}
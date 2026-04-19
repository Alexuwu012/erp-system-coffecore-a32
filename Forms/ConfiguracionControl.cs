using System.Text.Json;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class ConfiguracionControl : UserControl
{
    static readonly Color BgDark = Color.FromArgb(18, 18, 24);
    static readonly Color BgCard = Color.FromArgb(26, 26, 36);
    static readonly Color Accent = Color.FromArgb(194, 139, 74);
    static readonly Color TextLight = Color.FromArgb(230, 230, 235);
    static readonly Color TextMuted = Color.FromArgb(140, 140, 155);

    // Ruta del archivo de configuración (junto al .exe)
    static readonly string ConfigPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "config.json");

    TextBox txServer = null!, txDb = null!, txUser = null!, txPass = null!;
    Label lblStatus = null!;

    // ── Modelo de configuración ──────────────────────────────────────
    record AppConfig(
        string Server,
        string Database,
        string User,
        string Password);

    // ── Métodos estáticos de carga/guardado ──────────────────────────

    /// <summary>
    /// Llamar desde Program.cs antes de abrir cualquier Form.
    /// Carga config.json y actualiza DBConnection.
    /// </summary>
    public static void CargarConfiguracion()
    {
        try
        {
            if (!File.Exists(ConfigPath)) return;
            var json = File.ReadAllText(ConfigPath);
            var cfg = JsonSerializer.Deserialize<AppConfig>(json);
            if (cfg is null) return;

            DBConnection.Server = cfg.Server;
            DBConnection.Database = cfg.Database;
            DBConnection.User = cfg.User;
            DBConnection.Password = cfg.Password;
        }
        catch { /* Si el archivo está corrupto, se ignora y se usa el default */ }
    }

    static void GuardarConfiguracion(string server, string db, string user, string pass)
    {
        var cfg = new AppConfig(server, db, user, pass);
        var json = JsonSerializer.Serialize(cfg, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }

    // ── Constructor ──────────────────────────────────────────────────
    public ConfiguracionControl()
    {
        BackColor = BgDark;
        Font = new Font("Segoe UI", 9.5f);
        BuildUI();
    }

    void BuildUI()
    {
        var header = new Label
        {
            Text = "⚙  Configuracion de Conexion",
            Font = new Font("Segoe UI", 14f, FontStyle.Bold),
            ForeColor = Accent,
            Dock = DockStyle.Top,
            Height = 60,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(20, 0, 0, 0)
        };

        var card = new Panel
        {
            Width = 520,
            Height = 450,
            BackColor = BgCard,
            Location = new Point(40, 80)
        };

        // ── Campos de texto ──────────────────────────────────────────
        txServer = MakeTx(DBConnection.Server, 55);
        txDb = MakeTx(DBConnection.Database, 120);
        txUser = MakeTx(DBConnection.User, 185);
        txPass = MakeTx(DBConnection.Password, 250);
        txPass.PasswordChar = '●';

        TextBox MakeTx(string valor, int top) => new TextBox
        {
            Location = new Point(20, top),
            Width = 460,
            BackColor = Color.FromArgb(32, 32, 46),
            ForeColor = TextLight,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 10f),
            Text = valor
        };

        Label MakeLbl(string texto, int top) => new Label
        {
            Text = texto,
            ForeColor = TextMuted,
            Location = new Point(20, top),
            AutoSize = true
        };

        lblStatus = new Label
        {
            ForeColor = Accent,
            Location = new Point(20, 380),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
        };

        // ── Botón Guardar y Probar ───────────────────────────────────
        var btnGuardar = new Button
        {
            Text = "💾  Guardar y Probar Conexion",
            Location = new Point(20, 320),
            Width = 240,
            Height = 42,
            BackColor = Accent,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnGuardar.FlatAppearance.BorderSize = 0;
        btnGuardar.Click += (s, e) => GuardarYProbar();

        // ── Indicador de archivo de config ───────────────────────────
        var lblRuta = new Label
        {
            Text = $"📁  Archivo: {ConfigPath}",
            ForeColor = TextMuted,
            Location = new Point(20, 410),
            Size = new Size(460, 18),
            Font = new Font("Segoe UI", 7.5f)
        };

        card.Controls.AddRange([
            MakeLbl("Servidor (ej: .\\SQLEXPRESS o IP):", 35), txServer,
            MakeLbl("Base de datos:", 100), txDb,
            MakeLbl("Usuario SQL (vacío = autenticación Windows):", 165), txUser,
            MakeLbl("Contraseña:", 230), txPass,
            btnGuardar,
            lblStatus,
            lblRuta
        ]);

        Controls.AddRange([card, header]);
    }

    // ── Lógica de guardar ────────────────────────────────────────────
    void GuardarYProbar()
    {
        var server = txServer.Text.Trim();
        var db = txDb.Text.Trim();
        var user = txUser.Text.Trim();
        var pass = txPass.Text;

        if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(db))
        {
            lblStatus.Text = "⚠  Servidor y base de datos son obligatorios.";
            lblStatus.ForeColor = Color.FromArgb(255, 160, 50);
            return;
        }

        // 1. Actualizar DBConnection en memoria
        DBConnection.Server = server;
        DBConnection.Database = db;
        DBConnection.User = user;
        DBConnection.Password = pass;

        lblStatus.Text = "🔄  Probando conexion...";
        lblStatus.ForeColor = TextMuted;

        // 2. Probar conexión de forma asíncrona
        Task.Run(() =>
        {
            bool ok = DBConnection.TestConnection();

            Invoke(() =>
            {
                if (ok)
                {
                    // 3. Solo guardar en disco si la conexión es exitosa
                    GuardarConfiguracion(server, db, user, pass);
                    lblStatus.Text = "✅  Conexion exitosa — configuracion guardada.";
                    lblStatus.ForeColor = Color.FromArgb(80, 200, 120);
                }
                else
                {
                    // 4. Si falla, revertir DBConnection a lo que estaba guardado
                    CargarConfiguracion();
                    lblStatus.Text = "❌  Error de conexion. No se guardaron los cambios.";
                    lblStatus.ForeColor = Color.FromArgb(220, 80, 80);
                }
            });
        });
    }
}
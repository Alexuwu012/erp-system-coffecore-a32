using Microsoft.Data.SqlClient;
using CoffeeERP.Database;

namespace CoffeeERP.Forms;

public class LoginForm : Form
{
    static readonly Color BgDark    = Color.FromArgb(12, 12, 18);
    static readonly Color BgCard    = Color.FromArgb(22, 22, 32);
    static readonly Color Accent    = Color.FromArgb(194, 139, 74);
    static readonly Color TextLight = Color.FromArgb(230, 230, 235);
    static readonly Color TextMuted = Color.FromArgb(140, 140, 155);

    TextBox txEmail = null!, txPass = null!;
    Label lblError = null!;
    Button btnLogin = null!;

    public static string CurrentUser     = "";
    public static string CurrentRole     = "";
    public static int    CurrentSucursal = 0;

    public LoginForm()
    {
        Size = new Size(460, 580);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = BgDark;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Text = "Coffee ERP — Login";
        Font = new Font("Segoe UI", 9.5f);
        BuildUI();
    }

    void BuildUI()
    {
        var card = new Panel
        {
            Width = 380,
            Height = 460,
            BackColor = BgCard,
            Location = new Point(40, 60)
        };

        var topBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 4,
            BackColor = Accent
        };

        var lblLogo = new Label
        {
            Text = "☕",
            Font = new Font("Segoe UI", 42f),
            ForeColor = Accent,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(0, 20),
            Width = 380,
            Height = 70
        };

        var lblTitle = new Label
        {
            Text = "COFFEE ERP",
            Font = new Font("Segoe UI", 18f, FontStyle.Bold),
            ForeColor = TextLight,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(0, 95),
            Width = 380,
            Height = 32
        };

        var lblSub = new Label
        {
            Text = "Sistema de Gestion",
            Font = new Font("Segoe UI", 9f),
            ForeColor = TextMuted,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(0, 128),
            Width = 380,
            Height = 22
        };

        var lblEmail = new Label
        {
            Text = "Email",
            ForeColor = TextMuted,
            Font = new Font("Segoe UI", 8.5f),
            Location = new Point(40, 170),
            AutoSize = true
        };

        txEmail = new TextBox
        {
            Location = new Point(40, 188),
            Width = 300,
            Height = 36,
            BackColor = Color.FromArgb(32, 32, 46),
            ForeColor = TextLight,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 11f),
            PlaceholderText = "usuario@coffecore.com"
        };

        var lblPass = new Label
        {
            Text = "Contrasena",
            ForeColor = TextMuted,
            Font = new Font("Segoe UI", 8.5f),
            Location = new Point(40, 236),
            AutoSize = true
        };

        txPass = new TextBox
        {
            Location = new Point(40, 254),
            Width = 300,
            Height = 36,
            BackColor = Color.FromArgb(32, 32, 46),
            ForeColor = TextLight,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 11f),
            PasswordChar = '●',
            PlaceholderText = "••••••••"
        };
        txPass.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) DoLogin(); };

        lblError = new Label
        {
            Text = "",
            ForeColor = Color.FromArgb(220, 80, 80),
            Font = new Font("Segoe UI", 9f),
            Location = new Point(40, 300),
            Width = 300,
            Height = 20
        };

        btnLogin = new Button
        {
            Text = "INGRESAR",
            Location = new Point(40, 326),
            Width = 300,
            Height = 44,
            BackColor = Accent,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.Click += (s, e) => DoLogin();

        var lblVersion = new Label
        {
            Text = "v2.0 — Coffee ERP 2026",
            ForeColor = TextMuted,
            Font = new Font("Segoe UI", 8f),
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(40, 390),
            Width = 300,
            Height = 20
        };

        card.Controls.AddRange([
            topBar, lblLogo, lblTitle, lblSub,
            lblEmail, txEmail,
            lblPass, txPass,
            lblError, btnLogin, lblVersion
        ]);

        Controls.Add(card);
    }

    void DoLogin()
    {
        if (string.IsNullOrWhiteSpace(txEmail.Text) || string.IsNullOrWhiteSpace(txPass.Text))
        {
            lblError.Text = "Complete todos los campos.";
            return;
        }

        btnLogin.Text = "Verificando...";
        btnLogin.Enabled = false;
        lblError.Text = "";

        Task.Run(() =>
        {
            try
            {
                using var conn = DBConnection.GetConnection();
                using var cmd = new SqlCommand(@"
                    SELECT nombre, rol, id_sucursal
                    FROM Usuarios
                    WHERE email=@e AND password_hash=@p AND activo=1", conn);
                cmd.Parameters.AddWithValue("@e", txEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@p", txPass.Text);
                using var r = cmd.ExecuteReader();

                Invoke(() =>
                {
                    if (r.Read())
                    {
                        CurrentUser     = r["nombre"].ToString()!;
                        CurrentRole     = r["rol"].ToString()!;
                        CurrentSucursal = r["id_sucursal"] != DBNull.Value
                            ? Convert.ToInt32(r["id_sucursal"]) : 0;

                        var main = new MainForm();
                        main.FormClosed += (s, e) =>
                        {
                            // Al cerrar MainForm volvemos al login
                            txEmail.Text = "";
                            txPass.Text  = "";
                            lblError.Text = "";
                            btnLogin.Text = "INGRESAR";
                            btnLogin.Enabled = true;
                            Show();
                        };
                        Hide();
                        main.Show();
                    }
                    else
                    {
                        lblError.Text = "Email o contrasena incorrectos.";
                        btnLogin.Text = "INGRESAR";
                        btnLogin.Enabled = true;
                    }
                });
            }
            catch
            {
                Invoke(() =>
                {
                    lblError.Text = "Error de conexion. Verifica los datos.";
                    btnLogin.Text = "INGRESAR";
                    btnLogin.Enabled = true;
                });
            }
        });
    }
}
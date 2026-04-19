using Microsoft.Data.SqlClient;

namespace CoffeeERP.Database;

public static class DBConnection
{
    public static string Server = "ZZZ";
    public static string Database = "ERPCOFFECORE";
    public static string User = "";
    public static string Password = "";

    public static string GetConnectionString() =>
        $"Server={Server};Database={Database};" +
        $"Trusted_Connection=True;" +
        $"TrustServerCertificate=True;Connection Timeout=30;";

    public static SqlConnection GetConnection()
    {
        var conn = new SqlConnection(GetConnectionString());
        conn.Open();
        return conn;
    }

    public static bool TestConnection()
    {
        try { using var c = GetConnection(); return true; }
        catch { return false; }
    }

    public static void RegistrarAuditoria(string modulo, string accion)
    {
        try
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(@"
                INSERT INTO Auditoria(usuario, rol, modulo, accion, descripcion, id_sucursal)
                VALUES(@u, @r, @m, @a, @d, @s)", conn);

            cmd.Parameters.AddWithValue("@u", Forms.LoginForm.CurrentUser ?? "Sistema");
            cmd.Parameters.AddWithValue("@r", Forms.LoginForm.CurrentRole ?? "Sistema");
            cmd.Parameters.AddWithValue("@m", modulo);
            cmd.Parameters.AddWithValue("@a", accion);
            // descripcion = el mismo texto de accion (más detallado en el futuro si quieres)
            cmd.Parameters.AddWithValue("@d", accion);
            // id_sucursal: NULL si no hay sesión activa, o el ID de la sucursal actual
            cmd.Parameters.AddWithValue("@s",
                Forms.LoginForm.CurrentSucursal > 0
                    ? (object)Forms.LoginForm.CurrentSucursal
                    : DBNull.Value);

            cmd.ExecuteNonQuery();
        }
        catch { }
    }
}
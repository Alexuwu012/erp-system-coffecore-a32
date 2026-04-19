using CoffeeERP.Forms;
using CoffeeERP.Database;

namespace CoffeeERP;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Carga config.json antes de abrir el login
        ConfiguracionControl.CargarConfiguracion();

        Application.Run(new LoginForm());
    }
}
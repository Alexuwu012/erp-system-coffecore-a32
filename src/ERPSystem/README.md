# ⚙️ ERP Coffee Core — Backend

> Código fuente del backend del sistema ERP Coffee Core, desarrollado en C# con ASP.NET Core.

---

## 🛠️ Tecnologías

| Capa | Tecnología |
|------|------------|
| Lenguaje | C# (.NET 8) |
| Framework Web | ASP.NET Core MVC |
| ORM | Entity Framework Core |
| Base de Datos | SQL Server |
| Autenticación | ASP.NET Identity |
| Frontend | Razor Views + Bootstrap |
| Pruebas | xUnit |

---

## 🏗️ Arquitectura del Sistema

El sistema sigue una **arquitectura en capas** que separa las responsabilidades en tres niveles:

```text
┌─────────────────────────────────┐
│      Capa de Presentación       │  ← Razor Views / Interfaz Web
├─────────────────────────────────┤
│    Capa de Lógica de Negocio    │  ← Controllers + Services
├─────────────────────────────────┤
│        Capa de Datos            │  ← Entity Framework Core + SQL Server
└─────────────────────────────────┘
```

El cliente accede al sistema mediante una interfaz web local. El servidor de aplicación ejecuta la lógica de negocio y la base de datos almacena la información empresarial.

---

## 📁 Estructura del Proyecto

```text
src/ERPSystem/
├── Controllers/        # Controladores de cada módulo
├── Models/             # Entidades y modelos de datos
├── Views/              # Vistas Razor por módulo
├── Services/           # Lógica de negocio por módulo
├── Data/               # DbContext y configuración de EF Core
├── Migrations/         # Migraciones de base de datos
├── wwwroot/            # Archivos estáticos (CSS, JS, imágenes)
└── Program.cs          # Punto de entrada de la aplicación
```

---

## 🧩 Módulos del Sistema

| Módulo | Descripción | Estado |
|--------|-------------|--------|
| **EP-ADM** | Administración de sucursales, usuarios y roles | ✅ Sprint 1 |
| **EP-CRM** | Gestión de clientes y trazabilidad de compras | ✅ Sprint 1 |
| **EP-INV** | Control de inventario, productos y stock | ✅ Sprint 1 |
| **EP-PROV** | Gestión de proveedores y condiciones de pago | ✅ Sprint 1 |
| **EP-VEN** | Registro de ventas y transacciones por sucursal | ✅ Sprint 1 |
| **EP-RRHH** | Registro de empleados, roles y salarios | ✅ Sprint 1 |
| **EP-FIN** | Ingresos, egresos y movimientos financieros | ✅ Sprint 1 |
| **EP-COMP** | Órdenes de compra y recepción de productos | ✅ Sprint 1 |

---

## 🚀 Estado de Desarrollo

| Sprint | Enfoque | Estado |
|--------|---------|--------|
| Sprint 0 | Planeación y arquitectura inicial | ✅ Completado |
| Sprint 1 | Modelo de base de datos y operaciones CRUD | ✅ Completado |
| Sprint 2 | Vistas, lógica de negocio e integración entre módulos | 🔄 En progreso |
| Sprint 3 | Diseño responsivo, pruebas unitarias y producción | ⏳ Pendiente |

---

## ⚙️ Configuración del Proyecto

### Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) o SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/)

### Pasos para ejecutar

1. Clona el repositorio:

```bash
git clone https://github.com/Alexuwu012/erp-system-coffecore-a32.git
cd erp-system-coffecore-a32/src/ERPSystem
```

2. Configura la cadena de conexión en `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=ERPCoffeeCore;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

3. Aplica las migraciones de base de datos:

```bash
dotnet ef database update
```

4. Ejecuta el proyecto desde Visual Studio presionando **F5** o desde la terminal:

```bash
dotnet run
```

5. El sistema se ejecutará en la **máquina local** y podrá abrirse desde el navegador configurado en el entorno de desarrollo.

---

## 🗂️ Convenciones de Código

- **Idioma del código:** Inglés para variables, métodos y clases
- **Idioma de comentarios:** Español
- **Documentación interna:** Comentarios XML en C# (`/// <summary>`)
- **Nomenclatura de tablas:** PascalCase en español, por ejemplo `Cliente`, `Producto`
- **Claves primarias:** `Id_[NombreTabla]`
- **Claves foráneas:** `Id_[TablaReferenciada]`

---

## 📄 Relación con la Documentación

Este backend forma parte del proyecto académico **ERP Coffee Core** y su diseño está alineado con la documentación arquitectónica elaborada en **arc42**, disponible en la carpeta `documentación/`.

---

> Proyecto académico — ERP Coffee Core A32

# ☕ CoffeeERP — Sistema ERP para Cafeterías

Sistema de gestión empresarial (ERP) desarrollado en **C# con Windows Forms**, orientado a la administración integral de una cadena de cafeterías. Proyecto desarrollado como parte del curso de **Arquitectura de Software** en la Universidad Manuela Beltrán (UMB).

---

## 🏗️ Tecnologías utilizadas

| Tecnología | Uso |
|---|---|
| C# / .NET | Lenguaje principal |
| Windows Forms | Interfaz gráfica de usuario |
| SQL Server | Base de datos relacional |
| Visual Studio 2026 | IDE de desarrollo |
| PlantUML | Diagramas de arquitectura |
| arc42 | Plantilla de documentación |

---

## 📁 Estructura del proyecto

```
CoffeeERP/
├── Database/
│   └── DBConnection.cs         # Conexión y queries a la base de datos
├── Forms/
│   ├── LoginForm.cs            # Autenticación de usuarios
│   ├── MainForm.cs             # Ventana principal / navegación
│   ├── DashboardControl.cs     # Panel de control general
│   ├── AuditoriaControl.cs     # Módulo de auditoría del sistema
│   ├── CategoriasControl.cs    # Gestión de categorías de productos
│   ├── ClientesControl.cs      # Módulo CRM - Gestión de clientes
│   ├── ComprasControl.cs       # Gestión de compras
│   ├── ConfiguracionControl.cs # Configuración del sistema
│   ├── EmpleadosControl.cs     # Módulo RRHH - Gestión de empleados
│   ├── FinanzasControl.cs      # Módulo de finanzas y reportes financieros
│   ├── InsumosControl.cs       # Gestión de insumos
│   ├── InventarioControl.cs    # Control de inventario
│   ├── ProductosControl.cs     # Gestión de productos
│   ├── ProveedoresControl.cs   # Gestión de proveedores
│   ├── RecetasControl.cs       # Gestión de recetas
│   ├── ReportesControl.cs      # Reportes y estadísticas
│   ├── SucursalesControl.cs    # Gestión de sucursales
│   ├── TurnosControl.cs        # Gestión de turnos
│   ├── VentasControl.cs        # Módulo de ventas
│   └── BaseGridControl.cs      # Componente base reutilizable
├── Program.cs                  # Punto de entrada
└── CoffeeERP.csproj            # Configuración del proyecto
```

---

## 🧩 Módulos del sistema

| Módulo | Descripción |
|---|---|
| 🔐 Login | Autenticación y control de acceso por roles |
| 📊 Dashboard | Panel principal con métricas e indicadores clave |
| 👥 Clientes (CRM) | Registro, consulta y gestión de clientes |
| 📦 Inventario | Control de stock, productos e insumos |
| 🛒 Compras | Gestión de órdenes de compra a proveedores |
| 💰 Ventas | Registro de transacciones y punto de venta |
| 👨‍💼 Empleados (RRHH) | Gestión de personal y turnos |
| 🏭 Proveedores | Administración de proveedores y abastecimiento |
| 📈 Finanzas | Reportes financieros y contabilidad básica |
| 🏪 Sucursales | Gestión multi-sede |
| 🍵 Recetas | Fichas técnicas de productos del menú |
| ⚙️ Configuración | Parámetros generales del sistema |
| 🔍 Auditoría | Registro y limpieza del log de actividades |

---

## 🚀 Instalación y ejecución

### Requisitos previos
- Visual Studio 2022 o superior
- .NET Framework 4.8 o .NET 6+
- SQL Server 2019 o superior

### Pasos

1. Clonar el repositorio:
```bash
git clone https://github.com/Alexuwu012/erp-system-coffecore-a32.git
cd erp-system-coffecore-a32
```

2. Abrir `CoffeeERP.sln` en Visual Studio

3. Configurar la cadena de conexión en `Database/DBConnection.cs`:
```csharp
string connectionString = "Server=TU_SERVIDOR;Database=CoffeeERP;Trusted_Connection=True;";
```

4. Ejecutar los scripts SQL de la carpeta `Database/` para crear las tablas

5. Compilar y ejecutar con `F5`

---

## 📐 Arquitectura

El proyecto sigue una arquitectura **N-Capas** con separación entre interfaz, lógica de negocio y acceso a datos. La documentación completa de arquitectura sigue la plantilla **arc42** y está disponible en la carpeta `documentacion/`.

Diagramas disponibles en PlantUML:
- Diagrama C4 por módulo
- Diagramas de secuencia por caso de uso
- Diagrama de clases
- Diagrama de despliegue
- Diagrama de base de datos

---

## 👥 Equipo de desarrollo

Proyecto desarrollado por estudiantes de Ingeniería de Sistemas — Universidad Manuela Beltrán (UMB), 2026.

---

## 📄 Licencia

Proyecto académico — Universidad Manuela Beltrán. Todos los derechos reservados.

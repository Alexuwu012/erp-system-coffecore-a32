# ☕ ERP Coffee Core — A32

<img width="400" alt="CoffeeERP Logo" src="https://github.com/user-attachments/assets/f8cc271f-f275-4f4b-a493-3e0e03e91043" />

> Sistema ERP académico desarrollado con metodología ágil Scrum, enfocado en la gestión integral de procesos empresariales para cadenas de cafeterías.

![GitHub branches](https://img.shields.io/badge/branches-2-blue)
![GitHub commits](https://img.shields.io/badge/commits-35-green)
![Sprint](https://img.shields.io/badge/sprint-2-orange)
![Status](https://img.shields.io/badge/status-en%20desarrollo-yellow)
![Universidad](https://img.shields.io/badge/UMB-Arquitectura%20de%20Software-brown)

---

## 📋 Descripción

**ERP Coffee Core** es un sistema de Planificación de Recursos Empresariales desarrollado como proyecto académico en la **Universidad Manuela Beltrán (UMB)**. Su objetivo es centralizar y automatizar los procesos de una cadena de cafeterías mediante módulos independientes e integrados entre sí.

El sistema está orientado a **pequeñas y medianas empresas** que requieren control de inventario, ventas, compras, finanzas y recursos humanos en una sola plataforma.

---

## 🏗️ Estructura del Repositorio

```text
erp-system-coffecore-a32/
├── database/
│   └── scripts/              # Scripts SQL de creación de tablas por módulo
├── documentación/            # Documentación del proyecto (arc42)
├── sprint/                   # Entregables y evidencias por sprint
├── src/
│   └── CoffeeERP/            # Código fuente principal (C# / Windows Forms)
│       ├── Database/
│       │   └── DBConnection.cs
│       ├── Forms/
│       │   ├── LoginForm.cs
│       │   ├── MainForm.cs
│       │   ├── DashboardControl.cs
│       │   ├── AuditoriaControl.cs
│       │   ├── ClientesControl.cs
│       │   ├── EmpleadosControl.cs
│       │   ├── InventarioControl.cs
│       │   ├── VentasControl.cs
│       │   ├── ComprasControl.cs
│       │   ├── FinanzasControl.cs
│       │   ├── ProveedoresControl.cs
│       │   ├── ReportesControl.cs
│       │   └── ...
│       └── Program.cs
├── .gitignore
└── README.md
```

---

## 🧩 Módulos del Sistema

| Módulo | Descripción |
|--------|-------------|
| 🔐 **Login** | Autenticación y control de acceso por roles |
| 📊 **Dashboard** | Panel principal con métricas e indicadores clave |
| 🏢 **EP-ADM** | Administración de sucursales, usuarios y roles |
| 👥 **EP-CRM** | Gestión de clientes y trazabilidad de compras |
| 🏭 **EP-PROV** | Gestión de proveedores y condiciones de pago |
| 👨‍💼 **EP-RRHH** | Registro de empleados, roles, salarios y horarios |
| 📦 **EP-INV** | Control de inventario, productos y movimientos de stock |
| 🛒 **EP-COMP** | Órdenes de compra y recepción de productos |
| 💰 **EP-VEN** | Registro de ventas y transacciones por sucursal |
| 📈 **EP-FIN** | Ingresos, egresos y movimientos financieros |
| 🍵 **Recetas** | Fichas técnicas de productos del menú |
| 🔍 **Auditoría** | Registro y limpieza del log de actividades del sistema |

---

## ⚙️ Tecnologías Utilizadas

| Capa | Tecnología |
|------|------------|
| Lenguaje | C# / .NET |
| Interfaz | Windows Forms |
| Base de Datos | SQL Server |
| IDE | Visual Studio 2022+ |
| Control de Versiones | Git + GitHub |
| Documentación | arc42 + PlantUML |
| Metodología | Scrum |

---

## 🚀 Sprints

### Sprint 0 — Planeación del Proyecto
- Definición de visión y alcance del sistema
- Identificación de personas, roles y stakeholders
- Levantamiento de requerimientos y mapeo de procesos
- Planificación de releases y generación del backlog inicial

### Sprint 1 — Base de Datos
- Implementación de la estructura de base de datos normalizada
- Creación de tablas por módulo con claves primarias y foráneas
- Operaciones CRUD básicas para cada módulo
- Triggers automáticos para control de inventario e integridad financiera

### Sprint 2 — Interfaces y Lógica de Negocio *(en progreso)*
- Desarrollo de formularios por módulo en Windows Forms (C#)
- Implementación de lógica de negocio
- Integración entre módulos mediante `DBConnection.cs`
- Validaciones y manejo de errores

---

## 💻 Instalación y Ejecución

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

2. Abrir `src/CoffeeERP/CoffeeERP.sln` en Visual Studio

3. Configurar la cadena de conexión en `Database/DBConnection.cs`

4. Ejecutar los scripts SQL de la carpeta `database/scripts/`

5. Compilar y ejecutar con `F5`

---

## 🗂️ Gestión del Proyecto

- Metodología: **Scrum**
- Ramas: `main` (producción) y `develop` (desarrollo)
- Integración de cambios mediante **Pull Requests**
- Documentación de arquitectura: **arc42**

---

## 📐 Arquitectura

El proyecto sigue una arquitectura **N-Capas** con separación entre interfaz, lógica de negocio y acceso a datos. La documentación completa sigue la plantilla **arc42** y está en `/documentación`.

Diagramas disponibles en PlantUML:
- Diagrama C4 por módulo
- Diagramas de secuencia por caso de uso
- Diagrama de clases por módulo
- Diagrama de despliegue
- Diagrama de base de datos

---

## 👥 Roles del Equipo

| Rol | Responsabilidad |
|-----|----------------|
| Desarrollador Full Stack | Implementación de módulos, formularios y lógica de negocio |
| Diseñador de Base de Datos | Modelo relacional, scripts SQL y triggers |
| Documentador arc42 | Mantenimiento de la documentación de arquitectura |
| Scrum Master | Coordinación de sprints y seguimiento del backlog |

---

## 📄 Documentación

La documentación completa de arquitectura se encuentra en [`/documentación`](./documentación/) siguiendo la plantilla **arc42**.

---

> Proyecto académico — Universidad Manuela Beltrán © 2026 ERP Coffee Core A32

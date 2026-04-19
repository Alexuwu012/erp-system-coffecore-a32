# Sistema ERP - Proyecto Académico - COFFE CORE - A32
<img width="1600" height="1600" alt="image" src="https://github.com/user-attachments/assets/f8cc271f-f275-4f4b-a493-3e0e03e91043" />

> Sistema ERP académico desarrollado con metodología ágil Scrum, enfocado en la gestión integral de procesos empresariales.

![GitHub branches](https://img.shields.io/badge/branches-2-blue)
![GitHub commits](https://img.shields.io/badge/commits-35-green)
![Sprint](https://img.shields.io/badge/sprint-2-orange)
![Status](https://img.shields.io/badge/status-en%20desarrollo-yellow)

---

## 📋 Descripción

**ERP Coffee Core** es un sistema de Planificación de Recursos Empresariales desarrollado como proyecto académico. Su objetivo es centralizar y automatizar los procesos de una organización mediante módulos independientes e integrados entre sí.

El sistema está orientado a **pequeñas y medianas empresas** que requieren control de inventario, ventas, compras, finanzas y recursos humanos en una sola plataforma.

---

## 🏗️ Estructura del Repositorio

```text
erp-system-coffecore-a32/
├── database/
│   └── scripts/          # Scripts SQL de creación de tablas por módulo
├── documentación/        # Documentación del proyecto (arc42)
├── sprint/               # Entregables y evidencias por sprint
├── src/
│   └── ERPSystem/        # Código fuente principal de la aplicación
├── .gitignore
└── README.md
```

---

## 🧩 Módulos del Sistema

| Módulo | Descripción |
|--------|-------------|
| **EP-ADM** | Administración de sucursales, usuarios y roles |
| **EP-CRM** | Gestión de clientes y trazabilidad de compras |
| **EP-PROV** | Gestión de proveedores y condiciones de pago |
| **EP-RRHH** | Registro de empleados, roles, salarios y horarios |
| **EP-INV** | Control de inventario, productos y movimientos de stock |
| **EP-COMP** | Órdenes de compra y recepción de productos |
| **EP-VEN** | Registro de ventas y transacciones por sucursal |
| **EP-FIN** | Ingresos, egresos y movimientos financieros |

---

## 🚀 Sprints

### Sprint 0 — Planeación del Proyecto

- Definición de visión y alcance del sistema
- Identificación de personas y roles (Analista de Compras, Gerente de Ventas, Contador, etc.)
- Levantamiento de requerimientos y mapeo de procesos
- Planificación de releases y generación del backlog inicial

### Sprint 1 — Base de Datos

- Implementación de la estructura de base de datos normalizada
- Creación de tablas por módulo con claves primarias y foráneas
- Operaciones CRUD básicas para cada módulo
- Triggers automáticos para control de inventario e integridad financiera

### Sprint 2 — Interfaces y Lógica de Negocio *(en progreso)*

- Desarrollo de vistas por módulo
- Implementación de lógica de negocio en capa de servicios
- Integración entre módulos
- Validaciones en capa de aplicación

---

## ⚙️ Tecnologías Utilizadas

| Capa | Tecnología |
|------|------------|
| Backend | ASP.NET Core (C#) |
| ORM | Entity Framework Core |
| Base de Datos | SQL Server |
| Autenticación | ASP.NET Identity |
| Frontend | Razor Views + Bootstrap |
| Control de Versiones | Git + GitHub |

---

## 🗂️ Gestión del Proyecto

- Metodología: **Scrum**
- Control de versiones: **GitHub** con ramas `main` y `develop`
- Integración de cambios mediante **Pull Requests**
- Documentación de arquitectura: **arc42**

---

## 👥 Roles del Equipo

| Rol | Responsabilidad |
|-----|----------------|
| Desarrollador Full Stack | Implementación de módulos, vistas y lógica de negocio |
| Diseñador de Base de Datos | Modelo relacional, scripts SQL y migraciones |
| Documentador arc42 | Mantenimiento de la documentación de arquitectura |
| Scrum Master | Coordinación de sprints y seguimiento del backlog |

---

## 📄 Documentación

La documentación completa de arquitectura del sistema se encuentra en la carpeta [`/documentación`](./documentación/) siguiendo la plantilla **arc42**.

---

## 📌 Estado del Proyecto

Actualmente el proyecto cuenta con:

- Estructura inicial del repositorio definida
- Base de datos modelada por módulos
- Scripts SQL organizados en la carpeta `database/scripts`
- Documentación del proyecto en la carpeta `documentación`
- Evidencias y avances de sprint en la carpeta `sprint`
- Código fuente principal en `src/ERPSystem`

---

## 🔗 Repositorio

Puedes acceder al repositorio del proyecto aquí:

[ERP Coffee Core - GitHub](https://github.com/Alexuwu012/erp-system-coffecore-a32)

---

> Proyecto académico — Todos los derechos reservados © 2025 ERP Coffee Core A32

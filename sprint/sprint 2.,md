# 🏃 Sprint 2 — Interfaces y Lógica de Negocio

## 🎯 Sprint Goal

Implementar las vistas de usuario y la lógica de negocio de cada módulo del sistema
ERP Coffee Core, asegurando la integración entre módulos y el correcto funcionamiento
de las operaciones del sistema.

---

## 📅 Información General

| Campo | Detalle |
|-------|---------|
| **Sprint** | 2 |
| **Estado** | 🔄 En progreso |
| **Enfoque** | Vistas + Lógica de negocio + Integración |
| **Rama** | `develop` |

---

## ✅ Definition of Done

Una tarea se considera terminada cuando:

- Las vistas del módulo están implementadas (listar, crear, editar, eliminar)
- La lógica de negocio está implementada en la capa de servicios
- Los datos del formulario están validados en la capa de aplicación
- La integración con otros módulos ha sido probada manualmente sin errores críticos
- El código está almacenado en la rama `develop` de GitHub
- La documentación arc42 ha sido actualizada

---

## 🧩 Módulos Implementados

| Módulo | Vistas | Lógica de Negocio | Integración | Estado |
|--------|--------|-------------------|-------------|--------|
| EP-ADM | ✅ | ✅ Autenticación y roles | EP-RRHH | ✅ |
| EP-CRM | ✅ | ✅ Gestión de clientes | EP-VEN | ✅ |
| EP-PROV | ✅ | ✅ Gestión de proveedores | EP-INV | ✅ |
| EP-INV | ✅ | ✅ Control de stock | EP-VEN, EP-COMP | ✅ |
| EP-COMP | ✅ | ✅ Órdenes de compra | EP-INV, EP-PROV | ✅ |
| EP-VEN | ✅ | ✅ Registro de ventas | EP-INV, EP-CRM | ✅ |
| EP-RRHH | ✅ | ✅ Gestión de empleados | EP-ADM | ✅ |
| EP-FIN | ✅ | ✅ Reportes financieros | EP-VEN, EP-COMP | ✅ |

---

## 🔧 Funcionalidades Implementadas

- **Autenticación y control de roles** mediante ASP.NET Identity
- **Actualización automática de inventario** al registrar ventas y compras
- **Cálculo de totales y subtotales** en el módulo de ventas
- **Generación de reportes de ingresos y egresos** en el módulo financiero
- **Asignación de roles a empleados** desde el módulo de administración

---

## 📁 Entregables

- Vistas Razor para todos los módulos en `src/ERPSystem/Views/`
- Servicios de negocio en `src/ERPSystem/Services/`
- Documentación arc42 actualizada en `documentación/arc42/`
- Evidencias del sprint en `sprint/sprint2/`

---

## 📌 Deuda Técnica Detectada

- Vistas sin diseño responsivo en EP-CRM, EP-VEN e EP-INV
- Sin paginación en listados con múltiples registros
- Lógica de negocio mezclada con la capa de presentación en EP-VEN y EP-FIN
- Sin mensajes de confirmación al eliminar registros
- Sin estrategia de backup de base de datos

---

## 🔁 Próximo Sprint

**Sprint 3** — Corrección de deuda técnica, diseño responsivo, pruebas unitarias
y configuración del entorno de producción.

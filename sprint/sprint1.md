# 🏃 Sprint 1 — Base de Datos y Arquitectura

## 🎯 Sprint Goal

Desarrollar la estructura inicial de la base de datos del sistema ERP Coffee Core,
estableciendo el modelo relacional completo y documentando la arquitectura base en arc42.

---

## 📅 Información General

| Campo | Detalle |
|-------|---------|
| **Sprint** | 1 |
| **Estado** | ✅ Completado |
| **Enfoque** | Base de datos + Arquitectura arc42 |
| **Rama** | `develop` |

---

## ✅ Definition of Done

Una tarea se considera terminada cuando:

- Las tablas del módulo están creadas con claves primarias y foráneas definidas
- Las operaciones CRUD básicas están implementadas
- El script SQL está almacenado en `database/scripts/` en la rama `develop`
- La estructura del módulo está documentada en arc42

---

## 🧩 Módulos Implementados

| Módulo | Tablas Creadas | CRUD | Triggers |
|--------|---------------|------|---------|
| EP-ADM | Sucursales, Usuarios, Roles | ✅ | ❌ |
| EP-CRM | Clientes | ✅ | ❌ |
| EP-PROV | Proveedores | ✅ | ❌ |
| EP-INV | Productos, Inventario | ✅ | ✅ |
| EP-COMP | OrdenCompra, DetalleCompra | ✅ | ✅ |
| EP-VEN | Ventas, DetalleVenta | ✅ | ✅ |
| EP-RRHH | Empleados | ✅ | ❌ |
| EP-FIN | Finanzas, Movimientos | ✅ | ✅ |

---

## ⚙️ Triggers Implementados

- **TR_ActualizarInventario** — Actualiza automáticamente el stock al registrar
  una compra o una venta
- **TR_RegistrarMovimientoFinanciero** — Genera automáticamente un movimiento
  financiero asociado a cada transacción de venta o compra
- **TR_ValidarStock** — Valida que exista stock suficiente antes de confirmar una venta

---

## 📁 Entregables

- Scripts SQL organizados en `database/scripts/` por módulo
- Documentación de arquitectura actualizada en `documentación/arc42/`
- Evidencias del sprint en `sprint/sprint1/`

---

## 📌 Deuda Técnica Detectada

- Validaciones solo en base de datos, pendientes en capa de lógica
- Sin manejo de excepciones en operaciones CRUD
- Código sin comentarios XML en C#
- Sin pruebas unitarias automatizadas

---

## 🔁 Próximo Sprint

**Sprint 2** — Desarrollo de interfaces de usuario e implementación de lógica
de negocio para cada módulo.

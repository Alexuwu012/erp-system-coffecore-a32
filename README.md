# Sistema ERP - Proyecto Académico - COFFE CORE - A32
<img width="1600" height="1600" alt="image" src="https://github.com/user-attachments/assets/f8cc271f-f275-4f4b-a493-3e0e03e91043" />

Este repositorio contiene el desarrollo del modelo de base de datos para un sistema ERP (Enterprise Resource Planning), enfocado en la gestión integral de los procesos de una empresa. El proyecto se llevó a cabo como parte de un ejercicio académico, aplicando metodologías ágiles (Scrum) y buenas prácticas de desarrollo de software.

---

## Sprint 0 – Planeación del Proyecto

El Sprint 0 tuvo como objetivo definir la visión, alcance y planificación inicial del sistema ERP antes de comenzar el desarrollo técnico. Durante esta fase se realizaron las siguientes actividades:

### 1. Definición de la Visión y Alcance
- Se identificó el problema principal: la necesidad de un sistema centralizado para la gestión de procesos empresariales.
- Se definió el usuario objetivo: pequeñas y medianas empresas que requieren control de inventario, ventas, compras, finanzas y recursos humanos.
- Plantilla de visión utilizada:  
  > "Para [usuario] que [necesidad], nuestro sistema ERP [valor] a diferencia de [alternativas], [diferenciador]".

### 2. Identificación de Personas y Roles
- Analista de Compras
- Empleado de Almacén
- Gerente de Ventas
- Contador
- Administrador del sistema

### 3. Levantamiento de Requerimientos Iniciales
- Mapeo de los procesos principales de la empresa.
- Identificación de datos críticos y relaciones entre módulos.
- Priorización de funcionalidades para el primer incremento del sistema.

### 4. Planificación de Releases y Sprints
- Sprint 1: Estructura de la base de datos y triggers iniciales.
- Sprint 2: Integración de interfaces y pruebas funcionales.

### 5. Documentación de Trazabilidad
- Generación del backlog inicial de historias de usuario.
- Establecimiento de tablero Scrum para gestión de tareas y seguimiento de avances.

---

## Sprint 1 – Desarrollo de la Base de Datos

Durante el Sprint 1 se implementó la estructura inicial de la base de datos del ERP, enfocándose en garantizar una correcta normalización y relaciones entre los módulos.

### 1. Administración
- **Sucursales**: Tabla para almacenar información de cada sucursal de la empresa, incluyendo dirección, teléfono y gerente responsable.
- **Usuarios**: Tabla de usuarios con roles y permisos, vinculados a las sucursales correspondientes.

### 2. Clientes (CRM)
- Registro de clientes con datos de contacto, historial de compras y segmentación.
- Implementación de relaciones con módulos de ventas para trazabilidad de transacciones.

### 3. Proveedores
- Tabla de proveedores con información de contacto y condiciones de pago.
- Relación con el módulo de compras para controlar órdenes y pagos.

### 4. Recursos Humanos
- Registro de empleados, roles, salarios y horarios.
- Integración con módulos de finanzas y administración para cálculo de nómina y beneficios.

### 5. Inventario y Productos
- Tablas de productos con información detallada (código, nombre, categoría, precio, stock).
- Registro de movimientos de inventario y control de existencias.

### 6. Compras
- Tablas para órdenes de compra, recepción de productos y control de pagos a proveedores.
- Relación directa con el inventario para actualización automática de stock.

### 7. Ventas (POS)
- Registro de ventas y transacciones por sucursal.
- Integración con el módulo de clientes y el inventario para control en tiempo real.

### 8. Finanzas
- Registro de ingresos, egresos y movimientos financieros.
- Triggers automáticos para actualizar balances y detectar inconsistencias.

### 9. Triggers y Automatizaciones
- Control automático de inventario al realizar compras o ventas.
- Generación automática de movimientos financieros relacionados a cada transacción.
- Validaciones para mantener integridad referencial y consistencia de datos.

---

## Gestión del Proyecto
- Se utilizó **GitHub** como plataforma de control de versiones.
- Los cambios se integraron mediante **Pull Requests** y revisión de código.
- Se documentaron los avances y decisiones técnicas para facilitar la trazabilidad.

---

Este README sirve como referencia del estado actual del proyecto y documentación para futuros desarrollos e integraciones de los siguientes sprints.

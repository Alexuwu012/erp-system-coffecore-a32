/* CLIENTES (CRM) */

CREATE TABLE Clientes (
    id_cliente INT IDENTITY PRIMARY KEY,
    nombre VARCHAR(150),
    telefono VARCHAR(20),
    email VARCHAR(150),
    direccion VARCHAR(200),
    fecha_registro DATETIME DEFAULT GETDATE()
);

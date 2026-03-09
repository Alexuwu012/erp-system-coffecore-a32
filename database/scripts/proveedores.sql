/* PROVEEDORES */

CREATE TABLE Proveedores (
    id_proveedor INT IDENTITY PRIMARY KEY,
    nombre VARCHAR(150),
    telefono VARCHAR(20),
    email VARCHAR(150),
    direccion VARCHAR(200)
);

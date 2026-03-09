
/* PRODUCTOS (INVENTARIO) */

CREATE TABLE Productos (
    id_producto INT IDENTITY PRIMARY KEY,
    nombre VARCHAR(150),
    descripcion VARCHAR(200),
    precio DECIMAL(10,2),
    costo DECIMAL(10,2)
);

/* INVENTARIO POR SUCURSAL */

CREATE TABLE Inventario (
    id_inventario INT IDENTITY PRIMARY KEY,
    id_producto INT,
    id_sucursal INT,
    cantidad INT,
    FOREIGN KEY (id_producto) REFERENCES Productos(id_producto),
    FOREIGN KEY (id_sucursal) REFERENCES Sucursales(id_sucursal)
);

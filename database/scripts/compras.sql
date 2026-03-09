/* COMPRAS A PROVEEDORES */

CREATE TABLE Compras (
    id_compra INT IDENTITY PRIMARY KEY,
    id_proveedor INT,
    id_sucursal INT,
    fecha DATETIME DEFAULT GETDATE(),
    total DECIMAL(10,2),
    FOREIGN KEY (id_proveedor) REFERENCES Proveedores(id_proveedor),
    FOREIGN KEY (id_sucursal) REFERENCES Sucursales(id_sucursal)
);

CREATE TABLE ComprasDetalle (
    id_detalle INT IDENTITY PRIMARY KEY,
    id_compra INT,
    id_producto INT,
    cantidad INT,
    precio DECIMAL(10,2),
    FOREIGN KEY (id_compra) REFERENCES Compras(id_compra),
    FOREIGN KEY (id_producto) REFERENCES Productos(id_producto)
);

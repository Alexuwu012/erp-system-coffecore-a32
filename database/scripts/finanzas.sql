
/* FINANZAS */

CREATE TABLE MovimientosFinancieros (
    id_movimiento INT IDENTITY PRIMARY KEY,
    tipo VARCHAR(50),
    descripcion VARCHAR(200),
    monto DECIMAL(10,2),
    fecha DATETIME DEFAULT GETDATE(),
    id_sucursal INT,
    FOREIGN KEY (id_sucursal) REFERENCES Sucursales(id_sucursal)
);

/* EMPLEADOS (RRHH) */

CREATE TABLE Empleados (
    id_empleado INT IDENTITY PRIMARY KEY,
    nombre VARCHAR(150),
    cargo VARCHAR(100),
    salario DECIMAL(10,2),
    fecha_contrato DATE,
    id_sucursal INT,
    FOREIGN KEY (id_sucursal) REFERENCES Sucursales(id_sucursal)
);

/* SUCURSALES */

CREATE TABLE Sucursales (
    id_sucursal INT IDENTITY PRIMARY KEY,
    nombre VARCHAR(100),
    direccion VARCHAR(200),
    ciudad VARCHAR(100),
    telefono VARCHAR(20)
);

/* USUARIOS (ADMIN) */

CREATE TABLE Usuarios (
    id_usuario INT IDENTITY PRIMARY KEY,
    nombre VARCHAR(100),
    email VARCHAR(150),
    password_hash VARCHAR(255),
    rol VARCHAR(50),
    id_sucursal INT,
    FOREIGN KEY (id_sucursal) REFERENCES Sucursales(id_sucursal)
);

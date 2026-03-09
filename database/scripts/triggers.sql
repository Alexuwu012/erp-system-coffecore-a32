/* RESTAR INVENTARIO AL VENDER */

CREATE TRIGGER trg_restar_inventario
ON VentasDetalle
AFTER INSERT
AS
BEGIN
UPDATE inv
SET inv.cantidad = inv.cantidad - i.cantidad
FROM Inventario inv
JOIN inserted i ON inv.id_producto = i.id_producto
JOIN Ventas v ON v.id_venta = i.id_venta
WHERE inv.id_sucursal = v.id_sucursal
END;


/* SUMAR INVENTARIO AL COMPRAR */

CREATE TRIGGER trg_sumar_inventario
ON ComprasDetalle
AFTER INSERT
AS
BEGIN
UPDATE inv
SET inv.cantidad = inv.cantidad + i.cantidad
FROM Inventario inv
JOIN inserted i ON inv.id_producto = i.id_producto
JOIN Compras c ON c.id_compra = i.id_compra
WHERE inv.id_sucursal = c.id_sucursal
END;


/* REGISTRAR INGRESO */

CREATE TRIGGER trg_ingreso_finanzas
ON Ventas
AFTER INSERT
AS
BEGIN
INSERT INTO MovimientosFinancieros(tipo,descripcion,monto,id_sucursal)
SELECT
'INGRESO',
'Venta realizada',
total,
id_sucursal
FROM inserted
END;


/* REGISTRAR GASTO */

CREATE TRIGGER trg_gasto_finanzas
ON Compras
AFTER INSERT
AS
BEGIN
INSERT INTO MovimientosFinancieros(tipo,descripcion,monto,id_sucursal)
SELECT
'GASTO',
'Compra a proveedor',
total,
id_sucursal
FROM inserted
END;

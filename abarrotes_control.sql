CREATE DATABASE IF NOT EXISTS abarrotes_control CHARACTER SET utf8mb4;
USE abarrotes_control;

CREATE TABLE IF NOT EXISTS usuarios (
    id INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    rol VARCHAR(20) NOT NULL,
    activo TINYINT(1) NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_usuarios_email (email)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS categorias (
    id INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(80) NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_categorias_nombre (nombre)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS proveedores (
    id INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(120) NOT NULL,
    telefono VARCHAR(20) NULL,
    activo TINYINT(1) NOT NULL,
    PRIMARY KEY (id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS productos (
    id INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(120) NOT NULL,
    categoria_id INT NOT NULL,
    precio_venta DECIMAL(10,2) NOT NULL,
    stock_minimo INT NOT NULL,
    stock_actual INT NOT NULL,
    fecha_vencimiento DATE NULL,
    activo TINYINT(1) NOT NULL,
    creado_por INT NOT NULL,
    fecha_registro DATETIME NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_productos_nombre (nombre),
    KEY ix_productos_categoria_id (categoria_id),
    KEY ix_productos_creado_por (creado_por),
    CONSTRAINT fk_productos_categoria FOREIGN KEY (categoria_id)
        REFERENCES categorias (id) ON DELETE RESTRICT,
    CONSTRAINT fk_productos_usuario FOREIGN KEY (creado_por)
        REFERENCES usuarios (id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS entradas_mercaderia (
    id INT NOT NULL AUTO_INCREMENT,
    proveedor_id INT NOT NULL,
    usuario_id INT NOT NULL,
    fecha_entrada DATE NOT NULL,
    nro_documento VARCHAR(50) NULL,
    fecha_registro DATETIME NOT NULL,
    PRIMARY KEY (id),
    KEY ix_entradas_proveedor_id (proveedor_id),
    KEY ix_entradas_usuario_id (usuario_id),
    CONSTRAINT fk_entradas_proveedor FOREIGN KEY (proveedor_id)
        REFERENCES proveedores (id) ON DELETE RESTRICT,
    CONSTRAINT fk_entradas_usuario FOREIGN KEY (usuario_id)
        REFERENCES usuarios (id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS detalle_entrada (
    id INT NOT NULL AUTO_INCREMENT,
    entrada_id INT NOT NULL,
    producto_id INT NOT NULL,
    cantidad INT NOT NULL,
    costo_unitario DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_detalle_entrada_producto (entrada_id, producto_id),
    KEY ix_detalle_producto_id (producto_id),
    CONSTRAINT fk_detalle_entrada FOREIGN KEY (entrada_id)
        REFERENCES entradas_mercaderia (id) ON DELETE RESTRICT,
    CONSTRAINT fk_detalle_producto FOREIGN KEY (producto_id)
        REFERENCES productos (id) ON DELETE RESTRICT
) ENGINE=InnoDB;

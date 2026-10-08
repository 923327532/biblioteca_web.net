-- ==========================================================
-- PROCEDIMIENTOS ALMACENADOS - BibliotecaDB
-- Requisito: haber ejecutado antes biblioteca.sql
-- ==========================================================
USE BibliotecaDB;
GO

-- ==========================================================
-- LIBROS
-- ==========================================================

-- Listado de libros activos con el nombre del autor
CREATE OR ALTER PROCEDURE sp_Libros_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.LibroId, l.Titulo, l.ISBN, l.AutorId,
           a.Nombre AS Autor, l.Ejemplares, l.Activo
    FROM Libros l
    INNER JOIN Autores a ON a.AutorId = l.AutorId
    WHERE l.Activo = 1
    ORDER BY l.Titulo;
END
GO

-- Búsqueda por título (coincidencia parcial)
CREATE OR ALTER PROCEDURE sp_Libros_BuscarPorTitulo
    @Titulo VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.LibroId, l.Titulo, l.ISBN, l.AutorId,
           a.Nombre AS Autor, l.Ejemplares, l.Activo
    FROM Libros l
    INNER JOIN Autores a ON a.AutorId = l.AutorId
    WHERE l.Activo = 1
      AND l.Titulo LIKE '%' + @Titulo + '%'
    ORDER BY l.Titulo;
END
GO

-- Obtener un libro por Id
CREATE OR ALTER PROCEDURE sp_Libros_ObtenerPorId
    @LibroId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.LibroId, l.Titulo, l.ISBN, l.AutorId,
           a.Nombre AS Autor, l.Ejemplares, l.Activo
    FROM Libros l
    INNER JOIN Autores a ON a.AutorId = l.AutorId
    WHERE l.LibroId = @LibroId;
END
GO

-- Insertar libro (devuelve el Id generado)
CREATE OR ALTER PROCEDURE sp_Libros_Insertar
    @Titulo     VARCHAR(150),
    @ISBN       VARCHAR(20),
    @AutorId    INT,
    @Ejemplares INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Libros (Titulo, ISBN, AutorId, Ejemplares, Activo)
    VALUES (@Titulo, @ISBN, @AutorId, @Ejemplares, 1);

    SELECT SCOPE_IDENTITY() AS LibroId;
END
GO

-- Actualizar libro
CREATE OR ALTER PROCEDURE sp_Libros_Actualizar
    @LibroId    INT,
    @Titulo     VARCHAR(150),
    @ISBN       VARCHAR(20),
    @AutorId    INT,
    @Ejemplares INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Libros
    SET Titulo     = @Titulo,
        ISBN       = @ISBN,
        AutorId    = @AutorId,
        Ejemplares = @Ejemplares
    WHERE LibroId = @LibroId;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO

-- Eliminación LÓGICA (Activo = 0), nunca DELETE
CREATE OR ALTER PROCEDURE sp_Libros_Eliminar
    @LibroId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Libros
    SET Activo = 0
    WHERE LibroId = @LibroId;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO

-- ==========================================================
-- SOCIOS
-- ==========================================================

-- Listado de socios activos
CREATE OR ALTER PROCEDURE sp_Socios_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT SocioId, DNI, Nombre, Email, Activo
    FROM Socios
    WHERE Activo = 1
    ORDER BY Nombre;
END
GO

-- Insertar socio (devuelve el Id generado)
CREATE OR ALTER PROCEDURE sp_Socios_Insertar
    @DNI    VARCHAR(15),
    @Nombre VARCHAR(100),
    @Email  VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Socios (DNI, Nombre, Email, Activo)
    VALUES (@DNI, @Nombre, @Email, 1);

    SELECT SCOPE_IDENTITY() AS SocioId;
END
GO

-- ==========================================================
-- AUTORES (para la lista desplegable)
-- ==========================================================
CREATE OR ALTER PROCEDURE sp_Autores_ListarActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AutorId, Nombre
    FROM Autores
    WHERE Activo = 1
    ORDER BY Nombre;
END
GO

-- ==========================================================
-- REPORTE: préstamos por intervalo de fechas
-- INNER JOIN entre Prestamos, DetallePrestamo, Libros y Socios
-- ==========================================================
CREATE OR ALTER PROCEDURE sp_Prestamos_ReportePorFechas
    @FechaInicio DATE,
    @FechaFin    DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.PrestamoId,
           s.DNI,
           s.Nombre AS Socio,
           l.Titulo AS Libro,
           p.FechaPrestamo,
           p.FechaLimite,
           d.FechaDevolucion,
           p.Estado
    FROM Prestamos p
    INNER JOIN DetallePrestamo d ON d.PrestamoId = p.PrestamoId
    INNER JOIN Libros l          ON l.LibroId    = d.LibroId
    INNER JOIN Socios s          ON s.SocioId    = p.SocioId
    WHERE p.FechaPrestamo BETWEEN @FechaInicio AND @FechaFin
    ORDER BY p.FechaPrestamo, p.PrestamoId;
END
GO

-- ==========================================================
-- PRUEBAS (ejecutar una por una para verificar)
-- ==========================================================
-- EXEC sp_Libros_Listar;
-- EXEC sp_Libros_BuscarPorTitulo @Titulo = 'casa';
-- EXEC sp_Libros_ObtenerPorId @LibroId = 1;
-- EXEC sp_Libros_Insertar 'Libro de prueba', '978-0000000001', 1, 3;
-- EXEC sp_Libros_Actualizar 21, 'Libro editado', '978-0000000001', 2, 4;
-- EXEC sp_Libros_Eliminar @LibroId = 21;
-- EXEC sp_Socios_Listar;
-- EXEC sp_Socios_Insertar '81234567', 'Nuevo Socio', 'nuevo@email.com';
-- EXEC sp_Autores_ListarActivos;
-- EXEC sp_Prestamos_ReportePorFechas '2026-08-01', '2026-09-30';

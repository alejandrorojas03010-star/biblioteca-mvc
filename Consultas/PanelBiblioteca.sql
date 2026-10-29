-- Consultas de lectura mostradas en Rider.
-- Ejecutar sobre BibliotecaJoseHernandez; no modifican los datos.

-- Socios: incluye los registros generados con Bogus en Development.
SELECT IdSocio, Nombre, Email, FechaAlta
FROM [BibliotecaJoseHernandez].[dbo].[Socio]
ORDER BY IdSocio;

-- Resumen del panel de inicio.
SELECT
    (SELECT COUNT(*) FROM [BibliotecaJoseHernandez].[dbo].[Socio]) AS Socios,
    (SELECT COUNT(*) FROM [BibliotecaJoseHernandez].[dbo].[Libro]) AS TitulosEnCatalogo,
    (SELECT COALESCE(SUM(Stock), 0) FROM [BibliotecaJoseHernandez].[dbo].[Libro]) AS EjemplaresDisponibles,
    (SELECT COUNT(*) FROM [BibliotecaJoseHernandez].[dbo].[Prestamo]
     WHERE FechaDevolucion IS NULL) AS PrestamosPendientes;

-- Títulos en catálogo: cada fila representa un título.
SELECT l.IdLibro, l.Titulo, a.Nombre AS Autor, l.Genero, l.Stock
FROM [BibliotecaJoseHernandez].[dbo].[Libro] AS l
JOIN [BibliotecaJoseHernandez].[dbo].[Autor] AS a ON a.IdAutor = l.IdAutor
ORDER BY l.Titulo;

-- Ejemplares disponibles por título y total de disponibilidad.
SELECT IdLibro, Titulo, Stock AS EjemplaresDisponibles,
       SUM(Stock) OVER () AS TotalEjemplaresDisponibles
FROM [BibliotecaJoseHernandez].[dbo].[Libro]
WHERE Stock > 0
ORDER BY Titulo;

-- Préstamos que aún no tienen fecha de devolución.
SELECT p.IdPrestamo, s.Nombre AS Socio, l.Titulo AS Libro,
       p.FechaPrestamo, p.FechaDevolucion,
       DATEDIFF(DAY, p.FechaPrestamo, CAST(GETDATE() AS date)) AS DiasAfuera
FROM [BibliotecaJoseHernandez].[dbo].[Prestamo] AS p
JOIN [BibliotecaJoseHernandez].[dbo].[Socio] AS s ON s.IdSocio = p.IdSocio
JOIN [BibliotecaJoseHernandez].[dbo].[Libro] AS l ON l.IdLibro = p.IdLibro
WHERE p.FechaDevolucion IS NULL
ORDER BY p.FechaPrestamo, p.IdPrestamo;

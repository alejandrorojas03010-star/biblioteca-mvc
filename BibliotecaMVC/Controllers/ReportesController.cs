using BibliotecaMVC.Data;
using BibliotecaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers;

public class ReportesController : Controller
{
    private readonly BibliotecaContext _context;

    public ReportesController(BibliotecaContext context)
    {
        _context = context;
    }

    // Reporte 1: libros que siguen prestados y hace cuánto
    public async Task<IActionResult> PrestamosPendientes(PrestamosPendientesViewModel modelo)
    {
        modelo.Socios = await _context.Socios
            .OrderBy(s => s.Nombre)
            .Select(s => new SelectListItem(s.Nombre, s.IdSocio.ToString()))
            .ToListAsync();

        if (modelo.Desde is not null && modelo.Hasta is not null && modelo.Desde > modelo.Hasta)
        {
            ModelState.AddModelError(nameof(modelo.Hasta), "La fecha \"hasta\" no puede ser anterior a la fecha \"desde\".");
        }

        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        // Pendiente = todavía no tiene fecha de devolución
        var consulta = _context.Prestamos
            .Include(p => p.Socio)
            .Include(p => p.Libro)
                .ThenInclude(l => l!.Autor)
            .Where(p => p.FechaDevolucion == null)
            .AsNoTracking();

        if (modelo.IdSocio is not null)
        {
            consulta = consulta.Where(p => p.IdSocio == modelo.IdSocio);
        }

        if (modelo.Desde is not null)
        {
            consulta = consulta.Where(p => p.FechaPrestamo >= modelo.Desde.Value.Date);
        }

        if (modelo.Hasta is not null)
        {
            consulta = consulta.Where(p => p.FechaPrestamo <= modelo.Hasta.Value.Date);
        }

        var prestamos = await consulta
            .OrderBy(p => p.FechaPrestamo)
            .ThenBy(p => p.IdPrestamo)
            .ToListAsync();

        // Los días se calculan en C#, después de traer los datos de la base
        var hoy = DateTime.Today;
        modelo.Filas = prestamos
            .Select(p =>
            {
                var dias = (hoy - p.FechaPrestamo.Date).Days;
                return new PrestamoPendienteFila
                {
                    IdPrestamo = p.IdPrestamo,
                    Socio = p.Socio!.Nombre,
                    Email = p.Socio.Email,
                    Libro = p.Libro!.Titulo,
                    Autor = p.Libro.Autor!.Nombre,
                    FechaPrestamo = p.FechaPrestamo,
                    DiasAfuera = dias,
                    EstaAtrasado = dias > modelo.DiasPermitidos
                };
            })
            .Where(f => !modelo.SoloAtrasados || f.EstaAtrasado)
            .ToList();

        return View(modelo);
    }

    // Reporte 2: qué libros se prestaron más en un período
    public async Task<IActionResult> LibrosMasPrestados(LibrosMasPrestadosViewModel modelo)
    {
        // Sin fechas, se toma desde el 1 de enero del año actual hasta hoy
        modelo.Desde ??= new DateTime(DateTime.Today.Year, 1, 1);
        modelo.Hasta ??= DateTime.Today;

        modelo.Generos = await _context.Libros
            .Select(l => l.Genero)
            .Distinct()
            .OrderBy(g => g)
            .Select(g => new SelectListItem(g, g))
            .ToListAsync();

        if (modelo.Desde > modelo.Hasta)
        {
            ModelState.AddModelError(nameof(modelo.Hasta), "La fecha \"hasta\" no puede ser anterior a la fecha \"desde\".");
            return View(modelo);
        }

        var consulta = _context.Prestamos
            .Where(p => p.FechaPrestamo >= modelo.Desde.Value.Date && p.FechaPrestamo <= modelo.Hasta.Value.Date);

        if (!string.IsNullOrWhiteSpace(modelo.Genero))
        {
            consulta = consulta.Where(p => p.Libro!.Genero == modelo.Genero);
        }

        // Agrupa los préstamos por libro y cuenta cuántos tiene cada uno
        var ranking = await consulta
            .GroupBy(p => new
            {
                p.IdLibro,
                p.Libro!.Titulo,
                Autor = p.Libro.Autor!.Nombre,
                p.Libro.Genero,
                p.Libro.Stock
            })
            .Select(g => new LibroMasPrestadoFila
            {
                IdLibro = g.Key.IdLibro,
                Titulo = g.Key.Titulo,
                Autor = g.Key.Autor,
                Genero = g.Key.Genero,
                Stock = g.Key.Stock,
                CantidadPrestamos = g.Count()
            })
            .OrderByDescending(f => f.CantidadPrestamos)
            .ThenBy(f => f.Titulo)
            .ToListAsync();

        // Totales sobre todo el período, antes de recortar al Top
        modelo.TotalPrestamos = ranking.Sum(f => f.CantidadPrestamos);
        modelo.TitulosDistintos = ranking.Count;
        modelo.GeneroMasPedido = ranking
            .GroupBy(f => f.Genero)
            .OrderByDescending(g => g.Sum(f => f.CantidadPrestamos))
            .Select(g => g.Key)
            .FirstOrDefault();

        for (var i = 0; i < ranking.Count; i++)
        {
            ranking[i].Puesto = i + 1;
            ranking[i].Porcentaje = modelo.TotalPrestamos == 0
                ? 0
                : ranking[i].CantidadPrestamos * 100.0 / modelo.TotalPrestamos;
        }

        modelo.Filas = modelo.Top > 0 ? ranking.Take(modelo.Top).ToList() : ranking;

        return View(modelo);
    }
}

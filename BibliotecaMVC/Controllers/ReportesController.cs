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
}

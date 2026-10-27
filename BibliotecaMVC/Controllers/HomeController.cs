using System.Diagnostics;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers;

public class HomeController : Controller
{
    private readonly BibliotecaContext _context;

    public HomeController(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var modelo = new InicioViewModel
        {
            CantidadSocios = await _context.Socios.CountAsync(),
            CantidadLibros = await _context.Libros.CountAsync(),
            EjemplaresDisponibles = await _context.Libros.SumAsync(l => l.Stock),
            PrestamosPendientes = await _context.Prestamos.CountAsync(p => p.FechaDevolucion == null),
            UltimosPrestamos = await _context.Prestamos
                .Include(p => p.Socio)
                .Include(p => p.Libro)
                .OrderByDescending(p => p.FechaPrestamo)
                .ThenByDescending(p => p.IdPrestamo)
                .Take(5)
                .AsNoTracking()
                .ToListAsync()
        };

        return View(modelo);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

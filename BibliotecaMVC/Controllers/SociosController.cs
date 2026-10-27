using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers;

public class SociosController : Controller
{
    private readonly BibliotecaContext _context;

    public SociosController(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? buscar)
    {
        var consulta = _context.Socios.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            consulta = consulta.Where(s => s.Nombre.Contains(buscar) || s.Email.Contains(buscar));
        }

        ViewData["Buscar"] = buscar;
        return View(await consulta.OrderBy(s => s.Nombre).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var socio = await _context.Socios
            .Include(s => s.Prestamos)
                .ThenInclude(p => p.Libro)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.IdSocio == id);

        if (socio is null)
        {
            return NotFound();
        }

        return View(socio);
    }

    public IActionResult Create()
    {
        return View(new Socio());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Email,FechaAlta")] Socio socio)
    {
        await ValidarEmailUnicoAsync(socio);

        if (!ModelState.IsValid)
        {
            return View(socio);
        }

        _context.Socios.Add(socio);
        await _context.SaveChangesAsync();

        TempData["Exito"] = $"El socio \"{socio.Nombre}\" se registró correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var socio = await _context.Socios.FindAsync(id);
        if (socio is null)
        {
            return NotFound();
        }

        return View(socio);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdSocio,Nombre,Email,FechaAlta")] Socio socio)
    {
        if (id != socio.IdSocio)
        {
            return NotFound();
        }

        await ValidarEmailUnicoAsync(socio);

        if (!ModelState.IsValid)
        {
            return View(socio);
        }

        if (!await _context.Socios.AnyAsync(s => s.IdSocio == id))
        {
            return NotFound();
        }

        _context.Update(socio);
        await _context.SaveChangesAsync();

        TempData["Exito"] = $"Los datos de \"{socio.Nombre}\" se actualizaron.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var socio = await _context.Socios
            .Include(s => s.Prestamos)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.IdSocio == id);

        if (socio is null)
        {
            return NotFound();
        }

        return View(socio);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var socio = await _context.Socios.FindAsync(id);
        if (socio is null)
        {
            return NotFound();
        }

        // Se conserva el historial: no se borra un socio con préstamos registrados
        if (await _context.Prestamos.AnyAsync(p => p.IdSocio == id))
        {
            TempData["Error"] = $"No se puede eliminar a \"{socio.Nombre}\" porque tiene préstamos registrados.";
            return RedirectToAction(nameof(Index));
        }

        _context.Socios.Remove(socio);
        await _context.SaveChangesAsync();

        TempData["Exito"] = $"El socio \"{socio.Nombre}\" fue eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarEmailUnicoAsync(Socio socio)
    {
        var emailEnUso = await _context.Socios
            .AnyAsync(s => s.Email == socio.Email && s.IdSocio != socio.IdSocio);

        if (emailEnUso)
        {
            ModelState.AddModelError(nameof(Socio.Email), "Ya existe un socio con ese email.");
        }
    }
}

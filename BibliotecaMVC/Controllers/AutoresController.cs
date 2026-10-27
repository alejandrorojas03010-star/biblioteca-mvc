using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers;

public class AutoresController : Controller
{
    private readonly BibliotecaContext _context;

    public AutoresController(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? buscar)
    {
        var consulta = _context.Autores.Include(a => a.Libros).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            consulta = consulta.Where(a => a.Nombre.Contains(buscar) || a.Nacionalidad.Contains(buscar));
        }

        ViewData["Buscar"] = buscar;
        return View(await consulta.OrderBy(a => a.Nombre).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var autor = await _context.Autores
            .Include(a => a.Libros)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.IdAutor == id);

        if (autor is null)
        {
            return NotFound();
        }

        return View(autor);
    }

    public IActionResult Create()
    {
        return View(new Autor());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Nacionalidad")] Autor autor)
    {
        if (!ModelState.IsValid)
        {
            return View(autor);
        }

        _context.Autores.Add(autor);
        await _context.SaveChangesAsync();

        TempData["Exito"] = $"El autor \"{autor.Nombre}\" se registró correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var autor = await _context.Autores.FindAsync(id);
        if (autor is null)
        {
            return NotFound();
        }

        return View(autor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdAutor,Nombre,Nacionalidad")] Autor autor)
    {
        if (id != autor.IdAutor)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(autor);
        }

        if (!await _context.Autores.AnyAsync(a => a.IdAutor == id))
        {
            return NotFound();
        }

        _context.Update(autor);
        await _context.SaveChangesAsync();

        TempData["Exito"] = $"Los datos de \"{autor.Nombre}\" se actualizaron.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var autor = await _context.Autores
            .Include(a => a.Libros)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.IdAutor == id);

        if (autor is null)
        {
            return NotFound();
        }

        return View(autor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var autor = await _context.Autores.FindAsync(id);
        if (autor is null)
        {
            return NotFound();
        }

        if (await _context.Libros.AnyAsync(l => l.IdAutor == id))
        {
            TempData["Error"] = $"No se puede eliminar a \"{autor.Nombre}\" porque tiene libros en el catálogo.";
            return RedirectToAction(nameof(Index));
        }

        _context.Autores.Remove(autor);
        await _context.SaveChangesAsync();

        TempData["Exito"] = $"El autor \"{autor.Nombre}\" fue eliminado.";
        return RedirectToAction(nameof(Index));
    }
}

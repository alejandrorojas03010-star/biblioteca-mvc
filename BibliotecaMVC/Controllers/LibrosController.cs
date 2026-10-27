using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers;

public class LibrosController : Controller
{
    private readonly BibliotecaContext _context;

    public LibrosController(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? buscar, string? genero)
    {
        var consulta = _context.Libros.Include(l => l.Autor).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            consulta = consulta.Where(l => l.Titulo.Contains(buscar) || l.Autor!.Nombre.Contains(buscar));
        }

        if (!string.IsNullOrWhiteSpace(genero))
        {
            consulta = consulta.Where(l => l.Genero == genero);
        }

        ViewData["Buscar"] = buscar;
        ViewData["Genero"] = genero;
        ViewData["Generos"] = new SelectList(await ObtenerGenerosAsync(), genero);

        return View(await consulta.OrderBy(l => l.Titulo).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var libro = await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.Prestamos)
                .ThenInclude(p => p.Socio)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.IdLibro == id);

        if (libro is null)
        {
            return NotFound();
        }

        return View(libro);
    }

    public async Task<IActionResult> Create()
    {
        var modelo = new LibroFormViewModel { Stock = 1 };
        await CargarListasAsync(modelo);
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LibroFormViewModel modelo)
    {
        await ValidarAutorAsync(modelo);

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(modelo);
            return View(modelo);
        }

        var libro = new Libro();
        CopiarDatos(modelo, libro);

        _context.Libros.Add(libro);
        await _context.SaveChangesAsync();

        TempData["Exito"] = $"El libro \"{libro.Titulo}\" se agregó al catálogo.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var libro = await _context.Libros.FindAsync(id);
        if (libro is null)
        {
            return NotFound();
        }

        var modelo = new LibroFormViewModel
        {
            IdLibro = libro.IdLibro,
            Titulo = libro.Titulo,
            Genero = libro.Genero,
            Stock = libro.Stock,
            IdAutor = libro.IdAutor
        };

        await CargarListasAsync(modelo);
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LibroFormViewModel modelo)
    {
        if (id != modelo.IdLibro)
        {
            return NotFound();
        }

        var libro = await _context.Libros.FindAsync(id);
        if (libro is null)
        {
            return NotFound();
        }

        await ValidarAutorAsync(modelo);

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(modelo);
            return View(modelo);
        }

        CopiarDatos(modelo, libro);
        await _context.SaveChangesAsync();

        TempData["Exito"] = $"El libro \"{libro.Titulo}\" se actualizó.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var libro = await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.Prestamos)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.IdLibro == id);

        if (libro is null)
        {
            return NotFound();
        }

        return View(libro);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var libro = await _context.Libros.FindAsync(id);
        if (libro is null)
        {
            return NotFound();
        }

        if (await _context.Prestamos.AnyAsync(p => p.IdLibro == id))
        {
            TempData["Error"] = $"No se puede eliminar \"{libro.Titulo}\" porque tiene préstamos registrados.";
            return RedirectToAction(nameof(Index));
        }

        _context.Libros.Remove(libro);
        await _context.SaveChangesAsync();

        TempData["Exito"] = $"El libro \"{libro.Titulo}\" fue eliminado del catálogo.";
        return RedirectToAction(nameof(Index));
    }

    private static void CopiarDatos(LibroFormViewModel modelo, Libro libro)
    {
        libro.Titulo = modelo.Titulo.Trim();
        libro.Genero = modelo.Genero.Trim();
        libro.Stock = modelo.Stock!.Value;
        libro.IdAutor = modelo.IdAutor!.Value;
    }

    private async Task ValidarAutorAsync(LibroFormViewModel modelo)
    {
        if (modelo.IdAutor is not null && !await _context.Autores.AnyAsync(a => a.IdAutor == modelo.IdAutor))
        {
            ModelState.AddModelError(nameof(LibroFormViewModel.IdAutor), "El autor seleccionado no existe.");
        }
    }

    private async Task CargarListasAsync(LibroFormViewModel modelo)
    {
        modelo.Autores = await _context.Autores
            .OrderBy(a => a.Nombre)
            .Select(a => new SelectListItem(a.Nombre, a.IdAutor.ToString()))
            .ToListAsync();

        modelo.GenerosExistentes = await ObtenerGenerosAsync();
    }

    private async Task<List<string>> ObtenerGenerosAsync()
    {
        return await _context.Libros
            .Select(l => l.Genero)
            .Distinct()
            .OrderBy(g => g)
            .ToListAsync();
    }
}

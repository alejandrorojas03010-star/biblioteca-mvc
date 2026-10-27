using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers;

public class PrestamosController : Controller
{
    private readonly BibliotecaContext _context;

    public PrestamosController(BibliotecaContext context)
    {
        _context = context;
    }

    // estado: "pendientes", "devueltos" o vacío para todos
    public async Task<IActionResult> Index(string? buscar, string? estado)
    {
        var consulta = _context.Prestamos
            .Include(p => p.Socio)
            .Include(p => p.Libro)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            consulta = consulta.Where(p => p.Socio!.Nombre.Contains(buscar) || p.Libro!.Titulo.Contains(buscar));
        }

        consulta = estado switch
        {
            "pendientes" => consulta.Where(p => p.FechaDevolucion == null),
            "devueltos" => consulta.Where(p => p.FechaDevolucion != null),
            _ => consulta
        };

        ViewData["Buscar"] = buscar;
        ViewData["Estado"] = estado;

        return View(await consulta
            .OrderByDescending(p => p.FechaPrestamo)
            .ThenByDescending(p => p.IdPrestamo)
            .ToListAsync());
    }

    public async Task<IActionResult> Create(int? idSocio, int? idLibro)
    {
        var modelo = new PrestamoCreateViewModel { IdSocio = idSocio, IdLibro = idLibro };
        await CargarListasAsync(modelo);
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PrestamoCreateViewModel modelo)
    {
        var socio = modelo.IdSocio is null ? null : await _context.Socios.FindAsync(modelo.IdSocio);
        var libro = modelo.IdLibro is null ? null : await _context.Libros.FindAsync(modelo.IdLibro);

        if (modelo.IdSocio is not null && socio is null)
        {
            ModelState.AddModelError(nameof(modelo.IdSocio), "El socio seleccionado no existe.");
        }

        if (modelo.IdLibro is not null && libro is null)
        {
            ModelState.AddModelError(nameof(modelo.IdLibro), "El libro seleccionado no existe.");
        }

        if (libro is not null && libro.Stock <= 0)
        {
            ModelState.AddModelError(nameof(modelo.IdLibro), $"\"{libro.Titulo}\" no tiene ejemplares disponibles.");
        }

        if (socio is not null && modelo.FechaPrestamo.Date < socio.FechaAlta.Date)
        {
            ModelState.AddModelError(nameof(modelo.FechaPrestamo),
                $"La fecha no puede ser anterior al alta del socio ({socio.FechaAlta:dd/MM/yyyy}).");
        }

        if (socio is not null && libro is not null &&
            await _context.Prestamos.AnyAsync(p => p.IdSocio == socio.IdSocio &&
                                                   p.IdLibro == libro.IdLibro &&
                                                   p.FechaDevolucion == null))
        {
            ModelState.AddModelError(string.Empty, $"{socio.Nombre} ya tiene \"{libro.Titulo}\" prestado y aún no lo devolvió.");
        }

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(modelo);
            return View(modelo);
        }

        _context.Prestamos.Add(new Prestamo
        {
            IdSocio = socio!.IdSocio,
            IdLibro = libro!.IdLibro,
            FechaPrestamo = modelo.FechaPrestamo.Date
        });

        // Un ejemplar menos disponible; se guarda junto con el préstamo
        libro.Stock--;
        await _context.SaveChangesAsync();

        TempData["Exito"] = $"Préstamo registrado: \"{libro.Titulo}\" para {socio.Nombre}.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Devolver(int id)
    {
        var prestamo = await BuscarConDetalleAsync(id);
        if (prestamo is null)
        {
            return NotFound();
        }

        if (!prestamo.EstaPendiente)
        {
            TempData["Error"] = "Ese préstamo ya fue devuelto.";
            return RedirectToAction(nameof(Index));
        }

        var modelo = CrearEditViewModel(prestamo);
        modelo.FechaDevolucion = DateTime.Today;
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Devolver(int id, PrestamoEditViewModel modelo)
    {
        var prestamo = await BuscarConDetalleAsync(id, seguimiento: true);
        if (prestamo is null || id != modelo.IdPrestamo)
        {
            return NotFound();
        }

        if (!prestamo.EstaPendiente)
        {
            TempData["Error"] = "Ese préstamo ya fue devuelto.";
            return RedirectToAction(nameof(Index));
        }

        // La fecha de préstamo no se edita en esta pantalla
        modelo.FechaPrestamo = prestamo.FechaPrestamo;
        ModelState.Remove(nameof(modelo.FechaPrestamo));

        if (modelo.FechaDevolucion is null)
        {
            ModelState.AddModelError(nameof(modelo.FechaDevolucion), "Ingresá la fecha de devolución.");
        }
        else if (modelo.FechaDevolucion.Value.Date < prestamo.FechaPrestamo.Date)
        {
            ModelState.AddModelError(nameof(modelo.FechaDevolucion),
                "La fecha de devolución no puede ser anterior a la fecha de préstamo.");
        }

        if (!ModelState.IsValid)
        {
            CompletarNombres(modelo, prestamo);
            return View(modelo);
        }

        prestamo.FechaDevolucion = modelo.FechaDevolucion!.Value.Date;
        prestamo.Libro!.Stock++;
        await _context.SaveChangesAsync();

        TempData["Exito"] = $"Se registró la devolución de \"{prestamo.Libro.Titulo}\".";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var prestamo = await BuscarConDetalleAsync(id);
        if (prestamo is null)
        {
            return NotFound();
        }

        return View(CrearEditViewModel(prestamo));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PrestamoEditViewModel modelo)
    {
        var prestamo = await BuscarConDetalleAsync(id, seguimiento: true);
        if (prestamo is null || id != modelo.IdPrestamo)
        {
            return NotFound();
        }

        if (modelo.FechaPrestamo.Date < prestamo.Socio!.FechaAlta.Date)
        {
            ModelState.AddModelError(nameof(modelo.FechaPrestamo),
                $"La fecha no puede ser anterior al alta del socio ({prestamo.Socio.FechaAlta:dd/MM/yyyy}).");
        }

        var estabaPendiente = prestamo.EstaPendiente;
        var quedaPendiente = modelo.FechaDevolucion is null;

        // Volver a marcar como pendiente un préstamo devuelto necesita un ejemplar disponible
        if (!estabaPendiente && quedaPendiente && prestamo.Libro!.Stock <= 0)
        {
            ModelState.AddModelError(nameof(modelo.FechaDevolucion),
                "No se puede quitar la devolución: el libro no tiene ejemplares disponibles.");
        }

        if (!ModelState.IsValid)
        {
            CompletarNombres(modelo, prestamo);
            return View(modelo);
        }

        prestamo.FechaPrestamo = modelo.FechaPrestamo.Date;
        prestamo.FechaDevolucion = modelo.FechaDevolucion?.Date;

        if (estabaPendiente && !quedaPendiente)
        {
            prestamo.Libro!.Stock++;
        }
        else if (!estabaPendiente && quedaPendiente)
        {
            prestamo.Libro!.Stock--;
        }

        await _context.SaveChangesAsync();

        TempData["Exito"] = $"El préstamo Nº {prestamo.IdPrestamo} se actualizó.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var prestamo = await BuscarConDetalleAsync(id);
        if (prestamo is null)
        {
            return NotFound();
        }

        return View(prestamo);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var prestamo = await BuscarConDetalleAsync(id, seguimiento: true);
        if (prestamo is null)
        {
            return NotFound();
        }

        // Si el libro seguía prestado, el ejemplar vuelve a estar disponible
        if (prestamo.EstaPendiente)
        {
            prestamo.Libro!.Stock++;
        }

        _context.Prestamos.Remove(prestamo);
        await _context.SaveChangesAsync();

        TempData["Exito"] = $"El préstamo Nº {prestamo.IdPrestamo} fue eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<Prestamo?> BuscarConDetalleAsync(int id, bool seguimiento = false)
    {
        var consulta = _context.Prestamos
            .Include(p => p.Socio)
            .Include(p => p.Libro)
                .ThenInclude(l => l!.Autor)
            .AsQueryable();

        if (!seguimiento)
        {
            consulta = consulta.AsNoTracking();
        }

        return await consulta.FirstOrDefaultAsync(p => p.IdPrestamo == id);
    }

    private static PrestamoEditViewModel CrearEditViewModel(Prestamo prestamo)
    {
        var modelo = new PrestamoEditViewModel
        {
            IdPrestamo = prestamo.IdPrestamo,
            FechaPrestamo = prestamo.FechaPrestamo,
            FechaDevolucion = prestamo.FechaDevolucion
        };

        CompletarNombres(modelo, prestamo);
        return modelo;
    }

    private static void CompletarNombres(PrestamoEditViewModel modelo, Prestamo prestamo)
    {
        modelo.NombreSocio = prestamo.Socio!.Nombre;
        modelo.TituloLibro = prestamo.Libro!.Titulo;
    }

    private async Task CargarListasAsync(PrestamoCreateViewModel modelo)
    {
        modelo.Socios = await _context.Socios
            .OrderBy(s => s.Nombre)
            .Select(s => new SelectListItem($"{s.Nombre} ({s.Email})", s.IdSocio.ToString()))
            .ToListAsync();

        modelo.Libros = await _context.Libros
            .Where(l => l.Stock > 0)
            .OrderBy(l => l.Titulo)
            .Select(l => new SelectListItem($"{l.Titulo} — {l.Autor!.Nombre} (disponibles: {l.Stock})", l.IdLibro.ToString()))
            .ToListAsync();
    }
}

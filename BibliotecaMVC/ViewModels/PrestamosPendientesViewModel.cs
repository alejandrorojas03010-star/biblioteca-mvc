using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BibliotecaMVC.ViewModels;

/// <summary>
/// Reporte 1: préstamos que todavía no fueron devueltos.
/// Las propiedades de filtro llegan por la URL (formulario GET).
/// </summary>
public class PrestamosPendientesViewModel
{
    // ---- Filtros ----

    [Display(Name = "Socio")]
    public int? IdSocio { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Prestado desde")]
    public DateTime? Desde { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Prestado hasta")]
    public DateTime? Hasta { get; set; }

    [Range(1, 365, ErrorMessage = "Los días permitidos deben estar entre {1} y {2}.")]
    [Display(Name = "Días permitidos")]
    public int DiasPermitidos { get; set; } = 14;

    [Display(Name = "Solo atrasados")]
    public bool SoloAtrasados { get; set; }

    // ---- Resultado ----

    public List<PrestamoPendienteFila> Filas { get; set; } = [];

    public int CantidadPendientes => Filas.Count;

    public int CantidadAtrasados => Filas.Count(f => f.EstaAtrasado);

    public double PromedioDiasAfuera => Filas.Count == 0 ? 0 : Filas.Average(f => f.DiasAfuera);

    public IEnumerable<SelectListItem> Socios { get; set; } = [];
}

public class PrestamoPendienteFila
{
    public int IdPrestamo { get; set; }
    public string Socio { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Libro { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public DateTime FechaPrestamo { get; set; }
    public int DiasAfuera { get; set; }
    public bool EstaAtrasado { get; set; }
}

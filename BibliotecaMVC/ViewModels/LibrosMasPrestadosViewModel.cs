using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BibliotecaMVC.ViewModels;

/// <summary>
/// Reporte 2: ranking de libros según la cantidad de préstamos en un período.
/// Las propiedades de filtro llegan por la URL (formulario GET).
/// </summary>
public class LibrosMasPrestadosViewModel
{
    // ---- Filtros ----

    [DataType(DataType.Date)]
    [Display(Name = "Desde")]
    public DateTime? Desde { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Hasta")]
    public DateTime? Hasta { get; set; }

    [Display(Name = "Género")]
    public string? Genero { get; set; }

    // 0 = mostrar todos
    [Display(Name = "Mostrar")]
    public int Top { get; set; } = 10;

    // ---- Resultado ----

    public List<LibroMasPrestadoFila> Filas { get; set; } = [];

    public int TotalPrestamos { get; set; }

    public int TitulosDistintos { get; set; }

    public string? GeneroMasPedido { get; set; }

    public IEnumerable<SelectListItem> Generos { get; set; } = [];

    public IEnumerable<SelectListItem> OpcionesTop { get; } =
    [
        new("Top 5", "5"),
        new("Top 10", "10"),
        new("Todos", "0")
    ];
}

public class LibroMasPrestadoFila
{
    public int Puesto { get; set; }
    public int IdLibro { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public int CantidadPrestamos { get; set; }
    public double Porcentaje { get; set; }
    public int Stock { get; set; }
}

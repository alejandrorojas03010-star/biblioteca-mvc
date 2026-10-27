using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BibliotecaMVC.ViewModels;

public class LibroFormViewModel
{
    public int IdLibro { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(150, ErrorMessage = "El título no puede superar los {1} caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El género es obligatorio.")]
    [StringLength(50, ErrorMessage = "El género no puede superar los {1} caracteres.")]
    [Display(Name = "Género")]
    public string Genero { get; set; } = string.Empty;

    [Required(ErrorMessage = "El stock es obligatorio.")]
    [Range(0, 1000, ErrorMessage = "El stock debe estar entre {1} y {2}.")]
    [Display(Name = "Stock disponible")]
    public int? Stock { get; set; }

    [Required(ErrorMessage = "Seleccioná un autor.")]
    [Display(Name = "Autor")]
    public int? IdAutor { get; set; }

    public IEnumerable<SelectListItem> Autores { get; set; } = [];

    // Géneros ya cargados, para sugerirlos al escribir
    public IEnumerable<string> GenerosExistentes { get; set; } = [];
}

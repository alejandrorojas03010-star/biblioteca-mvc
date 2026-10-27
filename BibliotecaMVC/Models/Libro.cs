using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models;

public class Libro
{
    [Key]
    [Display(Name = "Nº de libro")]
    public int IdLibro { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(150, ErrorMessage = "El título no puede superar los {1} caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El género es obligatorio.")]
    [StringLength(50, ErrorMessage = "El género no puede superar los {1} caracteres.")]
    [Display(Name = "Género")]
    public string Genero { get; set; } = string.Empty;

    [Range(0, 1000, ErrorMessage = "El stock debe estar entre {1} y {2}.")]
    [Display(Name = "Stock disponible")]
    public int Stock { get; set; }

    // FK de la relación ESCRIBE (N libros : 1 autor)
    [Display(Name = "Autor")]
    public int IdAutor { get; set; }

    public Autor? Autor { get; set; }

    public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}

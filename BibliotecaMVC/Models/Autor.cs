using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models;

public class Autor
{
    [Key]
    [Display(Name = "Nº de autor")]
    public int IdAutor { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nacionalidad es obligatoria.")]
    [StringLength(60, ErrorMessage = "La nacionalidad no puede superar los {1} caracteres.")]
    public string Nacionalidad { get; set; } = string.Empty;

    public ICollection<Libro> Libros { get; set; } = new List<Libro>();
}

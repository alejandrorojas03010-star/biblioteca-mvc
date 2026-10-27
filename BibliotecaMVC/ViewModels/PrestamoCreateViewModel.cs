using System.ComponentModel.DataAnnotations;
using BibliotecaMVC.Validations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BibliotecaMVC.ViewModels;

public class PrestamoCreateViewModel
{
    [Required(ErrorMessage = "Seleccioná un socio.")]
    [Display(Name = "Socio")]
    public int? IdSocio { get; set; }

    [Required(ErrorMessage = "Seleccioná un libro.")]
    [Display(Name = "Libro")]
    public int? IdLibro { get; set; }

    [Required(ErrorMessage = "La fecha de préstamo es obligatoria.")]
    [DataType(DataType.Date)]
    [NoFutura]
    [Display(Name = "Fecha de préstamo")]
    public DateTime FechaPrestamo { get; set; } = DateTime.Today;

    public IEnumerable<SelectListItem> Socios { get; set; } = [];

    // Solo libros con stock disponible
    public IEnumerable<SelectListItem> Libros { get; set; } = [];
}

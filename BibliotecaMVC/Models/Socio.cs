using System.ComponentModel.DataAnnotations;
using BibliotecaMVC.Validations;

namespace BibliotecaMVC.Models;

public class Socio
{
    [Key]
    [Display(Name = "Nº de socio")]
    public int IdSocio { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresá un email válido.")]
    [StringLength(150, ErrorMessage = "El email no puede superar los {1} caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de alta es obligatoria.")]
    [DataType(DataType.Date)]
    [NoFutura]
    [Display(Name = "Fecha de alta")]
    public DateTime FechaAlta { get; set; } = DateTime.Today;

    public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}

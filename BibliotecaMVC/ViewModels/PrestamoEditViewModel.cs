using System.ComponentModel.DataAnnotations;
using BibliotecaMVC.Validations;

namespace BibliotecaMVC.ViewModels;

/// <summary>
/// Se usa para editar fechas y para registrar la devolución.
/// El socio y el libro no se cambian para no descuadrar el stock.
/// </summary>
public class PrestamoEditViewModel : IValidatableObject
{
    public int IdPrestamo { get; set; }

    [Display(Name = "Socio")]
    public string NombreSocio { get; set; } = string.Empty;

    [Display(Name = "Libro")]
    public string TituloLibro { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de préstamo es obligatoria.")]
    [DataType(DataType.Date)]
    [NoFutura]
    [Display(Name = "Fecha de préstamo")]
    public DateTime FechaPrestamo { get; set; }

    [DataType(DataType.Date)]
    [NoFutura]
    [Display(Name = "Fecha de devolución")]
    public DateTime? FechaDevolucion { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FechaDevolucion is not null && FechaDevolucion.Value.Date < FechaPrestamo.Date)
        {
            yield return new ValidationResult(
                "La fecha de devolución no puede ser anterior a la fecha de préstamo.",
                [nameof(FechaDevolucion)]);
        }
    }
}

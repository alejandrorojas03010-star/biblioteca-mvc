using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Validations;

/// <summary>
/// Valida que una fecha no sea posterior al día de hoy.
/// </summary>
public class NoFuturaAttribute : ValidationAttribute
{
    public NoFuturaAttribute()
        : base("La fecha de {0} no puede ser posterior a hoy.")
    {
    }

    public override bool IsValid(object? value)
    {
        return value is not DateTime fecha || fecha.Date <= DateTime.Today;
    }
}

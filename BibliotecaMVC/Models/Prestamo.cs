using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models;

public class Prestamo
{
    [Key]
    [Display(Name = "Nº de préstamo")]
    public int IdPrestamo { get; set; }

    // FK de la relación REALIZA (1 socio : N préstamos)
    [Display(Name = "Socio")]
    public int IdSocio { get; set; }

    public Socio? Socio { get; set; }

    // FK de la relación INCLUYE (N préstamos : 1 libro)
    [Display(Name = "Libro")]
    public int IdLibro { get; set; }

    public Libro? Libro { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de préstamo")]
    public DateTime FechaPrestamo { get; set; }

    // Null mientras el libro no fue devuelto
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de devolución")]
    public DateTime? FechaDevolucion { get; set; }

    public bool EstaPendiente => FechaDevolucion is null;
}

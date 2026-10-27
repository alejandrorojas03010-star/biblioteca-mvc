using BibliotecaMVC.Models;

namespace BibliotecaMVC.ViewModels;

public class InicioViewModel
{
    public int CantidadSocios { get; set; }
    public int CantidadLibros { get; set; }
    public int EjemplaresDisponibles { get; set; }
    public int PrestamosPendientes { get; set; }
    public IEnumerable<Prestamo> UltimosPrestamos { get; set; } = [];
}

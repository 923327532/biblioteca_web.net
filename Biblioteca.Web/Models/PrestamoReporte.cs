using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models;

public class PrestamoReporte
{
    public int PrestamoId { get; set; }
    public string DNI { get; set; } = string.Empty;
    public string Socio { get; set; } = string.Empty;
    public string Libro { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime FechaPrestamo { get; set; }

    [DataType(DataType.Date)]
    public DateTime FechaLimite { get; set; }

    [DataType(DataType.Date)]
    public DateTime? FechaDevolucion { get; set; }

    public string Estado { get; set; } = string.Empty;
}

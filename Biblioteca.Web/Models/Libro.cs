using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models;

public class Libro
{
    public int LibroId { get; set; }

    [Required(ErrorMessage = "El titulo es obligatorio.")]
    [StringLength(150, ErrorMessage = "El titulo no puede exceder 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ISBN es obligatorio.")]
    [StringLength(20, ErrorMessage = "El ISBN no puede exceder 20 caracteres.")]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione un autor.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un autor valido.")]
    public int AutorId { get; set; }

    public string? Autor { get; set; }

    [Required(ErrorMessage = "Los ejemplares son obligatorios.")]
    [Range(0, 9999, ErrorMessage = "Los ejemplares deben estar entre 0 y 9999.")]
    public int Ejemplares { get; set; }

    public bool Activo { get; set; } = true;
}

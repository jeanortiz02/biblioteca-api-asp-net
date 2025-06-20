
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs;

public class LibroCreactionDTO
{
    [Required]
    [StringLength(250, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
    public required string Titulo { get; set; }
    public List<int> AutoresId { get; set; } = [];
}

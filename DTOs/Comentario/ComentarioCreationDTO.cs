using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs;

public class ComentarioCreationDTO
{
    [Required]
    public required string Cuerpo { get; set; }
}

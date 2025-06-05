using BibliotecaAPI.Entidades;

namespace BibliotecaAPI.DTOs;

public class AutoresDTO
{
    public int Id { get; set; }
    public required string NombreCompleto { get; set; }
}

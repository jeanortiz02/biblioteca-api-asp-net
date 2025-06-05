
using BibliotecaAPI.Entidades;

namespace BibliotecaAPI.DTOs;

public class AutorConLibrosDTO : AutoresDTO
{
    public List<Libro> Libros { get; set; } = [];
}

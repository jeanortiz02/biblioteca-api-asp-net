using System;

namespace BibliotecaAPI.DTOs;

public class LibroConAutoresDTO : LibroDTO
{
    public List<AutoresDTO> Autores { get; set; } = [];

}



namespace BibliotecaAPI.DTOs.Autor;

public class AutorFiltroDTO
{
    public int Pagina { get; set; }
    public int RecordsPorPagina { get; set; }
    public PaginationDTO PaginationDTO
    {
        get
        {
            return new PaginationDTO(Pagina, RecordsPorPagina);
        }
    }

    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public bool? TieneFoto { get; set; }
    public bool? TieneLibros { get; set; }
    public string? TituloLibro { get; set; }
    public bool IncluirLibros { get; set; }
    public string? CampoOrdenar { get; set; }
    public bool OrdenAscendente { get; set; }
    
}

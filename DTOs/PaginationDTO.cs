

namespace BibliotecaAPI.DTOs;

public record PaginationDTO(int Pagina = 1, int RecordPorPagina = 10)

{
    private const int CantidadMaximaRecordPorPagina = 50;
    public int Pagina { get; set; } = Math.Max(1, Pagina);
    public int RecordPorPagina { get; set; } = Math.Clamp(RecordPorPagina, 1, CantidadMaximaRecordPorPagina);
}

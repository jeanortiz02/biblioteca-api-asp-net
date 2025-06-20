using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Utilidades;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Servicios.V1;


public class ServicioAutores : IServicioAutores
{
    private readonly AplicationDbContext context;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IMapper mapper;

    public ServicioAutores(AplicationDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        this.context = context;
        this.httpContextAccessor = httpContextAccessor;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<AutorDTO>> Get( PaginationDTO paginationDTO)
    {
        // throw new NotImplementedException();
        var queryable = context.Autores.AsQueryable();
        await httpContextAccessor.HttpContext!.InsertarParametrosPaginacionEnCabecera(queryable);
        var autores = await queryable
                                .OrderBy(x => x.Nombres)
                                .Paginar(paginationDTO)
                                .ToListAsync();
        var autoresDto = mapper.Map<IEnumerable<AutorDTO>>(autores); // <IEnumerable<AutoresDTO>>

        return autoresDto;
    }
}

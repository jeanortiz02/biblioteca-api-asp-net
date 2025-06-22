
using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers.V1;


[ApiController]
[Route("api/v1/libros")]
[Authorize(Policy = "esadmin")]
public class LibrosController : ControllerBase
{
    private readonly AplicationDbContext context;
    private readonly IMapper mapper;
    private readonly IOutputCacheStore outputCacheStore;
    private const string cache = "libros-obtener";

    public LibrosController(
        AplicationDbContext context,
        IMapper mapper,
        IOutputCacheStore outputCacheStore

        )
    {
        this.context = context;
        this.mapper = mapper;
        this.outputCacheStore = outputCacheStore;
    }


    [HttpGet (Name = "ObtenerLibrosV1")]
    [AllowAnonymous]
    [OutputCache]
    public async Task<IEnumerable<LibroDTO>> Get([FromQuery] PaginationDTO paginationDTO)
    {
        var queryable = context.Libros.AsQueryable();
        await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
        var libros = await queryable
                            .OrderBy(x => x.Titulo)
                            .Paginar(paginationDTO)
                            .ToListAsync();

        var librosDTOs = mapper.Map<IEnumerable<LibroDTO>>(libros);
        return librosDTOs;
    }

    [HttpGet("{id:int}", Name = "ObtenerLibroV1")]
    [AllowAnonymous]
    [OutputCache(Tags = [cache])]
    public async Task<ActionResult<LibroConAutoresDTO>> Get(int id)
    {
        var libro = await context.Libros
                    .Include(x => x.Autores)
                    .ThenInclude(x => x.Autor)
                    .FirstOrDefaultAsync(x => x.Id == id);

        if (libro is null)
        {
            return BadRequest("Libro no registrado");
        }

        var libroDTO = mapper.Map<LibroConAutoresDTO>(libro);

        return libroDTO;
    }

    [HttpPost (Name = "CrearLibroV1")]
    [ServiceFilter<FiltroValidacionLibro>()]
    public async Task<ActionResult> Post(LibroCreactionDTO libroCreactionDTO)
    {
        // if (libroCreactionDTO.AutoresId is null || libroCreactionDTO.AutoresId.Count == 0)
        // {
        //     ModelState.AddModelError(nameof(libroCreactionDTO.AutoresId), "No se puede crear un libro sin autores");
        //     return ValidationProblem();
        // }

        // var autoresIdsExisten = await context.Autores
        //                         .Where(x => libroCreactionDTO.AutoresId.Contains(x.Id))
        //                         .Select(x => x.Id).ToListAsync();

        // if (libroCreactionDTO.AutoresId.Count != autoresIdsExisten.Count)
        // {
        //     var autoresNoExisten = libroCreactionDTO.AutoresId.Except(autoresIdsExisten);
        //     var autoresNoExistenString = string.Join(", ", autoresNoExisten); // "1, 2, 3"
        //     var mensajedeError = "Los autores con ids: " + autoresNoExistenString + " no existen";
        //     ModelState.AddModelError(nameof(libroCreactionDTO.AutoresId), mensajedeError);
        //     return ValidationProblem();
        // }

        var libro = mapper.Map<Libro>(libroCreactionDTO);
        AsignarOrdenAutores(libro);
        context.Add(libro);
        await context.SaveChangesAsync();
        await outputCacheStore.EvictByTagAsync(cache, default);

        var libroDTO = mapper.Map<LibroDTO>(libro);

        return CreatedAtRoute("ObtenerLibroV1", new { id = libro.Id }, libroDTO);
    }

    private void AsignarOrdenAutores(Libro libro)
    {
        if (libro.Autores is not null)
        {
            for (int i = 0; i < libro.Autores.Count; i++)
            {
                libro.Autores[i].Orden = i;
            }
        }
    }

    [HttpPut("{id:int}", Name = "ActualizarLibroV1")] // api/libros/id
    [ServiceFilter<FiltroValidacionLibro>()]
    public async Task<ActionResult> Put(int id, LibroCreactionDTO libroCreactionDTO)
    {
        // if (libroCreactionDTO.AutoresId is null || libroCreactionDTO.AutoresId.Count == 0)
        // {
        //     ModelState.AddModelError(nameof(libroCreactionDTO.AutoresId), "No se puede crear un libro sin autores");
        //     return ValidationProblem();
        // }

        // var autoresIdsExisten = await context.Autores
        //                         .Where(x => libroCreactionDTO.AutoresId.Contains(x.Id))
        //                         .Select(x => x.Id).ToListAsync();

        // if (libroCreactionDTO.AutoresId.Count != autoresIdsExisten.Count)
        // {
        //     var autoresNoExisten = libroCreactionDTO.AutoresId.Except(autoresIdsExisten);
        //     var autoresNoExistenString = string.Join(", ", autoresNoExisten); // "1, 2, 3"
        //     var mensajedeError = "Los autores con ids: " + autoresNoExistenString + " no existen";
        //     ModelState.AddModelError(nameof(libroCreactionDTO.AutoresId), mensajedeError);
        //     return ValidationProblem();
        // }

        var libroDb = await context.Libros
                        .Include(x => x.Autores)
                        .FirstOrDefaultAsync(x => x.Id == id);

        if (libroDb is null)
        {
            return NotFound();
        }

        libroDb = mapper.Map(libroCreactionDTO, libroDb);
        AsignarOrdenAutores(libroDb);

        await context.SaveChangesAsync();
        await outputCacheStore.EvictByTagAsync(cache, default);
        return NoContent();
    }

    [HttpDelete("{id:int}", Name = "EliminarLibroV1")]
    public async Task<ActionResult> Delete(int id)
    {
        var registroBorrados = await context.Autores.Where(x => x.Id == id).ExecuteDeleteAsync();

        if (registroBorrados == 0)
        {
            return NotFound();
        }
        await outputCacheStore.EvictByTagAsync(cache, default);

        return NoContent();
    }
}

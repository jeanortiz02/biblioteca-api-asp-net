
using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers.V2;

[ApiController]
[Route("api/v2/autores-collection")]
[Authorize(Policy = "esadmin")]
public class AutoresCollectionController : ControllerBase
{
    private readonly AplicationDbContext context;
    private readonly IMapper mapper;

    public AutoresCollectionController(AplicationDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet("{ids}", Name = "ObtenerAutoresPorIdsV2")] // api/autores-collection/1,2,3
    public async Task<ActionResult<List <AutorConLibrosDTO> > > Get(string ids)
    {
        var idsColeccion = new List<int>();

        foreach (var id in ids.Split(","))
        {
            if (int.TryParse(id, out int idInt))
            {
                idsColeccion.Add(idInt);
            }

        }
        if (!idsColeccion.Any())
        {
            ModelState.AddModelError(nameof(ids), "Ningun Id fue encontrado");
        }

        var autores = await context.Autores
            .Include(x => x.Libros)
            .ThenInclude(x => x.Libro)
            .Where(x => idsColeccion.Contains(x.Id)).ToListAsync();

        if (autores.Count != idsColeccion.Count)
        {
            return NotFound();
        }
        
        var autoresDto = mapper.Map<List<AutorConLibrosDTO>>(autores);
        return Ok(autoresDto);
    }

    [HttpPost]
    public async Task<ActionResult> Post(IEnumerable<AutorCreacionDTO> autoresCreacionDTOs)
    {
        var autores = mapper.Map<IEnumerable<Autor>>(autoresCreacionDTOs);
        context.AddRange(autores);
        await context.SaveChangesAsync();
        var autoresDTO = mapper.Map<IEnumerable<AutorDTO>>(autores);
        var ids = autores.Select( x => x.Id);
        var idsString = string.Join(",", ids);
        return CreatedAtRoute("ObtenerAutoresPorIdsV2", new { ids = idsString }, autoresDTO);
    }
}

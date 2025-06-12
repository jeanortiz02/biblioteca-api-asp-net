using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers;

[ApiController]
[Route("api/autores")]
[Authorize(Policy = "esadmin")]
public class AutoresController : ControllerBase
{
    private readonly AplicationDbContext context;
    private readonly IMapper mapper;

    public AutoresController(AplicationDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }


    // [HttpGet("/listado-de-autores")] // Ruta personalizada independiente
    [HttpGet]
    [AllowAnonymous]
    public async Task<IEnumerable<AutoresDTO>> Get()
    {
        var autores = await context.Autores.ToListAsync();
        var autoresDto = mapper.Map<IEnumerable<AutoresDTO>>(autores); // <IEnumerable<AutoresDTO>>

        return autoresDto;
    }

    [HttpGet("{id:int}", Name = "ObtenerAutor")] // api/autores/id
    public async Task<ActionResult<AutorConLibrosDTO>> Get( int id)
    {
        var autor = await context.Autores
                        .Include(x => x.Libros)
                        .ThenInclude(x => x.Libro)
                        .FirstOrDefaultAsync(x => x.Id == id);

        if (autor is null)
        {
            return NotFound();
        }

        var autorDto = mapper.Map<AutorConLibrosDTO>(autor);
        return autorDto;
    }

    // Dos parametros en ruta
    
    [HttpPost]
    public async Task<ActionResult> Post( AutorCreacionDTO autorCreacionDTO)
    {
        var autor = mapper.Map<Autor>(autorCreacionDTO);
        context.Add(autor);
        await context.SaveChangesAsync();
        var autorDto = mapper.Map<AutoresDTO>(autor);
        return CreatedAtRoute("ObtenerAutor", new { id = autor.Id }, autorDto);
    }

    [HttpPut("{id:int}")] // api/autores/id 
    public async Task<ActionResult> Put(int id, AutorCreacionDTO autorCreacionDTO)
    {
        var autor = mapper.Map<Autor>(autorCreacionDTO);
        autor.Id = id;

        var existeAutor = await context.Autores.AnyAsync(x => x.Id == autor.Id);
        
        if (!existeAutor)
        {
            return BadRequest($"El autor del libro {autor.Id} no existe");
        }
        context.Update(autor);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<AutorPatchDTO> patchDoc)
    {
        if (patchDoc is null)
        {
            return BadRequest();
        }
        var autorDb = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

        if (autorDb is null)
        {
            return NotFound();
        }

        var autorToPatch = mapper.Map<AutorPatchDTO>(autorDb);
        patchDoc.ApplyTo(autorToPatch, ModelState);

        var isValid = TryValidateModel(autorToPatch);

        if (!isValid)
        {
            return ValidationProblem();
        }


        mapper.Map(autorToPatch, autorDb); // Aplica los cambios
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var registroBorrados = await context.Autores.Where(x => x.Id == id).ExecuteDeleteAsync();

        if (registroBorrados == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

}

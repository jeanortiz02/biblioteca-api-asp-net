using System;
using System.Collections;
using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers;


[ApiController]
[Route("api/libros")]
public class LibrosController : ControllerBase
{
    private readonly AplicationDbContext context;
    private readonly IMapper mapper;

    public LibrosController(AplicationDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }


    [HttpGet]
    public async Task<IEnumerable<LibroDTO>> Get()
    {
        var libros = await context.Libros.ToListAsync();
        var librosDTOs = mapper.Map<IEnumerable<LibroDTO>>(libros);
        return librosDTOs;
    }

    [HttpGet("{id:int}", Name = "ObtenerLibro")]
    public async Task<ActionResult<LibroConAutorDTO>> Get(int id)
    {
        var libro = await context.Libros
                    .Include( x => x.Autores )
                    .FirstOrDefaultAsync(x => x.Id == id);

        if (libro is null)
        {
            return BadRequest("Libro no registrado");
        }

        var libroDTO = mapper.Map<LibroConAutorDTO>(libro);

        return libroDTO;
    }

    // [HttpPost]
    // public async Task<ActionResult> Post(LibroCreactionDTO libroCreactionDTO)
    // {
    //     var libro = mapper.Map<Libro>(libroCreactionDTO);

    //     var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

    //     if (!existeAutor)
    //     {
    //         ModelState.AddModelError(nameof(libro.AutorId), $"El autor del libro {libro.AutorId} no existe");
    //         return ValidationProblem();
    //     }

    //     context.Add(libro);
    //     await context.SaveChangesAsync();

    //     var libroDTO = mapper.Map<LibroDTO>(libro);

    //     return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDTO);
    // }

    // [HttpPut("{id:int}")]
    // public async Task<ActionResult> Put(int id, LibroCreactionDTO libroCreactionDTO)
    // {
    //     var libro = mapper.Map<Libro>(libroCreactionDTO);
    //     libro.Id = id;

    //     var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

    //     if (!existeAutor)
    //     {
    //         return BadRequest($"El autor del libro {libro.AutorId} no existe");
    //     }

    //     context.Update(libro);
    //     await context.SaveChangesAsync();
    //     return NoContent();
    // }

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

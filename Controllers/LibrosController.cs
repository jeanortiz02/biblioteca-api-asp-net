using System;
using System.Collections;
using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class LibrosController : ControllerBase
{
    private readonly AplicationDbContext context;

    public LibrosController(AplicationDbContext context)
    {
        this.context = context;
    }


    [HttpGet]
    public async Task<IEnumerable<Libro>> Get()
    {
        return await context.Libros.ToListAsync();
    }

    [HttpGet("{id:int}", Name = "ObtenerLibro")]
    public async Task<ActionResult<Libro>> Get(int id)
    {
        var libro = await context.Libros
                    .Include( x => x.Autor )
                    .FirstOrDefaultAsync(x => x.Id == id);

        if (libro is null)
        {
            return BadRequest("Libro no registrado");
        }

        return libro;
    }

    [HttpPost]
    public async Task<ActionResult> Post(Libro libro)
    {

        var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

        if (!existeAutor)
        {
            ModelState.AddModelError(nameof(libro.AutorId), $"El autor del libro {libro.AutorId} no existe");
            return ValidationProblem();
        }

        context.Add(libro);
        await context.SaveChangesAsync();
        return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libro);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, Libro libro)
    {
        if (id != libro.Id)
        {
            return BadRequest("Los Ids no coinciden");
        }

        var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

        if (!existeAutor)
        {
            return BadRequest($"El autor del libro {libro.AutorId} no existe");
        }

        context.Update(libro);
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var registroBorrados = await context.Autores.Where(x => x.Id == id).ExecuteDeleteAsync();

        if (registroBorrados == 0)
        {
            return NotFound();
        }

        return Ok();
    }
}

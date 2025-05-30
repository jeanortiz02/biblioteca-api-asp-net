using System;
using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BibliotecaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutoresController : ControllerBase
{
    private readonly AplicationDbContext context;

    public AutoresController(AplicationDbContext context)
    {
        this.context = context;
    }


    // [HttpGet("/listado-de-autores")] // Ruta personalizada independiente
    [HttpGet]
    public async Task<IEnumerable<Autor>> Get()
    {
        return await context.Autores.ToListAsync();
    }

    // [HttpGet("primero")] // api/autores/primero
    // public async Task<ActionResult<Autor>> GetFirstAutor()
    // {
    //     return await context.Autores.FirstAsync();
    // }

    [HttpGet("{id:int}")] // api/autores/id
    public async Task<ActionResult<Autor>> Get(int id)
    {
        var autor = await context.Autores
                        // .Include( x => x.Libros)
                        .FirstOrDefaultAsync(x => x.Id == id);

        if (autor is null)
        {
            return NotFound();
        }
        return autor;
    }
    [HttpGet("{nombre:alpha}")]
    public async Task<IEnumerable<Autor>> Get(string nombre)
    {
        return await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();
    }

    // Dos parametros en ruta
    [HttpGet("{parametro1}/{parametro2}")] // api/autores/primero/segundo
    public ActionResult Get(string parametro1, string parametro2)
    {
        return Ok(new { parametro2, parametro1 });
    }

    [HttpPost]
    public async Task<ActionResult> Post(Autor autor)
    {
        context.Add(autor);
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id:int}")] // api/autores/id 
    public async Task<ActionResult> Put(int id, Autor autor)
    {
        if (id != autor.Id)
        {
            return BadRequest("Los Ids deben coincidir");
        }

        context.Update(autor);
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

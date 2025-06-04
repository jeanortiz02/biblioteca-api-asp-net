using System;
using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
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
    private readonly IMapper mapper;

    public AutoresController(AplicationDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }


    // [HttpGet("/listado-de-autores")] // Ruta personalizada independiente
    [HttpGet]
    public async Task<IEnumerable<AutoresDTO>> Get()
    {
        var autores = await context.Autores.ToListAsync();
        var autoresDto = mapper.Map<IEnumerable<AutoresDTO>>(autores); // <IEnumerable<AutoresDTO>>
        
        return autoresDto;
    }

    [HttpGet("{id:int}", Name = "ObtenerAutor")] // api/autores/id
    public async Task<ActionResult<AutoresDTO>> Get( int id)
    {
        var autor = await context.Autores
                        .Include( x => x.Libros)
                        .FirstOrDefaultAsync(x => x.Id == id);

        if (autor is null)
        {
            return NotFound();
        }

        var autorDto = mapper.Map<AutoresDTO>(autor);
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

using System;
using System.Collections;
using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers;


[ApiController]
[Route("api/libros")]
[Authorize]
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

    [HttpPost]
    public async Task<ActionResult> Post(LibroCreactionDTO libroCreactionDTO)
    {
        if (libroCreactionDTO.AutoresId is null || libroCreactionDTO.AutoresId.Count == 0)
        {
            ModelState.AddModelError(nameof(libroCreactionDTO.AutoresId), "No se puede crear un libro sin autores");
            return ValidationProblem();
        }

        var autoresIdsExisten = await context.Autores
                                .Where(x => libroCreactionDTO.AutoresId.Contains(x.Id))
                                .Select( x => x.Id ).ToListAsync();

        if (libroCreactionDTO.AutoresId.Count != autoresIdsExisten.Count)
        {
            var autoresNoExisten = libroCreactionDTO.AutoresId.Except(autoresIdsExisten);
            var autoresNoExistenString = string.Join(", ", autoresNoExisten); // "1, 2, 3"
            var mensajedeError = "Los autores con ids: " + autoresNoExistenString + " no existen";
            ModelState.AddModelError(nameof(libroCreactionDTO.AutoresId), mensajedeError);
            return ValidationProblem();
        }

        var libro = mapper.Map<Libro>(libroCreactionDTO);
        AsignarOrdenAutores(libro);
        context.Add(libro);
        await context.SaveChangesAsync();

        var libroDTO = mapper.Map<LibroDTO>(libro);

        return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDTO);
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

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, LibroCreactionDTO libroCreactionDTO)
    {
        if (libroCreactionDTO.AutoresId is null || libroCreactionDTO.AutoresId.Count == 0)
        {
            ModelState.AddModelError(nameof(libroCreactionDTO.AutoresId), "No se puede crear un libro sin autores");
            return ValidationProblem();
        }

        var autoresIdsExisten = await context.Autores
                                .Where(x => libroCreactionDTO.AutoresId.Contains(x.Id))
                                .Select( x => x.Id ).ToListAsync();

        if (libroCreactionDTO.AutoresId.Count != autoresIdsExisten.Count)
        {
            var autoresNoExisten = libroCreactionDTO.AutoresId.Except(autoresIdsExisten);
            var autoresNoExistenString = string.Join(", ", autoresNoExisten); // "1, 2, 3"
            var mensajedeError = "Los autores con ids: " + autoresNoExistenString + " no existen";
            ModelState.AddModelError(nameof(libroCreactionDTO.AutoresId), mensajedeError);
            return ValidationProblem();
        }

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

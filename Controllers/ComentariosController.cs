using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;

        public ComentariosController(AplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {

            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentarios = await context.Comentarios.
                    Where(x => x.LibroId == libroId)
                    .OrderByDescending(x => x.FechaPublicacion)
                    .ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id}", Name = "obtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> Get(Guid id)
        {

            var comentario = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

            if (comentario is null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost]
        public async Task<ActionResult<ComentarioDTO>> Post(int libroId, ComentarioCreationDTO comentarioCreationDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreationDTO);
            comentario.LibroId = libroId;
            comentario.FechaPublicacion = DateTime.UtcNow;

            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("obtenerComentario", new { libroId, id = comentario.Id }, comentarioDTO);

        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id, int libroId, JsonPatchDocument<ComentarioPatchDTO> patchDoc)
        {

            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            if (patchDoc is null)
            {
                return BadRequest();
            }
            var comentarioDb = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

            if (comentarioDb is null)
            {
                return NotFound();
            }

            var comentariosPatchDTO = mapper.Map<ComentarioPatchDTO>(comentarioDb);
            patchDoc.ApplyTo(comentariosPatchDTO, ModelState);

            var isValid = TryValidateModel(comentariosPatchDTO);

            if (!isValid)
            {
                return ValidationProblem();
            }


            mapper.Map(comentariosPatchDTO, comentarioDb); // Aplica los cambios
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, int libroId)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var registrosBorrados = await context.Comentarios.Where(x => x.Id == id).ExecuteDeleteAsync();

            if (registrosBorrados == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

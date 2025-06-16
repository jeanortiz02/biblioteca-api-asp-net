using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    [Authorize]
    public class ComentariosController : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IServiciosUsuarios serviciosUsuarios;

        public ComentariosController(AplicationDbContext context, IMapper mapper, IServiciosUsuarios serviciosUsuarios)
        {
            this.context = context;
            this.mapper = mapper;
            this.serviciosUsuarios = serviciosUsuarios;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {

            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentarios = await context.Comentarios
                    .Include(x => x.Usuario)
                    .Where(x => x.LibroId == libroId)
                    .OrderByDescending(x => x.FechaPublicacion)
                    .ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id}", Name = "obtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> Get(Guid id)
        {

            var comentario = await context.Comentarios
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == id);

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

            var usuario = await serviciosUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreationDTO);
            comentario.LibroId = libroId;
            comentario.FechaPublicacion = DateTime.UtcNow;
            comentario.UsuarioId = usuario.Id;

            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("obtenerComentario", new { libroId, id = comentario.Id }, comentarioDTO);

        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id, int libroId, JsonPatchDocument<ComentarioPatchDTO> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest();
            }

            var usuario = await serviciosUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return NotFound();
            }


            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }
            var comentarioDb = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

            if (comentarioDb is null)
            {
                return NotFound();
            }

            if (comentarioDb.UsuarioId != usuario.Id)
            {
                return Forbid();
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

            var usuario = await serviciosUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return NotFound();
            }

            var comentarioDb = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

            if (comentarioDb is null)
            {
                return NotFound();
            }

            if (comentarioDb.UsuarioId != usuario.Id)
            {
                return Forbid();
            }

            context.Remove(comentarioDb);
            await context.SaveChangesAsync();

                return NoContent();
        }
    }
}

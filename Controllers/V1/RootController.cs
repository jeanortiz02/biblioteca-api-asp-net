using BibliotecaAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers.V1;

[ApiController]
[Route("api/v1")]
[Authorize]
public class RootController : ControllerBase
{
    private readonly IAuthorizationService authorizationService;


    public RootController(IAuthorizationService authorizationService)
    {
        this.authorizationService = authorizationService;
    }


    [HttpGet(Name = "ObtenerRootV1")]
    [AllowAnonymous]
    public async Task<IEnumerable<DatosHATEOASDTO>> Get()
    {
        var datosHATEOAS = new List<DatosHATEOASDTO>();
        var esAdmin = await authorizationService.AuthorizeAsync(User, "esadmin");

        // Acciones HATEOAS para todos los usuarios
        datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerRootV1", new { })!, Descripcion: "self", Metodo: "GET"));
        datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerAutoresV1", new { })!, Descripcion: "Autores", Metodo: "GET"));
        datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerLibrosV1", new { })!, Descripcion: "Libros", Metodo: "GET"));
        datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerUsuariosV1", new { })!, Descripcion: "Usuarios", Metodo: "GET"));
        datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("LoginUsuarioV1", new { })!, Descripcion: "Login Usuario", Metodo: "POST"));
        datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerComentariosV1", new { })!, Descripcion: "Comentarios", Metodo: "GET"));
        datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerAutoresPorIdsV1", new { })!, Descripcion: "Obtener Autores por Ids", Metodo: "GET"));
        datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerLibroV1", new { id = 0 })!, Descripcion: "Obtener Libro por Id", Metodo: "GET"));
        datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerAutorV1", new { id = 0 })!, Descripcion: "Obtener Autor por Id", Metodo: "GET"));
        datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerUsuarioV1", new { id = 0 })!, Descripcion: "Obtener Usuario por Id", Metodo: "GET"));
        datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("RegistrarUsuarioV1", new { })!, Descripcion: "Registrar Usuario", Metodo: "POST"));
        datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerComentarioV1", new { libroId = 0, id = 0 })!, Descripcion: "Obtener Comentario por Id", Metodo: "GET"));


        // Usuarios autenticados
        if (User.Identity!.IsAuthenticated)
        {
            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ActualizarComentarioV1", new { libroId = 0, id = 0 })!, Descripcion: "Actualizar Comentario", Metodo: "PUT"));
            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("CrearComentarioV1", new { })!, Descripcion: "Crear Comentario", Metodo: "POST"));
            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("CrearLibroV1", new { })!, Descripcion: "Crear Libro", Metodo: "POST"));
            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("EliminarComentarioV1", new { libroId = 0, id = 0 })!, Descripcion: "Eliminar Comentario", Metodo: "DELETE"));
            
        }


        // Acciones HATEOAS para administradores
        if (esAdmin.Succeeded)
        {
            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("CrearAutorV1", new { })!, Descripcion: "Crear Autor", Metodo: "POST"));
            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("CrearAutoresV1", new { })!, Descripcion: "Crear Autores", Metodo: "POST"));
            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("RemoverAdminV1", new { })!, Descripcion: "Remover Admin", Metodo: "POST"));
            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("HacerAdminV1", new { })!, Descripcion: "Hacer Admin", Metodo: "POST"));
            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ActualizarUsuarioV1", new { })!, Descripcion: "Actualizar Usuario", Metodo: "PUT"));
            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("RenovarTokenV1", new { })!, Descripcion: "Renovar Token", Metodo: "GET"));

        }

        return datosHATEOAS;
    }
}

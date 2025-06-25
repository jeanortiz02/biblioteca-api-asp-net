

using BibliotecaAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BibliotecaAPI.Servicios.V1;



public class GeneradorEnlaces : IGeneradorEnlaces
{
    private readonly LinkGenerator linkGenerator;
    private readonly IAuthorizationService authorizationService;
    private readonly IHttpContextAccessor httpContextAccessor;

    public GeneradorEnlaces(LinkGenerator linkGenerator, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
    {
        this.linkGenerator = linkGenerator;
        this.authorizationService = authorizationService;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task GenerarEnlaces(AutorDTO autorDTO)
    {
        var usuario = httpContextAccessor.HttpContext!.User;
        var esAdmin = await authorizationService.AuthorizeAsync(usuario, "esadmin");
        GenerarEnlacesHATEOAS(autorDTO, esAdmin.Succeeded);
    }

    private void GenerarEnlacesHATEOAS(AutorDTO autor, bool esAdmin)
    {
        autor.Enlaces.Add(new DatosHATEOASDTO(
            Enlace: linkGenerator.GetPathByRouteValues(httpContextAccessor.HttpContext!, "ObtenerAutorV1", new { id = autor.Id })!,
            Descripcion: "self",
            Metodo: "GET"));

        if (esAdmin)
        {

            autor.Enlaces.Add(new DatosHATEOASDTO(
                    Enlace: linkGenerator.GetPathByRouteValues(httpContextAccessor.HttpContext!, "ActualizarAutorV1", new { id = autor.Id })!,
                    Descripcion: "autor-actualizar",
                    Metodo: "PUT"));

            autor.Enlaces.Add(new DatosHATEOASDTO(
                    Enlace: linkGenerator.GetPathByRouteValues(httpContextAccessor.HttpContext!, "ActualizarAutorPatchV1", new { id = autor.Id })!,
                    Descripcion: "autor-actualizar-patch",
                    Metodo: "PATCH"));
            autor.Enlaces.Add(new DatosHATEOASDTO(
                    Enlace: linkGenerator.GetPathByRouteValues(httpContextAccessor.HttpContext!, "EliminarAutorV1", new { id = autor.Id })!,
                    Descripcion: "autor-eliminar",
                    Metodo: "DELETE"));

            autor.Enlaces.Add(new DatosHATEOASDTO(
                    Enlace: linkGenerator.GetPathByRouteValues(httpContextAccessor.HttpContext!, "CrearAutorConFotoV1", new { })!,
                    Descripcion: "autor-crear-con-foto",
                    Metodo: "POST"));
        }


    }
}

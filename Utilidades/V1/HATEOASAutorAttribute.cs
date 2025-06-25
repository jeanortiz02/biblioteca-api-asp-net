

using BibliotecaAPI.DTOs;
using BibliotecaAPI.Servicios.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BibliotecaAPI.Utilidades.V1;

public class HATEOASAutorAttribute : HATEOASFilterAttribute
{
    private readonly IGeneradorEnlaces generadorEnlaces;

    public HATEOASAutorAttribute(IGeneradorEnlaces generadorEnlaces)
    {
        this.generadorEnlaces = generadorEnlaces;
    }

    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var incluirHATEOAS = DebeIncluirHATEOAS(context);

        if (!incluirHATEOAS)
        {
            await next();
            return;
        }

        var result = context.Result as ObjectResult;
        var modelo = result!.Value as AutorDTO ?? throw new ArgumentException("El resultado no es un AutorDTO");

        await generadorEnlaces.GenerarEnlaces(modelo);

        await next();
    }

    private bool DebeIncluirHATEOAS(FilterContext context)
    {
        // Implement your logic here or call the base method if available
        return true;
    }
}

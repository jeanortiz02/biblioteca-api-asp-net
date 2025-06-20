
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Utilidades;

public class FiltroValidacionLibro : IAsyncActionFilter
{
    private readonly AplicationDbContext dbContext;

    public FiltroValidacionLibro(AplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ActionArguments.TryGetValue("libroCreactionDTO", out var value) ||
            value is not LibroCreactionDTO libroCreactionDTO)
        {
            context.ModelState.AddModelError("libroCreactionDTO", "El objeto libroCreactionDTO no es valido");
            context.Result = context.ModelState.ConstruirProblemDetail();
            return;
        }
        // validations 
        if (libroCreactionDTO.AutoresId is null || libroCreactionDTO.AutoresId.Count == 0)
        {
            context. ModelState.AddModelError(nameof(libroCreactionDTO.AutoresId), "No se puede crear un libro sin autores");
             context.Result = context.ModelState.ConstruirProblemDetail();
            return;
        }

        var autoresIdsExisten = await dbContext.Autores
                                .Where(x => libroCreactionDTO.AutoresId.Contains(x.Id))
                                .Select(x => x.Id).ToListAsync();

        if (libroCreactionDTO.AutoresId.Count != autoresIdsExisten.Count)
        {
            var autoresNoExisten = libroCreactionDTO.AutoresId.Except(autoresIdsExisten);
            var autoresNoExistenString = string.Join(", ", autoresNoExisten); // "1, 2, 3"
            var mensajedeError = "Los autores con ids: " + autoresNoExistenString + " no existen";
            context.ModelState.AddModelError(nameof(libroCreactionDTO.AutoresId), mensajedeError);
            context.Result = context.ModelState.ConstruirProblemDetail();
            return;
        }


        await next();

    }
}

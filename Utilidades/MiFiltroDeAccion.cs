

using Microsoft.AspNetCore.Mvc.Filters;

namespace BibliotecaAPI.Utilidades;

public class MiFiltroDeAccion : IActionFilter
{
    private readonly ILogger<MiFiltroDeAccion> logger;

    public MiFiltroDeAccion(ILogger<MiFiltroDeAccion> logger)
    {
        this.logger = logger;
    }
    // Antes de la accion
    public void OnActionExecuting(ActionExecutingContext context)
    {
        logger.LogInformation("Antes de la accion");
    }

    // Despues de la accion
    public void OnActionExecuted(ActionExecutedContext context)
    {
        logger.LogInformation("Despues de la accion");
    }

}

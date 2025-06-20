
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BibliotecaAPI.Utilidades;

public class FiltroTiempoEjecucion : IAsyncActionFilter
{
    private readonly ILogger<FiltroTiempoEjecucion> logger;

    public FiltroTiempoEjecucion(ILogger<FiltroTiempoEjecucion> logger)
    {
        this.logger = logger;
    }


    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Antes de la ejecucion de la accion
        var stopWatch = Stopwatch.StartNew();
        logger.LogInformation($"Inicio Accion: {context.ActionDescriptor.DisplayName}");

        await next();

        // Despues de la ejecucion de la accion
        stopWatch.Stop();
        logger.LogInformation($"Fin Accion: {context.ActionDescriptor.DisplayName} - Tiempo de ejecucion: {stopWatch.ElapsedMilliseconds} ms");
    }
}

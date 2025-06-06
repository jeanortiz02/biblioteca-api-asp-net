using System;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers;

[ApiController]
[Route("api/configuraciones")]
public class ConfiguracionesController : ControllerBase
{
    private readonly IConfiguration configuration;

    public ConfiguracionesController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [HttpGet]
    public ActionResult<string> Get()
    {
        var option1 = configuration["apellidos"];
        var option2 = configuration.GetValue<string>("apellidos")!;
        return option2;
    }

    [HttpGet("secciones")]
    public ActionResult<string> GetSecciones()
    {
        var option1 = configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        var seccion = configuration.GetSection("ConnectionStrings")!;
        var option2 = seccion["DefaultConnection"]!;
        return option1;
    }
}

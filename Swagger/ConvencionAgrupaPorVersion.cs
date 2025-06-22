using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BibliotecaAPI.Swagger;

public class ConvencionAgrupaPorVersion : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        // Ejemplo: "Controller.v1"
        var namespaceDelControllador = controller.ControllerType.Namespace;
        var version = namespaceDelControllador!.Split('.').Last().ToLower();

        controller.ApiExplorer.GroupName = version;
    }
}

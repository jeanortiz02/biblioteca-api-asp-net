using System;

namespace BibliotecaAPI;

public class ServicioTransient
{
    public readonly Guid _id;
    public ServicioTransient()
    {
        _id = Guid.NewGuid();
    }

    public Guid ObtenerGuid => _id;
}
public class ServiciosScoped
{
    public readonly Guid _id;
    public ServiciosScoped()
    {
        _id = Guid.NewGuid();
    }

    public Guid ObtenerGuid => _id;
}
public class ServicioSingleton
{
    public readonly Guid _id;
    public ServicioSingleton()
    {
        _id = Guid.NewGuid();
    }

    public Guid ObtenerGuid => _id;
}



namespace BibliotecaAPI.Servicios;

public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
{
    private readonly IWebHostEnvironment env;
    private readonly IHttpContextAccessor contextAccessor;

    public AlmacenadorArchivosLocal(IWebHostEnvironment env, IHttpContextAccessor contextAccessor)
    {
        this.env = env;
        this.contextAccessor = contextAccessor;
    }
    public async Task<string> Almacenar(string contenedor, IFormFile archivo)
    {
        var extension = Path.GetExtension(archivo.FileName);
        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        string folder = Path.Combine(env.WebRootPath, contenedor);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string ruta = Path.Combine(folder, nombreArchivo);
        using (var ms = new MemoryStream())
        {
            await archivo.CopyToAsync(ms);
            var contenido = ms.ToArray();
            await File.WriteAllBytesAsync(ruta, contenido);
        }

        var url = $"{contextAccessor.HttpContext!.Request.Scheme}://{contextAccessor.HttpContext.Request.Host}";

        var urlArchivo = Path.Combine(url, contenedor, nombreArchivo).Replace("\\", "/");

        return urlArchivo;
    }

    public Task Borrar(string? ruta, string contenedor)
    {
        if (string.IsNullOrEmpty(ruta))
        {
            return Task.CompletedTask;
        }

        var nombreArchivo = Path.GetFileName(ruta);
        var directorioArchivo = Path.Combine(env.WebRootPath, contenedor, nombreArchivo);

        if ( File.Exists(directorioArchivo) )
        {
            File.Delete(directorioArchivo);
        }

        return Task.CompletedTask;
    }
}

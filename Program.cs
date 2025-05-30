using System.Text.Json.Serialization;
using BibliotecaAPI.Datos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Area services
builder.Services.AddControllers().AddJsonOptions(opciones =>
    opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
); // Habilita el uso de controladores
builder.Services.AddDbContext<AplicationDbContext>(optiones =>
    optiones.UseSqlServer("name=DefaultConnection"));

var app = builder.Build();

// Area de middleware
app.MapControllers(); // Mapea los controladores a las rutas

app.Run();

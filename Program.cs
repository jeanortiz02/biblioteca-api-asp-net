using System.Text.Json.Serialization;
using BibliotecaAPI;
using BibliotecaAPI.Datos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Area services

// Probando los diferentes tipos de servicios
builder.Services.AddTransient<ServicioTransient>();
builder.Services.AddScoped<ServiciosScoped>();
builder.Services.AddSingleton<ServicioSingleton>();


// builder.Services.AddTransient<IRepositorioValores, RepositorioValores>(); // Configuracion Inyeccion dependencia
builder.Services.AddSingleton<IRepositorioValores, RepositorioValoresOracle>(); // Configuracion Inyeccion dependencia
builder.Services.AddControllers().AddJsonOptions(opciones =>
    opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
); // Habilita el uso de controladores
builder.Services.AddDbContext<AplicationDbContext>(optiones =>
    optiones.UseSqlServer("name=DefaultConnection"));


var app = builder.Build();

// Area de middleware
app.MapControllers(); // Mapea los controladores a las rutas

app.Run();

using System.Text;
using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using BibliotecaAPI.Swagger;
using BibliotecaAPI.Utilidades;
using BibliotecaAPI.Utilidades.V1;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Area services

builder.Services.AddOutputCache(opciones =>
{
    opciones.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(15);
}); // Habilita la cache

// builder.Services.AddStackExchangeRedisOutputCache(optiones =>
// {
//     optiones.Configuration = builder.Configuration.GetConnectionString("redis")!;
// });


builder.Services.AddDataProtection(); // Habilita la proteccion de datos

var myAllowSpecificOrigins = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>();

builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy( optionsCors =>
    {
        optionsCors.WithOrigins(myAllowSpecificOrigins!)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("cantidad-total-registros");
    });
});


builder.Services.AddAutoMapper(typeof(Program)); // Configurar automapper
// Probando los diferentes tipos de servicios

builder.Services.AddControllers( opciones =>
{
    opciones.Conventions.Add(new ConvencionAgrupaPorVersion());
    
}).AddNewtonsoftJson(); // Habilita el uso de controladores

builder.Services.AddDbContext<AplicationDbContext>(optiones =>
    optiones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<AplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<Usuario>>();
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddTransient<IServiciosUsuarios, ServiciosUsuarios>();
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();
builder.Services.AddScoped<FiltroValidacionLibro>();
builder.Services.AddScoped<BibliotecaAPI.Servicios.V1.IServicioAutores, BibliotecaAPI.Servicios.V1.ServicioAutores>();
builder.Services.AddScoped<BibliotecaAPI.Servicios.V1.IGeneradorEnlaces, BibliotecaAPI.Servicios.V1.GeneradorEnlaces>();

builder.Services.AddScoped<HATEOASAutorAttribute>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

// Configuration of the policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("esadmin", policy => policy.RequireClaim("esadmin"));
});


builder.Services.AddSwaggerGen( opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Biblioteca API",
        Description = "Este es un web API para trabajar con datos de autores y libros",
        Version = "v1",
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
    opciones.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Biblioteca API",
        Description = "Este es un web API para trabajar con datos de autores y libros",
        Version = "v2",
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    // opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    // {
    //     {
    //         new OpenApiSecurityScheme
    //         {
    //             Reference = new OpenApiReference
    //             {
    //                 Type = ReferenceType.SecurityScheme,
    //                 Id = "Bearer"
    //             }
    //         },
    //         new string[] {}
    //     }
    // });
});

var app = builder.Build();

// Area de middleware
app.UseExceptionHandler(excepcionHandleApp => excepcionHandleApp.Run(async context =>
{
    var exceptionHandlerFuture = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFuture?.Error!;

    var error = new Error()
    {
        Id = Guid.NewGuid(),
        MensajeDeError = exception.Message,
        StackTrace = exception.StackTrace,
        Fecha = DateTime.UtcNow
    };
    var dbContext = context.RequestServices.GetRequiredService<AplicationDbContext>();
    dbContext.Add(error);
    await dbContext.SaveChangesAsync();
    await Results.InternalServerError(new { tipo = "error", mensaje = "Ocurrio un error en el servidor", estatus = 500 }).ExecuteAsync(context);
})); // Manejo de errores globales


app.UseSwagger(); // Servi documento de swagger
app.UseSwaggerUI( opciones =>
{
    opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "Biblioteca API v1");
    opciones.SwaggerEndpoint("/swagger/v2/swagger.json", "Biblioteca API v2");
 // Habilita la interfaz de usuario de swagger en la raiz del proyecto
}); // Interfaz de usuario para el documento de swagger

app.UseStaticFiles(); // Habilita el uso de archivos estaticos

app.UseCors(); // Habilitar cors
app.UseOutputCache(); // Habilita la cache
app.MapControllers(); // Mapea los controladores a las rutas

app.Run();

using System.Text;
using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Area services

builder.Services.AddDataProtection(); // Habilita la proteccion de datos

var myAllowSpecificOrigins = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>();

builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy( optionsCors =>
    {
        optionsCors.WithOrigins(myAllowSpecificOrigins!)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("mi-cabecera");
    });
});


builder.Services.AddAutoMapper(typeof(Program)); // Configurar automapper
// Probando los diferentes tipos de servicios

builder.Services.AddControllers().AddNewtonsoftJson(); // Habilita el uso de controladores

builder.Services.AddDbContext<AplicationDbContext>(optiones =>
    optiones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<AplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<Usuario>>();
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddTransient<IServiciosUsuarios, ServiciosUsuarios>();


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
    opciones.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Biblioteca API",
        Description = "Este es un web API para trabajar con datos de autores y libros",
        Version = "v1",
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
});

var app = builder.Build();

// Area de middleware

app.UseSwagger(); // Servi documento de swagger
app.UseSwaggerUI(); // Interfaz de usuario para el documento de swagger

app.UseCors(); // Habilitar cors
app.MapControllers(); // Mapea los controladores a las rutas

app.Run();

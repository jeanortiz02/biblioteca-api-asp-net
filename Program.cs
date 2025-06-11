using System.Text;
using System.Text.Json.Serialization;
using BibliotecaAPI;
using BibliotecaAPI.Datos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Area services
builder.Services.AddAutoMapper(typeof(Program)); // Configurar automapper
// Probando los diferentes tipos de servicios

builder.Services.AddControllers().AddNewtonsoftJson(); // Habilita el uso de controladores

builder.Services.AddDbContext<AplicationDbContext>(optiones =>
    optiones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<AplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<SignInManager<IdentityUser>>();
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

var app = builder.Build();

// Area de middleware

app.MapControllers(); // Mapea los controladores a las rutas

app.Run();

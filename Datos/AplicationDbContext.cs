using System;
using BibliotecaAPI.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Datos;

public class AplicationDbContext : DbContext
{
    public AplicationDbContext(DbContextOptions options) : base(options)
    {

    }

    // Crea una tabla en la base de datos
    // que se llamará Autores y contendrá los datos de la entidad Autor
    public DbSet<Autor> Autores { get; set; }

}

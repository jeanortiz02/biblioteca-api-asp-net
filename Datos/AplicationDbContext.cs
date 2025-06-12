using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Datos;

public class AplicationDbContext : IdentityDbContext<Usuario>
{
    public AplicationDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    // Crea una tabla en la base de datos
    // que se llamará Autores y contendrá los datos de la entidad Autor
    public DbSet<Autor> Autores { get; set; }
    public DbSet<Libro> Libros { get; set; }
    public DbSet<Comentario> Comentarios { get; set; }
    public DbSet<AutorLibro> AutoresLibros { get; set; }

}

using BibliotecaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Data;

public class BibliotecaContext : DbContext
{
    public BibliotecaContext(DbContextOptions<BibliotecaContext> options)
        : base(options)
    {
    }

    public DbSet<Socio> Socios => Set<Socio>();
    public DbSet<Autor> Autores => Set<Autor>();
    public DbSet<Libro> Libros => Set<Libro>();
    public DbSet<Prestamo> Prestamos => Set<Prestamo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Socio>(entity =>
        {
            entity.ToTable("Socio");
            entity.HasIndex(s => s.Email).IsUnique();
            entity.Property(s => s.FechaAlta).HasColumnType("date");
        });

        modelBuilder.Entity<Autor>(entity =>
        {
            entity.ToTable("Autor");
        });

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.ToTable("Libro", t => t.HasCheckConstraint("CK_Libro_Stock", "[Stock] >= 0"));

            // ESCRIBE: un autor tiene muchos libros; no se borra un autor con libros
            entity.HasOne(l => l.Autor)
                .WithMany(a => a.Libros)
                .HasForeignKey(l => l.IdAutor)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.ToTable("Prestamo", t => t.HasCheckConstraint(
                "CK_Prestamo_Fechas", "[FechaDevolucion] IS NULL OR [FechaDevolucion] >= [FechaPrestamo]"));
            entity.Property(p => p.FechaPrestamo).HasColumnType("date");
            entity.Property(p => p.FechaDevolucion).HasColumnType("date");

            // REALIZA: un socio realiza muchos préstamos
            entity.HasOne(p => p.Socio)
                .WithMany(s => s.Prestamos)
                .HasForeignKey(p => p.IdSocio)
                .OnDelete(DeleteBehavior.Restrict);

            // INCLUYE: un libro aparece en muchos préstamos
            entity.HasOne(p => p.Libro)
                .WithMany(l => l.Prestamos)
                .HasForeignKey(p => p.IdLibro)
                .OnDelete(DeleteBehavior.Restrict);
        });

        CargarDatosIniciales(modelBuilder);
    }

    private static void CargarDatosIniciales(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Autor>().HasData(
            new Autor { IdAutor = 1, Nombre = "Jorge Luis Borges", Nacionalidad = "Argentina" },
            new Autor { IdAutor = 2, Nombre = "Julio Cortázar", Nacionalidad = "Argentina" },
            new Autor { IdAutor = 3, Nombre = "Gabriel García Márquez", Nacionalidad = "Colombiana" },
            new Autor { IdAutor = 4, Nombre = "Isabel Allende", Nacionalidad = "Chilena" },
            new Autor { IdAutor = 5, Nombre = "José Hernández", Nacionalidad = "Argentina" });

        modelBuilder.Entity<Libro>().HasData(
            new Libro { IdLibro = 1, Titulo = "Ficciones", Genero = "Cuento", Stock = 3, IdAutor = 1 },
            new Libro { IdLibro = 2, Titulo = "El Aleph", Genero = "Cuento", Stock = 1, IdAutor = 1 },
            new Libro { IdLibro = 3, Titulo = "Rayuela", Genero = "Novela", Stock = 2, IdAutor = 2 },
            new Libro { IdLibro = 4, Titulo = "Bestiario", Genero = "Cuento", Stock = 0, IdAutor = 2 },
            new Libro { IdLibro = 5, Titulo = "Cien años de soledad", Genero = "Novela", Stock = 4, IdAutor = 3 },
            new Libro { IdLibro = 6, Titulo = "Crónica de una muerte anunciada", Genero = "Novela", Stock = 2, IdAutor = 3 },
            new Libro { IdLibro = 7, Titulo = "La casa de los espíritus", Genero = "Novela", Stock = 1, IdAutor = 4 },
            new Libro { IdLibro = 8, Titulo = "Martín Fierro", Genero = "Poesía", Stock = 5, IdAutor = 5 });

        modelBuilder.Entity<Socio>().HasData(
            new Socio { IdSocio = 1, Nombre = "Laura Gómez", Email = "laura.gomez@mail.com", FechaAlta = new DateTime(2026, 2, 10) },
            new Socio { IdSocio = 2, Nombre = "Martín Pérez", Email = "martin.perez@mail.com", FechaAlta = new DateTime(2026, 3, 5) },
            new Socio { IdSocio = 3, Nombre = "Sofía Díaz", Email = "sofia.diaz@mail.com", FechaAlta = new DateTime(2026, 4, 18) },
            new Socio { IdSocio = 4, Nombre = "Carlos Ruiz", Email = "carlos.ruiz@mail.com", FechaAlta = new DateTime(2026, 6, 1) },
            new Socio { IdSocio = 5, Nombre = "Ana Torres", Email = "ana.torres@mail.com", FechaAlta = new DateTime(2026, 8, 22) });

        // Los préstamos sin fecha de devolución ya están descontados del stock de arriba
        modelBuilder.Entity<Prestamo>().HasData(
            new Prestamo { IdPrestamo = 1, IdSocio = 1, IdLibro = 1, FechaPrestamo = new DateTime(2026, 3, 2), FechaDevolucion = new DateTime(2026, 3, 16) },
            new Prestamo { IdPrestamo = 2, IdSocio = 2, IdLibro = 3, FechaPrestamo = new DateTime(2026, 4, 10), FechaDevolucion = new DateTime(2026, 4, 28) },
            new Prestamo { IdPrestamo = 3, IdSocio = 1, IdLibro = 5, FechaPrestamo = new DateTime(2026, 5, 7), FechaDevolucion = new DateTime(2026, 5, 20) },
            new Prestamo { IdPrestamo = 4, IdSocio = 3, IdLibro = 1, FechaPrestamo = new DateTime(2026, 6, 12), FechaDevolucion = new DateTime(2026, 6, 30) },
            new Prestamo { IdPrestamo = 5, IdSocio = 4, IdLibro = 8, FechaPrestamo = new DateTime(2026, 7, 3), FechaDevolucion = new DateTime(2026, 7, 15) },
            new Prestamo { IdPrestamo = 6, IdSocio = 2, IdLibro = 5, FechaPrestamo = new DateTime(2026, 8, 19), FechaDevolucion = new DateTime(2026, 9, 2) },
            new Prestamo { IdPrestamo = 7, IdSocio = 5, IdLibro = 2, FechaPrestamo = new DateTime(2026, 9, 8), FechaDevolucion = new DateTime(2026, 9, 25) },
            new Prestamo { IdPrestamo = 8, IdSocio = 3, IdLibro = 4, FechaPrestamo = new DateTime(2026, 9, 20) },
            new Prestamo { IdPrestamo = 9, IdSocio = 1, IdLibro = 3, FechaPrestamo = new DateTime(2026, 10, 1) },
            new Prestamo { IdPrestamo = 10, IdSocio = 4, IdLibro = 7, FechaPrestamo = new DateTime(2026, 10, 14) },
            new Prestamo { IdPrestamo = 11, IdSocio = 5, IdLibro = 5, FechaPrestamo = new DateTime(2026, 10, 20) });
    }
}

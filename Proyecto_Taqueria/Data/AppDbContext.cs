using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Proyecto_Taqueria.Data.Modelos;

namespace Proyecto_Taqueria.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; } // Tabla "Productos"

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tu compañero agregará configuraciones adicionales aquí si son necesarias
    }
}

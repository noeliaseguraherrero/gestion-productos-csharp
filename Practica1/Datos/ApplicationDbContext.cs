using Microsoft.EntityFrameworkCore;
using Practica1.Modelos;

namespace Practica1.Datos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Producto { get; set; } // <--- Esto permite el _context.Producto
    }
}


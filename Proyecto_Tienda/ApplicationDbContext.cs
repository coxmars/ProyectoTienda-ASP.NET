
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proyecto_Tienda.Entities;

namespace Proyecto_Tienda
{
    public class ApplicationDbContext : IdentityDbContext {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {

        }

        // Usando la API fluida (el recurso más poderoso para configurar)
        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            // Establecer propiedades de tabla para atributos

        }

        // Esto muestra que Producto es una entidad, use Dbset porque
        // queremos crear una tabla a partir de la clase Producto y el
        // nombre de esta tabla de base de datos será Productos
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
    }
}

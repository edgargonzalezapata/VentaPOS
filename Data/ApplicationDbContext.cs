using Microsoft.EntityFrameworkCore;
using VentaPOS.Models;

namespace VentaPOS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.CategoriaID);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.EmpresaRut).IsRequired();

                entity.HasOne(d => d.Empresa)
                    .WithMany()
                    .HasForeignKey(d => d.EmpresaRut)
                    .HasConstraintName("FK_Categorias_Empresas");
            });
        }
    }
} 
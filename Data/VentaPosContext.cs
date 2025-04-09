using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VentaPOS.Models;

namespace VentaPOS.Data;

public partial class VentaPosContext : DbContext
{
    public VentaPosContext()
    {
    }

    public VentaPosContext(DbContextOptions<VentaPosContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; } = null!;

    public virtual DbSet<Cliente> Clientes { get; set; } = null!;

    public virtual DbSet<ConfiguracionEmpresa> ConfiguracionEmpresas { get; set; } = null!;

    public virtual DbSet<DetallesVenta> DetallesVenta { get; set; } = null!;

    public virtual DbSet<Empresa> Empresas { get; set; } = null!;

    public virtual DbSet<Inventario> Inventarios { get; set; } = null!;

    public virtual DbSet<Planes> Planes { get; set; } = null!;

    public virtual DbSet<Producto> Productos { get; set; } = null!;

    public virtual DbSet<Roles> Roles { get; set; } = null!;

    public virtual DbSet<Suscripciones> Suscripciones { get; set; } = null!;

    public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

    public virtual DbSet<Venta> Ventas { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // La cadena de conexión se configura en Program.cs
        // Solo configurar si no está ya configurado (para permitir migrations desde línea de comandos)
        if (!optionsBuilder.IsConfigured)
        {
            // Configuración opcional para herramientas de EF Core si se ejecuta fuera de la aplicación
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Planes>(entity =>
        {
            entity.HasKey(e => e.PlanID);
            entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Precio).HasColumnType("int");
        });

        modelBuilder.Entity<Suscripciones>(entity =>
        {
            entity.HasKey(e => e.SuscripcionID);

            entity.HasOne(d => d.Plan)
                .WithMany(p => p.Suscripciones)
                .HasForeignKey(d => d.PlanID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Suscripciones_Planes");

            entity.HasOne(d => d.Empresa)
                .WithMany(e => e.Suscripciones)
                .HasForeignKey(d => d.EmpresaRut)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Suscripciones_Empresas");

            entity.HasMany(s => s.Usuarios)
                .WithMany(u => u.Suscripciones)
                .UsingEntity<Dictionary<string, object>>(
                    "UsuariosSuscripciones",
                    l => l.HasOne<Usuario>().WithMany().HasForeignKey("UsuarioID"),
                    r => r.HasOne<Suscripciones>().WithMany().HasForeignKey("SuscripcionID"),
                    j => j.HasKey("UsuarioID", "SuscripcionID")
                );
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaID).HasName("PK__Categori__F353C1C53DC7F524");

            entity.HasIndex(e => e.Nombre, "UQ__Categori__75E3EFCF34D4F80D").IsUnique();

            entity.Property(e => e.CategoriaID).HasColumnName("CategoriaID");
            entity.Property(e => e.Descripcion).HasColumnType("text");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Empresa).WithMany()
                .HasForeignKey(d => d.EmpresaRut)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categorias_Empresas");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId).HasName("PK__Clientes__71ABD0A71131B070");

            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UltimaCompra).HasColumnType("datetime");

            entity.HasOne(d => d.Empresa).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.EmpresaRut)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clientes_Empresas");
        });

        modelBuilder.Entity<ConfiguracionEmpresa>(entity =>
        {
            entity.HasKey(e => e.ConfiguracionId).HasName("PK__Configur__9B95E0563E7C30A7");

            entity.ToTable("ConfiguracionEmpresa");

            entity.Property(e => e.ConfiguracionId).HasColumnName("ConfiguracionID");
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Valor)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Empresa).WithMany(p => p.ConfiguracionEmpresas)
                .HasForeignKey(d => d.EmpresaRut)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConfiguracionEmpresa_Empresas");
        });

        modelBuilder.Entity<DetallesVenta>(entity =>
        {
            entity.HasKey(e => e.DetalleID);

            entity.ToTable("DetallesVenta");

            entity.Property(e => e.DetalleID)
                .HasColumnName("DetalleID")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(18,2)");

            // Relaciones
            entity.HasOne(d => d.Venta)
                .WithMany(v => v.DetallesVentas)
                .HasForeignKey(d => d.VentaID);

            entity.HasOne(d => d.Producto)
                .WithMany(p => p.DetallesVentas)
                .HasForeignKey(d => d.ProductoID)
                .HasConstraintName("FK_DetallesVenta_Productos");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.Rut);

            entity.Property(e => e.Rut)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Direccion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreEmpresa)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.InventarioId).HasName("PK__Inventar__FB8A24B78F5A075C");

            entity.ToTable("Inventario");

            entity.Property(e => e.InventarioId).HasColumnName("InventarioID");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProductoID).HasColumnName("ProductoID");
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Producto).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.ProductoID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_Productos");

            entity.HasOne(d => d.Empresa).WithMany()
                .HasForeignKey(d => d.EmpresaRut)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_Empresas");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoID).HasName("PK__Producto__A430AE832C367D1B");

            entity.Property(e => e.ProductoID).HasColumnName("ProductoID");
            entity.Property(e => e.CategoriaID).HasColumnName("CategoriaID");
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion).HasColumnType("text");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UltimaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_Categorias");

            entity.HasOne(d => d.Empresa).WithMany()
                .HasForeignKey(d => d.EmpresaRut)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_Empresas");
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__Roles__F92302D15CC78A2C");

            entity.Property(e => e.RolId).HasColumnName("RolID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasIndex(e => new { e.Nombre, e.EmpresaRut })
                .HasDatabaseName("UQ__Roles__NombreEmpresa")
                .IsUnique();

            entity.HasOne(d => d.Empresa).WithMany()
                .HasForeignKey(d => d.EmpresaRut)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Roles_Empresas");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE798FCB1AA72");

            entity.HasIndex(e => e.Correo, "UQ__Usuarios__60695A19E6A6D41B").IsUnique();

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmpresaRut).HasColumnName("EmpresaRut");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Empresa)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.EmpresaRut)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Empresas");

            entity.HasMany(d => d.Rols).WithMany(p => p.Usuarios)
                .UsingEntity<Dictionary<string, object>>(
                    "UsuarioRole",
                    r => r.HasOne<Roles>().WithMany()
                        .HasForeignKey("RolID")
                        .HasConstraintName("FK_UsuarioRoles_Roles"),
                    l => l.HasOne<Usuario>().WithMany()
                        .HasForeignKey("UsuarioID")
                        .HasConstraintName("FK_UsuarioRoles_Usuarios"),
                    j => {
                        j.HasKey("UsuarioID", "RolID");
                        j.ToTable("UsuarioRoles");
                    });

            entity.HasMany(d => d.Suscripciones).WithMany(p => p.Usuarios)
                .UsingEntity<Dictionary<string, object>>(
                    "UsuariosSuscripciones",
                    j => j.HasOne<Suscripciones>().WithMany().HasForeignKey("SuscripcionID"),
                    j => j.HasOne<Usuario>().WithMany().HasForeignKey("UsuarioID"),
                    j => j.HasKey("UsuarioID", "SuscripcionID"));
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.ToTable("Ventas");

            entity.HasKey(e => e.VentaID);

            entity.Property(e => e.VentaID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.FechaVenta)
                .HasColumnType("datetime");

            entity.Property(e => e.MetodoPago)
                .HasMaxLength(50);

            entity.Property(e => e.Comentarios)
                .HasMaxLength(500);

            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Descuento)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Impuestos)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Total)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.NumeroFactura)
                .HasMaxLength(50);

            entity.Property(e => e.EmpresaRut)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasOne(d => d.Cliente)
                .WithMany()
                .HasForeignKey(d => d.ClienteID)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Usuario)
                .WithMany()
                .HasForeignKey(d => d.UsuarioID)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Empresa)
                .WithMany(e => e.Venta)
                .HasForeignKey(d => d.EmpresaRut)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasMany(v => v.DetallesVentas)
                .WithOne(d => d.Venta)
                .HasForeignKey(d => d.VentaID)
                .HasConstraintName("FK_DetallesVenta_Ventas");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

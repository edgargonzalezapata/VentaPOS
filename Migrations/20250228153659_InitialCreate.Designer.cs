﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VentaPOS.Data;

#nullable disable

namespace VentaPOS.Migrations
{
    [DbContext(typeof(VentaPosContext))]
    [Migration("20250228153659_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("UsuarioRole", b =>
                {
                    b.Property<int>("UsuarioId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioID");

                    b.Property<int>("RolId")
                        .HasColumnType("int")
                        .HasColumnName("RolID");

                    b.HasKey("UsuarioId", "RolId")
                        .HasName("PK__UsuarioR__24AFD7B554E7B9DF");

                    b.HasIndex("RolId");

                    b.ToTable("UsuarioRoles", (string)null);
                });

            modelBuilder.Entity("UsuariosSuscripciones", b =>
                {
                    b.Property<int>("UsuarioId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioID");

                    b.Property<int>("SuscripcionId")
                        .HasColumnType("int")
                        .HasColumnName("SuscripcionID");

                    b.HasKey("UsuarioId", "SuscripcionId")
                        .HasName("PK__Usuarios__832930F0783A500D");

                    b.HasIndex("SuscripcionId");

                    b.ToTable("UsuariosSuscripciones", (string)null);
                });

            modelBuilder.Entity("VentaPOS.Models.Categoria", b =>
                {
                    b.Property<int>("CategoriaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CategoriaID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoriaId"));

                    b.Property<string>("Descripcion")
                        .HasMaxLength(500)
                        .HasColumnType("text");

                    b.Property<string>("EmpresaRut")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("CategoriaId")
                        .HasName("PK__Categori__F353C1C53DC7F524");

                    b.HasIndex("EmpresaRut");

                    b.HasIndex(new[] { "Nombre" }, "UQ__Categori__75E3EFCF34D4F80D")
                        .IsUnique();

                    b.ToTable("Categorias");
                });

            modelBuilder.Entity("VentaPOS.Models.Cliente", b =>
                {
                    b.Property<int>("ClienteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ClienteID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ClienteId"));

                    b.Property<string>("Apellidos")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("EmpresaRut")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Telefono")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime?>("UltimaCompra")
                        .HasColumnType("datetime");

                    b.HasKey("ClienteId")
                        .HasName("PK__Clientes__71ABD0A71131B070");

                    b.HasIndex("EmpresaRut");

                    b.ToTable("Clientes");
                });

            modelBuilder.Entity("VentaPOS.Models.ConfiguracionEmpresa", b =>
                {
                    b.Property<int>("ConfiguracionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ConfiguracionID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ConfiguracionId"));

                    b.Property<string>("Clave")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Descripcion")
                        .HasMaxLength(200)
                        .IsUnicode(false)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("EmpresaRut")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Valor")
                        .IsRequired()
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.HasKey("ConfiguracionId")
                        .HasName("PK__Configur__9B95E0563E7C30A7");

                    b.HasIndex("EmpresaRut");

                    b.ToTable("ConfiguracionEmpresa", (string)null);
                });

            modelBuilder.Entity("VentaPOS.Models.DetallesVentum", b =>
                {
                    b.Property<int>("DetalleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("DetalleID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DetalleId"));

                    b.Property<int>("Cantidad")
                        .HasColumnType("int");

                    b.Property<decimal?>("Descuento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(10, 2)")
                        .HasDefaultValue(0m);

                    b.Property<decimal?>("Impuesto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(10, 2)")
                        .HasDefaultValue(0m);

                    b.Property<decimal>("PrecioUnitario")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("ProductoId")
                        .HasColumnType("int")
                        .HasColumnName("ProductoID");

                    b.Property<decimal>("Subtotal")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("VentaId")
                        .HasColumnType("int")
                        .HasColumnName("VentaID");

                    b.HasKey("DetalleId")
                        .HasName("PK__Detalles__6E19D6FA3E480FBF");

                    b.HasIndex("ProductoId");

                    b.HasIndex("VentaId");

                    b.ToTable("DetallesVenta");
                });

            modelBuilder.Entity("VentaPOS.Models.Empresa", b =>
                {
                    b.Property<string>("Rut")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.Property<bool?>("Activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<byte[]>("Contraseña")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Direccion")
                        .HasMaxLength(200)
                        .IsUnicode(false)
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("FechaRegistro")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("NombreEmpresa")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Telefono")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.HasKey("Rut");

                    b.ToTable("Empresas");
                });

            modelBuilder.Entity("VentaPOS.Models.Inventario", b =>
                {
                    b.Property<int>("InventarioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("InventarioID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InventarioId"));

                    b.Property<int>("Cantidad")
                        .HasColumnType("int");

                    b.Property<string>("EmpresaRut")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime?>("FechaActualizacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("ProductoId")
                        .HasColumnType("int")
                        .HasColumnName("ProductoID");

                    b.Property<string>("Ubicacion")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("InventarioId")
                        .HasName("PK__Inventar__FB8A24B78F5A075C");

                    b.HasIndex("EmpresaRut");

                    b.HasIndex("ProductoId");

                    b.ToTable("Inventario", (string)null);
                });

            modelBuilder.Entity("VentaPOS.Models.Plane", b =>
                {
                    b.Property<int>("PlanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("PlanID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PlanId"));

                    b.Property<int?>("MaxUsuarios")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("PlanId")
                        .HasName("PK__Planes__755C22D75E7A22C7");

                    b.HasIndex(new[] { "Nombre" }, "UQ__Planes__75E3EFCFA5B72F05")
                        .IsUnique();

                    b.ToTable("Planes");
                });

            modelBuilder.Entity("VentaPOS.Models.Producto", b =>
                {
                    b.Property<int>("ProductoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ProductoID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductoId"));

                    b.Property<int>("CategoriaId")
                        .HasColumnType("int")
                        .HasColumnName("CategoriaID");

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Descripcion")
                        .HasMaxLength(500)
                        .HasColumnType("text");

                    b.Property<string>("EmpresaRut")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("Precio")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UltimaActualizacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("ProductoId")
                        .HasName("PK__Producto__A430AE832C367D1B");

                    b.HasIndex("CategoriaId");

                    b.HasIndex("EmpresaRut");

                    b.ToTable("Productos");
                });

            modelBuilder.Entity("VentaPOS.Models.Role", b =>
                {
                    b.Property<int>("RolId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RolID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RolId"));

                    b.Property<string>("Descripcion")
                        .HasMaxLength(200)
                        .IsUnicode(false)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("EmpresaRut")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("RolId")
                        .HasName("PK__Roles__F92302D15CC78A2C");

                    b.HasIndex("EmpresaRut");

                    b.HasIndex(new[] { "Nombre" }, "UQ__Roles__75E3EFCF93AC4567")
                        .IsUnique();

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("VentaPOS.Models.Suscripciones", b =>
                {
                    b.Property<int>("SuscripcionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("SuscripcionID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SuscripcionId"));

                    b.Property<bool>("Activa")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("EmpresaRut")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("EmpresaRut");

                    b.Property<DateTime>("FechaFin")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("FechaInicio")
                        .HasColumnType("datetime");

                    b.Property<string>("FormaPago")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<int>("MaxUsuarios")
                        .HasColumnType("int");

                    b.Property<bool?>("Pagado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<int>("PlanId")
                        .HasColumnType("int")
                        .HasColumnName("PlanID");

                    b.HasKey("SuscripcionId")
                        .HasName("PK__Suscripc__814D768B4ED74F72");

                    b.HasIndex("EmpresaRut");

                    b.HasIndex("PlanId");

                    b.ToTable("Suscripciones");
                });

            modelBuilder.Entity("VentaPOS.Models.Usuario", b =>
                {
                    b.Property<int>("UsuarioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UsuarioID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UsuarioId"));

                    b.Property<bool>("Activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<byte[]>("Contraseña")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("EmpresaRut")
                        .IsRequired()
                        .HasColumnType("varchar(20)")
                        .HasColumnName("EmpresaRut");

                    b.Property<DateTime>("FechaRegistro")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Telefono")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.HasKey("UsuarioId")
                        .HasName("PK__Usuarios__2B3DE798FCB1AA72");

                    b.HasIndex("EmpresaRut");

                    b.HasIndex(new[] { "Correo" }, "UQ__Usuarios__60695A19E6A6D41B")
                        .IsUnique();

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("VentaPOS.Models.Venta", b =>
                {
                    b.Property<int>("VentaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("VentaID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VentaId"));

                    b.Property<int?>("ClienteId")
                        .HasColumnType("int")
                        .HasColumnName("ClienteID");

                    b.Property<string>("Comentarios")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.Property<decimal?>("Descuento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(10, 2)")
                        .HasDefaultValue(0m);

                    b.Property<string>("EmpresaRut")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime?>("FechaVenta")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<decimal?>("Impuestos")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(10, 2)")
                        .HasDefaultValue(0m);

                    b.Property<string>("MetodoPago")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("NumeroFactura")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<decimal>("Subtotal")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioID");

                    b.HasKey("VentaId")
                        .HasName("PK__Ventas__5B41514CCDF474A8");

                    b.HasIndex("ClienteId");

                    b.HasIndex("EmpresaRut");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Ventas");
                });

            modelBuilder.Entity("UsuarioRole", b =>
                {
                    b.HasOne("VentaPOS.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_UsuarioRoles_Roles");

                    b.HasOne("VentaPOS.Models.Usuario", null)
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_UsuarioRoles_Usuarios");
                });

            modelBuilder.Entity("UsuariosSuscripciones", b =>
                {
                    b.HasOne("VentaPOS.Models.Suscripciones", null)
                        .WithMany()
                        .HasForeignKey("SuscripcionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_UsuariosSuscripciones_Suscripciones");

                    b.HasOne("VentaPOS.Models.Usuario", null)
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_UsuariosSuscripciones_Usuarios");
                });

            modelBuilder.Entity("VentaPOS.Models.Categoria", b =>
                {
                    b.HasOne("VentaPOS.Models.Empresa", "Empresa")
                        .WithMany()
                        .HasForeignKey("EmpresaRut")
                        .IsRequired()
                        .HasConstraintName("FK_Categorias_Empresas");

                    b.Navigation("Empresa");
                });

            modelBuilder.Entity("VentaPOS.Models.Cliente", b =>
                {
                    b.HasOne("VentaPOS.Models.Empresa", "Empresa")
                        .WithMany("Clientes")
                        .HasForeignKey("EmpresaRut")
                        .IsRequired()
                        .HasConstraintName("FK_Clientes_Empresas");

                    b.Navigation("Empresa");
                });

            modelBuilder.Entity("VentaPOS.Models.ConfiguracionEmpresa", b =>
                {
                    b.HasOne("VentaPOS.Models.Empresa", "Empresa")
                        .WithMany("ConfiguracionEmpresas")
                        .HasForeignKey("EmpresaRut")
                        .IsRequired()
                        .HasConstraintName("FK_ConfiguracionEmpresa_Empresas");

                    b.Navigation("Empresa");
                });

            modelBuilder.Entity("VentaPOS.Models.DetallesVentum", b =>
                {
                    b.HasOne("VentaPOS.Models.Producto", "Producto")
                        .WithMany("DetallesVenta")
                        .HasForeignKey("ProductoId")
                        .IsRequired()
                        .HasConstraintName("FK_DetallesVenta_Productos");

                    b.HasOne("VentaPOS.Models.Venta", "Venta")
                        .WithMany("DetallesVenta")
                        .HasForeignKey("VentaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_DetallesVenta_Ventas");

                    b.Navigation("Producto");

                    b.Navigation("Venta");
                });

            modelBuilder.Entity("VentaPOS.Models.Inventario", b =>
                {
                    b.HasOne("VentaPOS.Models.Empresa", "Empresa")
                        .WithMany()
                        .HasForeignKey("EmpresaRut")
                        .IsRequired()
                        .HasConstraintName("FK_Inventario_Empresas");

                    b.HasOne("VentaPOS.Models.Producto", "Producto")
                        .WithMany("Inventarios")
                        .HasForeignKey("ProductoId")
                        .IsRequired()
                        .HasConstraintName("FK_Inventario_Productos");

                    b.Navigation("Empresa");

                    b.Navigation("Producto");
                });

            modelBuilder.Entity("VentaPOS.Models.Producto", b =>
                {
                    b.HasOne("VentaPOS.Models.Categoria", "Categoria")
                        .WithMany("Productos")
                        .HasForeignKey("CategoriaId")
                        .IsRequired()
                        .HasConstraintName("FK_Productos_Categorias");

                    b.HasOne("VentaPOS.Models.Empresa", "Empresa")
                        .WithMany()
                        .HasForeignKey("EmpresaRut")
                        .IsRequired()
                        .HasConstraintName("FK_Productos_Empresas");

                    b.Navigation("Categoria");

                    b.Navigation("Empresa");
                });

            modelBuilder.Entity("VentaPOS.Models.Role", b =>
                {
                    b.HasOne("VentaPOS.Models.Empresa", "Empresa")
                        .WithMany()
                        .HasForeignKey("EmpresaRut")
                        .IsRequired()
                        .HasConstraintName("FK_Roles_Empresas");

                    b.Navigation("Empresa");
                });

            modelBuilder.Entity("VentaPOS.Models.Suscripciones", b =>
                {
                    b.HasOne("VentaPOS.Models.Empresa", "Empresa")
                        .WithMany("Suscripciones")
                        .HasForeignKey("EmpresaRut")
                        .IsRequired()
                        .HasConstraintName("FK_Suscripciones_Empresas");

                    b.HasOne("VentaPOS.Models.Plane", "Plan")
                        .WithMany("Suscripciones")
                        .HasForeignKey("PlanId")
                        .IsRequired()
                        .HasConstraintName("FK_Suscripciones_Planes");

                    b.Navigation("Empresa");

                    b.Navigation("Plan");
                });

            modelBuilder.Entity("VentaPOS.Models.Usuario", b =>
                {
                    b.HasOne("VentaPOS.Models.Empresa", "Empresa")
                        .WithMany("Usuarios")
                        .HasForeignKey("EmpresaRut")
                        .IsRequired()
                        .HasConstraintName("FK_Usuarios_Empresas");

                    b.Navigation("Empresa");
                });

            modelBuilder.Entity("VentaPOS.Models.Venta", b =>
                {
                    b.HasOne("VentaPOS.Models.Cliente", "Cliente")
                        .WithMany("Venta")
                        .HasForeignKey("ClienteId")
                        .HasConstraintName("FK_Ventas_Clientes");

                    b.HasOne("VentaPOS.Models.Empresa", "Empresa")
                        .WithMany("Venta")
                        .HasForeignKey("EmpresaRut")
                        .IsRequired()
                        .HasConstraintName("FK_Ventas_Empresas");

                    b.HasOne("VentaPOS.Models.Usuario", "Usuario")
                        .WithMany("Venta")
                        .HasForeignKey("UsuarioId")
                        .IsRequired()
                        .HasConstraintName("FK_Ventas_Usuarios");

                    b.Navigation("Cliente");

                    b.Navigation("Empresa");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("VentaPOS.Models.Categoria", b =>
                {
                    b.Navigation("Productos");
                });

            modelBuilder.Entity("VentaPOS.Models.Cliente", b =>
                {
                    b.Navigation("Venta");
                });

            modelBuilder.Entity("VentaPOS.Models.Empresa", b =>
                {
                    b.Navigation("Clientes");

                    b.Navigation("ConfiguracionEmpresas");

                    b.Navigation("Suscripciones");

                    b.Navigation("Usuarios");

                    b.Navigation("Venta");
                });

            modelBuilder.Entity("VentaPOS.Models.Plane", b =>
                {
                    b.Navigation("Suscripciones");
                });

            modelBuilder.Entity("VentaPOS.Models.Producto", b =>
                {
                    b.Navigation("DetallesVenta");

                    b.Navigation("Inventarios");
                });

            modelBuilder.Entity("VentaPOS.Models.Usuario", b =>
                {
                    b.Navigation("Venta");
                });

            modelBuilder.Entity("VentaPOS.Models.Venta", b =>
                {
                    b.Navigation("DetallesVenta");
                });
#pragma warning restore 612, 618
        }
    }
}

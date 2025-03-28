using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VentaPOS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Rut = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    NombreEmpresa = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Activo = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    Contraseña = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Rut);
                });

            migrationBuilder.CreateTable(
                name: "Planes",
                columns: table => new
                {
                    PlanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MaxUsuarios = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Planes__755C22D75E7A22C7", x => x.PlanID);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    CategoriaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    EmpresaRut = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__F353C1C53DC7F524", x => x.CategoriaID);
                    table.ForeignKey(
                        name: "FK_Categorias_Empresas",
                        column: x => x.EmpresaRut,
                        principalTable: "Empresas",
                        principalColumn: "Rut");
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    UltimaCompra = table.Column<DateTime>(type: "datetime", nullable: true),
                    EmpresaRut = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Clientes__71ABD0A71131B070", x => x.ClienteID);
                    table.ForeignKey(
                        name: "FK_Clientes_Empresas",
                        column: x => x.EmpresaRut,
                        principalTable: "Empresas",
                        principalColumn: "Rut");
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionEmpresa",
                columns: table => new
                {
                    ConfiguracionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaRut = table.Column<string>(type: "varchar(20)", nullable: false),
                    Clave = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Valor = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Configur__9B95E0563E7C30A7", x => x.ConfiguracionID);
                    table.ForeignKey(
                        name: "FK_ConfiguracionEmpresa_Empresas",
                        column: x => x.EmpresaRut,
                        principalTable: "Empresas",
                        principalColumn: "Rut");
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RolID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    EmpresaRut = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__F92302D15CC78A2C", x => x.RolID);
                    table.ForeignKey(
                        name: "FK_Roles_Empresas",
                        column: x => x.EmpresaRut,
                        principalTable: "Empresas",
                        principalColumn: "Rut");
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Contraseña = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    EmpresaRut = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuarios__2B3DE798FCB1AA72", x => x.UsuarioID);
                    table.ForeignKey(
                        name: "FK_Usuarios_Empresas",
                        column: x => x.EmpresaRut,
                        principalTable: "Empresas",
                        principalColumn: "Rut");
                });

            migrationBuilder.CreateTable(
                name: "Suscripciones",
                columns: table => new
                {
                    SuscripcionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaRut = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    PlanID = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime", nullable: false),
                    FormaPago = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Pagado = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    Activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaxUsuarios = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Suscripc__814D768B4ED74F72", x => x.SuscripcionID);
                    table.ForeignKey(
                        name: "FK_Suscripciones_Empresas",
                        column: x => x.EmpresaRut,
                        principalTable: "Empresas",
                        principalColumn: "Rut");
                    table.ForeignKey(
                        name: "FK_Suscripciones_Planes",
                        column: x => x.PlanID,
                        principalTable: "Planes",
                        principalColumn: "PlanID");
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    ProductoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    CategoriaID = table.Column<int>(type: "int", nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    EmpresaRut = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Producto__A430AE832C367D1B", x => x.ProductoID);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias",
                        column: x => x.CategoriaID,
                        principalTable: "Categorias",
                        principalColumn: "CategoriaID");
                    table.ForeignKey(
                        name: "FK_Productos_Empresas",
                        column: x => x.EmpresaRut,
                        principalTable: "Empresas",
                        principalColumn: "Rut");
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRoles",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    RolID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UsuarioR__24AFD7B554E7B9DF", x => new { x.UsuarioID, x.RolID });
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Roles",
                        column: x => x.RolID,
                        principalTable: "Roles",
                        principalColumn: "RolID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Usuarios",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    VentaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    ClienteID = table.Column<int>(type: "int", nullable: true),
                    NumeroFactura = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    FechaVenta = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    MetodoPago = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Impuestos = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    Descuento = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Comentarios = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    EmpresaRut = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ventas__5B41514CCDF474A8", x => x.VentaID);
                    table.ForeignKey(
                        name: "FK_Ventas_Clientes",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ClienteID");
                    table.ForeignKey(
                        name: "FK_Ventas_Empresas",
                        column: x => x.EmpresaRut,
                        principalTable: "Empresas",
                        principalColumn: "Rut");
                    table.ForeignKey(
                        name: "FK_Ventas_Usuarios",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID");
                });

            migrationBuilder.CreateTable(
                name: "UsuariosSuscripciones",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    SuscripcionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuarios__832930F0783A500D", x => new { x.UsuarioID, x.SuscripcionID });
                    table.ForeignKey(
                        name: "FK_UsuariosSuscripciones_Suscripciones",
                        column: x => x.SuscripcionID,
                        principalTable: "Suscripciones",
                        principalColumn: "SuscripcionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosSuscripciones_Usuarios",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventario",
                columns: table => new
                {
                    InventarioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoID = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Ubicacion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    EmpresaRut = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Inventar__FB8A24B78F5A075C", x => x.InventarioID);
                    table.ForeignKey(
                        name: "FK_Inventario_Empresas",
                        column: x => x.EmpresaRut,
                        principalTable: "Empresas",
                        principalColumn: "Rut");
                    table.ForeignKey(
                        name: "FK_Inventario_Productos",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ProductoID");
                });

            migrationBuilder.CreateTable(
                name: "DetallesVenta",
                columns: table => new
                {
                    DetalleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VentaID = table.Column<int>(type: "int", nullable: false),
                    ProductoID = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    Impuesto = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Detalles__6E19D6FA3E480FBF", x => x.DetalleID);
                    table.ForeignKey(
                        name: "FK_DetallesVenta_Productos",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ProductoID");
                    table.ForeignKey(
                        name: "FK_DetallesVenta_Ventas",
                        column: x => x.VentaID,
                        principalTable: "Ventas",
                        principalColumn: "VentaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_EmpresaRut",
                table: "Categorias",
                column: "EmpresaRut");

            migrationBuilder.CreateIndex(
                name: "UQ__Categori__75E3EFCF34D4F80D",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_EmpresaRut",
                table: "Clientes",
                column: "EmpresaRut");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionEmpresa_EmpresaRut",
                table: "ConfiguracionEmpresa",
                column: "EmpresaRut");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_ProductoID",
                table: "DetallesVenta",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_VentaID",
                table: "DetallesVenta",
                column: "VentaID");

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_EmpresaRut",
                table: "Inventario",
                column: "EmpresaRut");

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_ProductoID",
                table: "Inventario",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "UQ__Planes__75E3EFCFA5B72F05",
                table: "Planes",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaID",
                table: "Productos",
                column: "CategoriaID");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_EmpresaRut",
                table: "Productos",
                column: "EmpresaRut");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_EmpresaRut",
                table: "Roles",
                column: "EmpresaRut");

            migrationBuilder.CreateIndex(
                name: "UQ__Roles__75E3EFCF93AC4567",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_EmpresaRut",
                table: "Suscripciones",
                column: "EmpresaRut");

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_PlanID",
                table: "Suscripciones",
                column: "PlanID");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_RolID",
                table: "UsuarioRoles",
                column: "RolID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmpresaRut",
                table: "Usuarios",
                column: "EmpresaRut");

            migrationBuilder.CreateIndex(
                name: "UQ__Usuarios__60695A19E6A6D41B",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSuscripciones_SuscripcionID",
                table: "UsuariosSuscripciones",
                column: "SuscripcionID");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ClienteID",
                table: "Ventas",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_EmpresaRut",
                table: "Ventas",
                column: "EmpresaRut");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_UsuarioID",
                table: "Ventas",
                column: "UsuarioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionEmpresa");

            migrationBuilder.DropTable(
                name: "DetallesVenta");

            migrationBuilder.DropTable(
                name: "Inventario");

            migrationBuilder.DropTable(
                name: "UsuarioRoles");

            migrationBuilder.DropTable(
                name: "UsuariosSuscripciones");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Suscripciones");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Planes");

            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}

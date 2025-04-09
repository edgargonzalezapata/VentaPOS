using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VentaPOS.Migrations
{
    /// <inheritdoc />
    public partial class CrearIndiceCompuestoRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Crear el nuevo índice único compuesto
            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre_EmpresaRut",
                table: "Roles",
                columns: new[] { "Nombre", "EmpresaRut" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar el índice compuesto
            migrationBuilder.DropIndex(
                name: "IX_Roles_Nombre_EmpresaRut",
                table: "Roles");
        }
    }
}

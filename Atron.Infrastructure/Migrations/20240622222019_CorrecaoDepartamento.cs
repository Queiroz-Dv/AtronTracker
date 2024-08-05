using Microsoft.EntityFrameworkCore.Migrations;

namespace Atron.Infrastructure.Migrations
{
    public partial class CorrecaoDepartamento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Departamentos_DepartmentoId",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "DepartmentoId",
                table: "Usuarios",
                newName: "DepartamentoId");

            migrationBuilder.RenameColumn(
                name: "DepartmentoCodigo",
                table: "Usuarios",
                newName: "DepartamentoCodigo");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_DepartmentoId",
                table: "Usuarios",
                newName: "IX_Usuarios_DepartamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Departamentos_DepartamentoId",
                table: "Usuarios",
                column: "DepartamentoId",
                principalTable: "Departamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Departamentos_DepartamentoId",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "DepartamentoId",
                table: "Usuarios",
                newName: "DepartmentoId");

            migrationBuilder.RenameColumn(
                name: "DepartamentoCodigo",
                table: "Usuarios",
                newName: "DepartmentoCodigo");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_DepartamentoId",
                table: "Usuarios",
                newName: "IX_Usuarios_DepartmentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Departamentos_DepartmentoId",
                table: "Usuarios",
                column: "DepartmentoId",
                principalTable: "Departamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}

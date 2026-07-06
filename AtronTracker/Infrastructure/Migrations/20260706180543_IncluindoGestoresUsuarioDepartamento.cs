using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class IncluindoGestoresUsuarioDepartamento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GestorImediatoCodigo",
                table: "Usuarios",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GestorImediatoId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GestorDepartamentoCodigo",
                table: "Departamentos",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GestorDepartamentoId",
                table: "Departamentos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_GestorImediatoId_GestorImediatoCodigo",
                table: "Usuarios",
                columns: new[] { "GestorImediatoId", "GestorImediatoCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_GestorDepartamentoId_GestorDepartamentoCodigo",
                table: "Departamentos",
                columns: new[] { "GestorDepartamentoId", "GestorDepartamentoCodigo" });

            migrationBuilder.AddForeignKey(
                name: "FK_Departamentos_Usuarios_GestorDepartamentoId_GestorDepartamentoCodigo",
                table: "Departamentos",
                columns: new[] { "GestorDepartamentoId", "GestorDepartamentoCodigo" },
                principalTable: "Usuarios",
                principalColumns: new[] { "Id", "Codigo" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Usuarios_GestorImediatoId_GestorImediatoCodigo",
                table: "Usuarios",
                columns: new[] { "GestorImediatoId", "GestorImediatoCodigo" },
                principalTable: "Usuarios",
                principalColumns: new[] { "Id", "Codigo" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departamentos_Usuarios_GestorDepartamentoId_GestorDepartamentoCodigo",
                table: "Departamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Usuarios_GestorImediatoId_GestorImediatoCodigo",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_GestorImediatoId_GestorImediatoCodigo",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Departamentos_GestorDepartamentoId_GestorDepartamentoCodigo",
                table: "Departamentos");

            migrationBuilder.DropColumn(
                name: "GestorImediatoCodigo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "GestorImediatoId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "GestorDepartamentoCodigo",
                table: "Departamentos");

            migrationBuilder.DropColumn(
                name: "GestorDepartamentoId",
                table: "Departamentos");
        }
    }
}

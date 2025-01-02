using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class ConfigureAssociacaoUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Cargos_CargoId_CargoCodigo",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Departamentos_DepartamentoId_DepartamentoCodigo",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_CargoId_CargoCodigo",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_DepartamentoId_DepartamentoCodigo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CargoCodigo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CargoId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DepartamentoCodigo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DepartamentoId",
                table: "Usuarios");

            migrationBuilder.AlterColumn<int>(
                name: "SalarioAtual",
                table: "Usuarios",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Usuarios",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UsuarioCargoDepartamento",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CargoId = table.Column<int>(type: "int", nullable: false),
                    CargoCodigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false),
                    DepartamentoCodigo = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioCargoDepartamento", x => new { x.UsuarioId, x.UsuarioCodigo, x.CargoId, x.CargoCodigo, x.DepartamentoId, x.DepartamentoCodigo });
                    table.ForeignKey(
                        name: "FK_UsuarioCargoDepartamento_Cargos_CargoId_CargoCodigo",
                        columns: x => new { x.CargoId, x.CargoCodigo },
                        principalTable: "Cargos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioCargoDepartamento_Departamentos_DepartamentoId_DepartamentoCodigo",
                        columns: x => new { x.DepartamentoId, x.DepartamentoCodigo },
                        principalTable: "Departamentos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioCargoDepartamento_Usuarios_UsuarioId_UsuarioCodigo",
                        columns: x => new { x.UsuarioId, x.UsuarioCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCargoDepartamento_CargoId_CargoCodigo",
                table: "UsuarioCargoDepartamento",
                columns: new[] { "CargoId", "CargoCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCargoDepartamento_DepartamentoId_DepartamentoCodigo",
                table: "UsuarioCargoDepartamento",
                columns: new[] { "DepartamentoId", "DepartamentoCodigo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioCargoDepartamento");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Usuarios");

            migrationBuilder.AlterColumn<int>(
                name: "SalarioAtual",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CargoCodigo",
                table: "Usuarios",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CargoId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DepartamentoCodigo",
                table: "Usuarios",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartamentoId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CargoId_CargoCodigo",
                table: "Usuarios",
                columns: new[] { "CargoId", "CargoCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_DepartamentoId_DepartamentoCodigo",
                table: "Usuarios",
                columns: new[] { "DepartamentoId", "DepartamentoCodigo" });

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Cargos_CargoId_CargoCodigo",
                table: "Usuarios",
                columns: new[] { "CargoId", "CargoCodigo" },
                principalTable: "Cargos",
                principalColumns: new[] { "Id", "Codigo" });

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Departamentos_DepartamentoId_DepartamentoCodigo",
                table: "Usuarios",
                columns: new[] { "DepartamentoId", "DepartamentoCodigo" },
                principalTable: "Departamentos",
                principalColumns: new[] { "Id", "Codigo" });
        }
    }
}

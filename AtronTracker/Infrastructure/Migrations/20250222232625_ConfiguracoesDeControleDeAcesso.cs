using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class ConfiguracoesDeControleDeAcesso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Usuarios",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCodigo",
                table: "UsuarioCargoDepartamento",
                type: "nvarchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCodigo",
                table: "Tarefas",
                type: "nvarchar(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCodigo",
                table: "Salarios",
                type: "nvarchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "PerfisDeAcesso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisDeAcesso", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "PerfilDeAcessoModulos",
                columns: table => new
                {
                    PerfilDeAcessoId = table.Column<int>(type: "int", nullable: false),
                    PerfilDeAcessoCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    ModuloId = table.Column<int>(type: "int", nullable: false),
                    ModuloCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilDeAcessoModulos", x => new { x.PerfilDeAcessoId, x.PerfilDeAcessoCodigo, x.ModuloId, x.ModuloCodigo });
                    table.ForeignKey(
                        name: "FK_PerfilDeAcessoModulos_Modulos_ModuloId_ModuloCodigo",
                        columns: x => new { x.ModuloId, x.ModuloCodigo },
                        principalTable: "Modulos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfilDeAcessoModulos_PerfisDeAcesso_PerfilDeAcessoId_PerfilDeAcessoCodigo",
                        columns: x => new { x.PerfilDeAcessoId, x.PerfilDeAcessoCodigo },
                        principalTable: "PerfisDeAcesso",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfilDeAcessoUsuarios",
                columns: table => new
                {
                    PerfilDeAcessoId = table.Column<int>(type: "int", nullable: false),
                    PerfilDeAcessoCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilDeAcessoUsuarios", x => new { x.PerfilDeAcessoId, x.PerfilDeAcessoCodigo, x.UsuarioId, x.UsuarioCodigo });
                    table.ForeignKey(
                        name: "FK_PerfilDeAcessoUsuarios_PerfisDeAcesso_PerfilDeAcessoId_PerfilDeAcessoCodigo",
                        columns: x => new { x.PerfilDeAcessoId, x.PerfilDeAcessoCodigo },
                        principalTable: "PerfisDeAcesso",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfilDeAcessoUsuarios_Usuarios_UsuarioId_UsuarioCodigo",
                        columns: x => new { x.UsuarioId, x.UsuarioCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerfilDeAcessoModulos_ModuloId_ModuloCodigo",
                table: "PerfilDeAcessoModulos",
                columns: new[] { "ModuloId", "ModuloCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_PerfilDeAcessoUsuarios_UsuarioId_UsuarioCodigo",
                table: "PerfilDeAcessoUsuarios",
                columns: new[] { "UsuarioId", "UsuarioCodigo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerfilDeAcessoModulos");

            migrationBuilder.DropTable(
                name: "PerfilDeAcessoUsuarios");

            migrationBuilder.DropTable(
                name: "Modulos");

            migrationBuilder.DropTable(
                name: "PerfisDeAcesso");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Usuarios",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCodigo",
                table: "UsuarioCargoDepartamento",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCodigo",
                table: "Tarefas",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCodigo",
                table: "Salarios",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");
        }
    }
}

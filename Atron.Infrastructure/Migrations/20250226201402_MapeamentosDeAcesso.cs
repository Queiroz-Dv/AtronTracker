using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class MapeamentosDeAcesso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioCodigo",
                table: "PerfisDeAcesso",
                type: "nvarchar(10)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "PerfisDeAcesso",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PropriedadesDeFluxo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropriedadesDeFluxo", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "PropriedadeDeFluxoModulo",
                columns: table => new
                {
                    ModuloId = table.Column<int>(type: "int", nullable: false),
                    ModuloCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    PropriedadeDeFluxoId = table.Column<int>(type: "int", nullable: false),
                    PropriedadeDeFluxoCodigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PropriedadesDeFluxoId = table.Column<int>(type: "int", nullable: true),
                    PropriedadesDeFluxoCodigo = table.Column<string>(type: "nvarchar(25)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropriedadeDeFluxoModulo", x => new { x.ModuloId, x.ModuloCodigo, x.PropriedadeDeFluxoId, x.PropriedadeDeFluxoCodigo });
                    table.ForeignKey(
                        name: "FK_PropriedadeDeFluxoModulo_Modulos_ModuloId_ModuloCodigo",
                        columns: x => new { x.ModuloId, x.ModuloCodigo },
                        principalTable: "Modulos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropriedadeDeFluxoModulo_PropriedadesDeFluxo_PropriedadesDeFluxoId_PropriedadesDeFluxoCodigo",
                        columns: x => new { x.PropriedadesDeFluxoId, x.PropriedadesDeFluxoCodigo },
                        principalTable: "PropriedadesDeFluxo",
                        principalColumns: new[] { "Id", "Codigo" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerfisDeAcesso_UsuarioId_UsuarioCodigo",
                table: "PerfisDeAcesso",
                columns: new[] { "UsuarioId", "UsuarioCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_PropriedadeDeFluxoModulo_PropriedadesDeFluxoId_PropriedadesDeFluxoCodigo",
                table: "PropriedadeDeFluxoModulo",
                columns: new[] { "PropriedadesDeFluxoId", "PropriedadesDeFluxoCodigo" });

            migrationBuilder.AddForeignKey(
                name: "FK_PerfisDeAcesso_Usuarios_UsuarioId_UsuarioCodigo",
                table: "PerfisDeAcesso",
                columns: new[] { "UsuarioId", "UsuarioCodigo" },
                principalTable: "Usuarios",
                principalColumns: new[] { "Id", "Codigo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerfisDeAcesso_Usuarios_UsuarioId_UsuarioCodigo",
                table: "PerfisDeAcesso");

            migrationBuilder.DropTable(
                name: "PropriedadeDeFluxoModulo");

            migrationBuilder.DropTable(
                name: "PropriedadesDeFluxo");

            migrationBuilder.DropIndex(
                name: "IX_PerfisDeAcesso_UsuarioId_UsuarioCodigo",
                table: "PerfisDeAcesso");

            migrationBuilder.DropColumn(
                name: "UsuarioCodigo",
                table: "PerfisDeAcesso");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "PerfisDeAcesso");
        }
    }
}

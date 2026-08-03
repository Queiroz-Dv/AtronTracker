using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AtronTracker.Infrastructure.Migrations.Migrations
{
    public partial class RemovendoModuloSalarios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Salarios");

            migrationBuilder.Sql("DELETE FROM \"PerfilDeAcessoModulos\" WHERE \"ModuloId\" = 5 AND \"ModuloCodigo\" = 'SAL'");

            migrationBuilder.DeleteData(
                table: "Modulos",
                keyColumns: new[] { "Codigo", "Id" },
                keyValues: new object[] { "SAL", 5 });

            migrationBuilder.DropColumn(
                name: "SalarioAtual",
                table: "Usuarios");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SalarioAtual",
                table: "Usuarios",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Salarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "character varying(10)", nullable: false),
                    Ano = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    MesId = table.Column<int>(type: "integer", maxLength: 12, nullable: false),
                    SalarioMensal = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Salarios_Usuarios_UsuarioId_UsuarioCodigo",
                        columns: x => new { x.UsuarioId, x.UsuarioCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Modulos",
                columns: new[] { "Codigo", "Id", "Descricao" },
                values: new object[] { "SAL", 5, "Salários" });

            migrationBuilder.CreateIndex(
                name: "IX_Salarios_UsuarioId_UsuarioCodigo",
                table: "Salarios",
                columns: new[] { "UsuarioId", "UsuarioCodigo" },
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class ConfiguracoesDeAcesso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerfilDeAcesso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilDeAcesso", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "Modulo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PerfilDeAcessoId = table.Column<int>(type: "int", nullable: false),
                    PerfilDeAcessoCodigo = table.Column<string>(type: "nvarchar(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulo", x => new { x.Id, x.Codigo });
                    table.ForeignKey(
                        name: "FK_PERFILDEACESSO_ID",
                        columns: x => new { x.PerfilDeAcessoId, x.PerfilDeAcessoCodigo },
                        principalTable: "PerfilDeAcesso",
                        principalColumns: new[] { "Id", "Codigo" });
                });

            migrationBuilder.CreateTable(
                name: "PerfilDeAcessoUsuario",
                columns: table => new
                {
                    PerfisDeAcessoId = table.Column<int>(type: "int", nullable: false),
                    PerfisDeAcessoCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    UsuariosId = table.Column<int>(type: "int", nullable: false),
                    UsuariosCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilDeAcessoUsuario", x => new { x.PerfisDeAcessoId, x.PerfisDeAcessoCodigo, x.UsuariosId, x.UsuariosCodigo });
                    table.ForeignKey(
                        name: "FK_PerfilDeAcessoUsuario_PerfilDeAcesso_PerfisDeAcessoId_PerfisDeAcessoCodigo",
                        columns: x => new { x.PerfisDeAcessoId, x.PerfisDeAcessoCodigo },
                        principalTable: "PerfilDeAcesso",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfilDeAcessoUsuario_Usuarios_UsuariosId_UsuariosCodigo",
                        columns: x => new { x.UsuariosId, x.UsuariosCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });         
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modulo");

            migrationBuilder.DropTable(
                name: "PerfilDeAcessoUsuario");

            migrationBuilder.DropTable(
                name: "PerfilDeAcesso");
        }
    }
}

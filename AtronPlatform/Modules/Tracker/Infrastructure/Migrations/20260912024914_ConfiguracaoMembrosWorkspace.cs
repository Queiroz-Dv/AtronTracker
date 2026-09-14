using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ConfiguracaoMembrosWorkspace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MembrosWorkspace",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkspaceId = table.Column<int>(type: "integer", nullable: false),
                    WorkspaceCodigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembrosWorkspace", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembrosWorkspace_Workspaces_WorkspaceId_WorkspaceCodigo",
                        columns: x => new { x.WorkspaceId, x.WorkspaceCodigo },
                        principalTable: "Workspaces",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MembrosWorkspace_Usuarios_UsuarioId_UsuarioCodigo",
                        columns: x => new { x.UsuarioId, x.UsuarioCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembrosWorkspace_UsuarioId_UsuarioCodigo",
                table: "MembrosWorkspace",
                columns: new[] { "UsuarioId", "UsuarioCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_MembrosWorkspace_WorkspaceId_WorkspaceCodigo",
                table: "MembrosWorkspace",
                columns: new[] { "WorkspaceId", "WorkspaceCodigo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

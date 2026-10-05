using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class RemoveMembroWorkspace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MembrosWorkspace");

            migrationBuilder.AlterColumn<string>(
                name: "DestinoInicial",
                table: "Tarefas",
                type: "text",
                nullable: false,
                defaultValue: "1",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "TarefaMovimentacoes",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DestinoInicial",
                table: "Tarefas",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "1");

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "TarefaMovimentacoes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.CreateTable(
                name: "MembrosWorkspace",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true),
                    WorkspaceId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioCodigo1 = table.Column<string>(type: "character varying(10)", nullable: true),
                    WorkspaceCodigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembrosWorkspace", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembrosWorkspace_Usuarios_UsuarioId_UsuarioCodigo1",
                        columns: x => new { x.UsuarioId, x.UsuarioCodigo1 },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" });
                    table.ForeignKey(
                        name: "FK_MembrosWorkspace_Workspaces_WorkspaceId_WorkspaceCodigo",
                        columns: x => new { x.WorkspaceId, x.WorkspaceCodigo },
                        principalTable: "Workspaces",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembrosWorkspace_UsuarioId_UsuarioCodigo1",
                table: "MembrosWorkspace",
                columns: new[] { "UsuarioId", "UsuarioCodigo1" });

            migrationBuilder.CreateIndex(
                name: "IX_MembrosWorkspace_WorkspaceId_WorkspaceCodigo",
                table: "MembrosWorkspace",
                columns: new[] { "WorkspaceId", "WorkspaceCodigo" });
        }
    }
}

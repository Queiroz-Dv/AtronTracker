using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddRecursoWorkspaceETenantFilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecursoWorkspace",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkspaceId = table.Column<int>(type: "integer", nullable: false),
                    WorkspaceCodigo = table.Column<string>(type: "character varying(10)", nullable: true),
                    ModuloCodigo = table.Column<string>(type: "text", nullable: true),
                    RecursoId = table.Column<int>(type: "integer", nullable: false),
                    RecursoCodigo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecursoWorkspace", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecursoWorkspace_Workspaces_WorkspaceId_WorkspaceCodigo",
                        columns: x => new { x.WorkspaceId, x.WorkspaceCodigo },
                        principalTable: "Workspaces",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(@"
                INSERT INTO ""RecursoWorkspace"" (""WorkspaceId"", ""WorkspaceCodigo"", ""ModuloCodigo"", ""RecursoId"", ""RecursoCodigo"")
                SELECT ""WorkspaceId"", ""WorkspaceCodigo"", 'Modulo:DPT', ""DepartamentoId"", ""DepartamentoCodigo""
                FROM ""DepartamentoTenant"";
            ");

            migrationBuilder.DropTable(
                name: "DepartamentoTenant");

            migrationBuilder.CreateIndex(
                name: "ix_recurso_workspace_pesquisa",
                table: "RecursoWorkspace",
                columns: new[] { "RecursoId", "ModuloCodigo", "WorkspaceId" });

            migrationBuilder.CreateIndex(
                name: "IX_RecursoWorkspace_WorkspaceId_WorkspaceCodigo",
                table: "RecursoWorkspace",
                columns: new[] { "WorkspaceId", "WorkspaceCodigo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecursoWorkspace");

            migrationBuilder.CreateTable(
                name: "DepartamentoTenant",
                columns: table => new
                {
                    WorkspaceId = table.Column<int>(type: "integer", nullable: false),
                    WorkspaceCodigo = table.Column<string>(type: "character varying(10)", nullable: false),
                    DepartamentoId = table.Column<int>(type: "integer", nullable: false),
                    DepartamentoCodigo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartamentoTenant", x => new { x.WorkspaceId, x.WorkspaceCodigo, x.DepartamentoId, x.DepartamentoCodigo });
                    table.ForeignKey(
                        name: "FK_DepartamentoTenant_Departamentos_DepartamentoId_Departament~",
                        columns: x => new { x.DepartamentoId, x.DepartamentoCodigo },
                        principalTable: "Departamentos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartamentoTenant_Workspaces_WorkspaceId_WorkspaceCodigo",
                        columns: x => new { x.WorkspaceId, x.WorkspaceCodigo },
                        principalTable: "Workspaces",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartamentoTenant_DepartamentoId_DepartamentoCodigo",
                table: "DepartamentoTenant",
                columns: new[] { "DepartamentoId", "DepartamentoCodigo" });
        }
    }
}

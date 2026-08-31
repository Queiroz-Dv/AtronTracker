using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class VinculandoWorkspaceEmpresaPorCodigo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_Empresas_EmpresaId",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_Workspaces_EmpresaId",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_Empresas_Codigo",
                table: "Empresas");

            migrationBuilder.AddColumn<string>(
                name: "EmpresaCodigo",
                table: "Workspaces",
                type: "character varying(25)",
                maxLength: 25,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE "Workspaces" AS workspace
                SET "EmpresaCodigo" = empresa."Codigo"
                FROM "Empresas" AS empresa
                WHERE workspace."EmpresaId" = empresa."Id";
                """);

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Workspaces");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Empresas_Codigo",
                table: "Empresas",
                column: "Codigo");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_EmpresaCodigo",
                table: "Workspaces",
                column: "EmpresaCodigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_Empresas_EmpresaCodigo",
                table: "Workspaces",
                column: "EmpresaCodigo",
                principalTable: "Empresas",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_Empresas_EmpresaCodigo",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_Workspaces_EmpresaCodigo",
                table: "Workspaces");

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Workspaces",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE "Workspaces" AS workspace
                SET "EmpresaId" = empresa."Id"
                FROM "Empresas" AS empresa
                WHERE workspace."EmpresaCodigo" = empresa."Codigo";
                """);

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Empresas_Codigo",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "EmpresaCodigo",
                table: "Workspaces");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_EmpresaId",
                table: "Workspaces",
                column: "EmpresaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_Codigo",
                table: "Empresas",
                column: "Codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_Empresas_EmpresaId",
                table: "Workspaces",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

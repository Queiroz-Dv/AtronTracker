using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddWorkspaceResponsavelFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {           
            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_Usuarios_ResponsavelCodigo_ResponsavelEmail",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_Workspaces_ResponsavelCodigo_ResponsavelEmail",
                table: "Workspaces");               
            
            migrationBuilder.AlterColumn<string>(
                name: "ResponsavelEmail",
                table: "Workspaces",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Workspaces",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "ResponsavelId",
                table: "Workspaces",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_ResponsavelId_ResponsavelCodigo",
                table: "Workspaces",
                columns: new[] { "ResponsavelId", "ResponsavelCodigo" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_Usuarios_ResponsavelId_ResponsavelCodigo",
                table: "Workspaces",
                columns: new[] { "ResponsavelId", "ResponsavelCodigo" },
                principalTable: "Usuarios",
                principalColumns: new[] { "Id", "Codigo" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_Usuarios_ResponsavelId_ResponsavelCodigo",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_Workspaces_ResponsavelId_ResponsavelCodigo",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "ResponsavelId",
                table: "Workspaces");

            migrationBuilder.AlterColumn<string>(
                name: "ResponsavelEmail",
                table: "Workspaces",
                type: "character varying(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Workspaces",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "WorkspaceCodigo",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkspaceId",
                table: "Usuarios",
                type: "integer",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Usuarios_Codigo_Email",
                table: "Usuarios",
                columns: new[] { "Codigo", "Email" });

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_ResponsavelCodigo_ResponsavelEmail",
                table: "Workspaces",
                columns: new[] { "ResponsavelCodigo", "ResponsavelEmail" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_WorkspaceId_WorkspaceCodigo",
                table: "Usuarios",
                columns: new[] { "WorkspaceId", "WorkspaceCodigo" });

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Workspaces_WorkspaceId_WorkspaceCodigo",
                table: "Usuarios",
                columns: new[] { "WorkspaceId", "WorkspaceCodigo" },
                principalTable: "Workspaces",
                principalColumns: new[] { "Id", "Codigo" });

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_Usuarios_ResponsavelCodigo_ResponsavelEmail",
                table: "Workspaces",
                columns: new[] { "ResponsavelCodigo", "ResponsavelEmail" },
                principalTable: "Usuarios",
                principalColumns: new[] { "Codigo", "Email" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}

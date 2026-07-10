using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtronTracker.Infrastructure.PostgreSqlMigrations.Migrations
{
    public partial class IncluindoConfirmacaoEmailUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmado",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmado",
                table: "Usuarios");
        }
    }
}

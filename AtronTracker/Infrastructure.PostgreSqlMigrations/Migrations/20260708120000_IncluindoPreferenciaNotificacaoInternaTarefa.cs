using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtronTracker.Infrastructure.PostgreSqlMigrations.Migrations
{
    public partial class IncluindoPreferenciaNotificacaoInternaTarefa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReceberNotificacaoInternaTarefa",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceberNotificacaoInternaTarefa",
                table: "Usuarios");
        }
    }
}

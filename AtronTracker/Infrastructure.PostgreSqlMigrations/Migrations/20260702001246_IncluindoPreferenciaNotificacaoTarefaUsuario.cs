using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtronTracker.Infrastructure.PostgreSqlMigrations.Migrations
{
    public partial class IncluindoPreferenciaNotificacaoTarefaUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReceberNotificacaoTarefaPorEmail",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceberNotificacaoTarefaPorEmail",
                table: "Usuarios");
        }
    }
}

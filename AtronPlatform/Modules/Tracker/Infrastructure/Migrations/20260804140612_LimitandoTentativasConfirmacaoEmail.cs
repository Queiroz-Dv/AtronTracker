using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtronTracker.Infrastructure.Migrations.Migrations
{
    public partial class LimitandoTentativasConfirmacaoEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TentativasFalhas",
                table: "ConfirmacoesEmail",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TentativasFalhas",
                table: "ConfirmacoesEmail");
        }
    }
}

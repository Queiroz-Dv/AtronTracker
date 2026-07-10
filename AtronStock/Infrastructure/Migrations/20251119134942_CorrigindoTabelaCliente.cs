using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtronStock.Infrastructure.Migrations
{
    public partial class CorrigindoTabelaCliente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditoriaId",
                schema: "AtronStock",
                table: "Clientes");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "AtronStock",
                table: "Clientes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "AtronStock",
                table: "Clientes");

            migrationBuilder.AddColumn<int>(
                name: "AuditoriaId",
                schema: "AtronStock",
                table: "Clientes",
                type: "int",
                nullable: true);
        }
    }
}

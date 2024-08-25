using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class AlteracaoDaEntidadeDeRotas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HttpMethod",
                table: "ApiRoutes");

            migrationBuilder.DropColumn(
                name: "RouteUrl",
                table: "ApiRoutes");

            migrationBuilder.RenameColumn(
                name: "ModuleName",
                table: "ApiRoutes",
                newName: "Modulo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "ApiRoutes",
                newName: "Ativo");

            migrationBuilder.AddColumn<int>(
                name: "Acao",
                table: "ApiRoutes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NomeDaRotaDeAcesso",
                table: "ApiRoutes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acao",
                table: "ApiRoutes");

            migrationBuilder.DropColumn(
                name: "NomeDaRotaDeAcesso",
                table: "ApiRoutes");

            migrationBuilder.RenameColumn(
                name: "Modulo",
                table: "ApiRoutes",
                newName: "ModuleName");

            migrationBuilder.RenameColumn(
                name: "Ativo",
                table: "ApiRoutes",
                newName: "IsActive");

            migrationBuilder.AddColumn<string>(
                name: "HttpMethod",
                table: "ApiRoutes",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RouteUrl",
                table: "ApiRoutes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}

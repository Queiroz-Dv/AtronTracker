using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations
{
    public partial class AlterandoSchemaShared : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Historicos_CodigoRegistro",
                schema: "AtronShared",
                table: "Historicos");

            migrationBuilder.DropIndex(
                name: "IX_Auditorias_CodigoRegistro",
                schema: "AtronShared",
                table: "Auditorias");

            migrationBuilder.RenameTable(
                name: "Historicos",
                schema: "AtronShared",
                newName: "Historicos");

            migrationBuilder.RenameTable(
                name: "Auditorias",
                schema: "AtronShared",
                newName: "Auditorias");

            migrationBuilder.AddColumn<string>(
                name: "Contexto",
                table: "Historicos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Contexto",
                table: "Auditorias",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Historicos_Contexto_CodigoRegistro",
                table: "Historicos",
                columns: new[] { "Contexto", "CodigoRegistro" });

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_Contexto_CodigoRegistro",
                table: "Auditorias",
                columns: new[] { "Contexto", "CodigoRegistro" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Historicos_Contexto_CodigoRegistro",
                table: "Historicos");

            migrationBuilder.DropIndex(
                name: "IX_Auditorias_Contexto_CodigoRegistro",
                table: "Auditorias");

            migrationBuilder.DropColumn(
                name: "Contexto",
                table: "Historicos");

            migrationBuilder.DropColumn(
                name: "Contexto",
                table: "Auditorias");

            migrationBuilder.RenameTable(
                name: "Historicos",
                newName: "Historicos",
                newSchema: "AtronShared");

            migrationBuilder.RenameTable(
                name: "Auditorias",
                newName: "Auditorias",
                newSchema: "AtronShared");

            migrationBuilder.CreateIndex(
                name: "IX_Historicos_CodigoRegistro",
                schema: "AtronShared",
                table: "Historicos",
                column: "CodigoRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_CodigoRegistro",
                schema: "AtronShared",
                table: "Auditorias",
                column: "CodigoRegistro");
        }
    }
}

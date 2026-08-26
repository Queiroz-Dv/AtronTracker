using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class RemovendoIdentificadorTarefa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tarefas_Identificador",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "Identificador",
                table: "Tarefas");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Identificador",
                table: "Tarefas",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE \"Tarefas\" SET \"Identificador\" = \"Id\";");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_Identificador",
                table: "Tarefas",
                column: "Identificador");
        }
    }
}

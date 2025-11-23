using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class RemovendoEntidadeMes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salarios_Meses_MesId",
                table: "Salarios");

            migrationBuilder.DropTable(
                name: "Meses");         
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Meses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Meses",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 1, "Janeiro" },
                    { 2, "Fevereiro" },
                    { 3, "Março" },
                    { 4, "Abril" },
                    { 5, "Maio" },
                    { 6, "Junho" },
                    { 7, "Julho" },
                    { 8, "Agosto" },
                    { 9, "Setembro" },
                    { 10, "Outubro" },
                    { 11, "Novembro" },
                    { 12, "Dezembro" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Salarios_MesId",
                table: "Salarios",
                column: "MesId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Salarios_Meses_MesId",
                table: "Salarios",
                column: "MesId",
                principalTable: "Meses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

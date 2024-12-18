using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class InclusaoDeNovosEstados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TarefaEstados",
                keyColumn: "Id",
                keyValue: 2,
                column: "Descricao",
                value: "Pendente de aprovação");

            migrationBuilder.InsertData(
                table: "TarefaEstados",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 4, "Finalizada" });

            migrationBuilder.InsertData(
                table: "TarefaEstados",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 5, "Iniciada" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TarefaEstados",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TarefaEstados",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "TarefaEstados",
                keyColumn: "Id",
                keyValue: 2,
                column: "Descricao",
                value: "Aprovada");
        }
    }
}

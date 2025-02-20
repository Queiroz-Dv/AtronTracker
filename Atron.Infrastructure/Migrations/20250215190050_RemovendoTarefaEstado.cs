using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class RemovendoTarefaEstado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TarefaEstados");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TarefaEstados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarefaEstados", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TarefaEstados",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 1, "Em atividade" },
                    { 2, "Pendente de aprovação" },
                    { 3, "Entregue" },
                    { 4, "Finalizada" },
                    { 5, "Iniciada" }
                });
        }
    }
}

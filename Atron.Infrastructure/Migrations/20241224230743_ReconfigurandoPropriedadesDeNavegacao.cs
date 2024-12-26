using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class ReconfigurandoPropriedadesDeNavegacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_TarefaEstados_TarefaEstadoId",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_TarefaEstadoId",
                table: "Tarefas");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_TarefaEstadoId",
                table: "Tarefas",
                column: "TarefaEstadoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_TarefaEstados_TarefaEstadoId",
                table: "Tarefas",
                column: "TarefaEstadoId",
                principalTable: "TarefaEstados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

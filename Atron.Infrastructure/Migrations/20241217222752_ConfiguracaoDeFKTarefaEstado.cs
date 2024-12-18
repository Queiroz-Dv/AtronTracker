using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class ConfiguracaoDeFKTarefaEstado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EstadoDaTarefa",
                table: "Tarefas",
                newName: "EstadoDaTarefaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_EstadoDaTarefaId",
                table: "Tarefas",
                column: "EstadoDaTarefaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_TarefaEstados_EstadoDaTarefaId",
                table: "Tarefas",
                column: "EstadoDaTarefaId",
                principalTable: "TarefaEstados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_TarefaEstados_EstadoDaTarefaId",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_EstadoDaTarefaId",
                table: "Tarefas");

            migrationBuilder.RenameColumn(
                name: "EstadoDaTarefaId",
                table: "Tarefas",
                newName: "EstadoDaTarefa");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class ConfiguracaoDeEstadoDaTarefa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                newName: "TarefaEstadoId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_TarefaEstados_TarefaEstadoId",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_TarefaEstadoId",
                table: "Tarefas");

            migrationBuilder.RenameColumn(
                name: "TarefaEstadoId",
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
    }
}

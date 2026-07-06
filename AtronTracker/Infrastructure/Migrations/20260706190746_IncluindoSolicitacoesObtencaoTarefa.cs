using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class IncluindoSolicitacoesObtencaoTarefa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExigeAprovacaoParaObter",
                table: "Tarefas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SolicitacoesObtencaoTarefa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TarefaId = table.Column<int>(type: "int", nullable: false),
                    SolicitanteId = table.Column<int>(type: "int", nullable: false),
                    SolicitanteCodigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AprovadorId = table.Column<int>(type: "int", nullable: false),
                    AprovadorCodigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataSolicitacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataDecisao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitacoesObtencaoTarefa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitacoesObtencaoTarefa_Tarefas_TarefaId",
                        column: x => x.TarefaId,
                        principalTable: "Tarefas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitacoesObtencaoTarefa_Usuarios_AprovadorId_AprovadorCodigo",
                        columns: x => new { x.AprovadorId, x.AprovadorCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitacoesObtencaoTarefa_Usuarios_SolicitanteId_SolicitanteCodigo",
                        columns: x => new { x.SolicitanteId, x.SolicitanteCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesObtencaoTarefa_AprovadorId_AprovadorCodigo_Status",
                table: "SolicitacoesObtencaoTarefa",
                columns: new[] { "AprovadorId", "AprovadorCodigo", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesObtencaoTarefa_SolicitanteId_SolicitanteCodigo",
                table: "SolicitacoesObtencaoTarefa",
                columns: new[] { "SolicitanteId", "SolicitanteCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesObtencaoTarefa_TarefaId_Status",
                table: "SolicitacoesObtencaoTarefa",
                columns: new[] { "TarefaId", "Status" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitacoesObtencaoTarefa");

            migrationBuilder.DropColumn(
                name: "ExigeAprovacaoParaObter",
                table: "Tarefas");
        }
    }
}

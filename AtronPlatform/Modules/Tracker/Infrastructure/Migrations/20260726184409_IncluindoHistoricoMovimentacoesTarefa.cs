using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AtronTracker.Infrastructure.Migrations.Migrations
{
    public partial class IncluindoHistoricoMovimentacoesTarefa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TarefaMovimentacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TarefaId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1500)", maxLength: 1500, nullable: false),
                    ResponsavelCodigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ResponsavelNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DataOcorrencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarefaMovimentacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TarefaMovimentacoes_Tarefas_TarefaId",
                        column: x => x.TarefaId,
                        principalTable: "Tarefas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TarefaMovimentacoes_TarefaId_DataOcorrencia",
                table: "TarefaMovimentacoes",
                columns: new[] { "TarefaId", "DataOcorrencia" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TarefaMovimentacoes");
        }
    }
}

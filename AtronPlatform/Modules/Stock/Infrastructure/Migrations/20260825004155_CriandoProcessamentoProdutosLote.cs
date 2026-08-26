using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AtronStock.Infrastructure.Migrations
{
    public partial class CriandoProcessamentoProdutosLote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessamentosProdutosLote",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    CodigoBase = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    QuantidadeSolicitada = table.Column<int>(type: "integer", nullable: false),
                    QuantidadeProcessada = table.Column<int>(type: "integer", nullable: false),
                    SolicitanteCodigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DescricaoComplementar = table.Column<string>(type: "text", nullable: true),
                    DataAquisicao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoriaCodigos = table.Column<string>(type: "text", nullable: false),
                    LoteProdutoId = table.Column<int>(type: "integer", nullable: true),
                    Erro = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessamentosProdutosLote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessamentosProdutosLote_LotesProdutos_LoteProdutoId",
                        column: x => x.LoteProdutoId,
                        principalTable: "LotesProdutos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentosProdutosLote_LoteProdutoId",
                table: "ProcessamentosProdutosLote",
                column: "LoteProdutoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentosProdutosLote_Status_Id",
                table: "ProcessamentosProdutosLote",
                columns: new[] { "Status", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessamentosProdutosLote");

        }
    }
}

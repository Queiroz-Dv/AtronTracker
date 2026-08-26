using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AtronStock.Infrastructure.Migrations
{
    public partial class ModelandoProdutoPatrimonial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProdutoFornecedor");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAquisicao",
                table: "Produtos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEfetivaBaixa",
                table: "Produtos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescricaoComplementar",
                table: "Produtos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoteProdutoId",
                table: "Produtos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoUnitario",
                table: "Produtos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Produtos",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "LotesProdutos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotesProdutos", x => x.Id);
                });

            migrationBuilder.Sql(
                """
                UPDATE "Produtos" SET "Codigo" = UPPER(BTRIM("Codigo"));
                UPDATE "ProdutoCategorias" SET "ProdutoCodigo" = UPPER(BTRIM("ProdutoCodigo"));
                UPDATE "ItensEntrada" SET "ProdutoCodigo" = UPPER(BTRIM("ProdutoCodigo"));
                UPDATE "ItensVenda" SET "ProdutoCodigo" = UPPER(BTRIM("ProdutoCodigo"));
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Codigo",
                table: "Produtos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_LoteProdutoId",
                table: "Produtos",
                column: "LoteProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_LotesProdutos_Codigo",
                table: "LotesProdutos",
                column: "Codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_LotesProdutos_LoteProdutoId",
                table: "Produtos",
                column: "LoteProdutoId",
                principalTable: "LotesProdutos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_LotesProdutos_LoteProdutoId",
                table: "Produtos");

            migrationBuilder.DropTable(
                name: "LotesProdutos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_Codigo",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_LoteProdutoId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "DataAquisicao",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "DataEfetivaBaixa",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "DescricaoComplementar",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "LoteProdutoId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "PrecoUnitario",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Produtos");

            migrationBuilder.CreateTable(
                name: "ProdutoFornecedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FornecedorId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    FornecedorCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    ProdutoCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoFornecedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdutoFornecedor_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdutoFornecedor_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoFornecedor_FornecedorId",
                table: "ProdutoFornecedor",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoFornecedor_ProdutoId",
                table: "ProdutoFornecedor",
                column: "ProdutoId");
        }
    }
}

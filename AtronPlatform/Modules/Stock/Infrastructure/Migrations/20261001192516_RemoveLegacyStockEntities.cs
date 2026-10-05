using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AtronStock.Infrastructure.Migrations
{
    public partial class RemoveLegacyStockEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensEntrada");

            migrationBuilder.DropTable(
                name: "ItensVenda");

            migrationBuilder.DropTable(
                name: "MovimentacoesEstoque");

            migrationBuilder.DropTable(
                name: "Entradas");

            migrationBuilder.DropTable(
                name: "Vendas");

            migrationBuilder.DropTable(
                name: "Estoques");

            migrationBuilder.DropTable(
                name: "Fornecedores");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Produtos",
                type: "text",
                nullable: false,
                defaultValue: "Ativo",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProcessamentosProdutosLote",
                type: "text",
                nullable: false,
                defaultValue: "Pendente",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Categorias",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "Ativo",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Produtos",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Ativo");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ProcessamentosProdutosLote",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Pendente");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Categorias",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80,
                oldDefaultValue: "Ativo");

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Endereco_CEP = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    Endereco_Cidade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Endereco_Logradouro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Endereco_Numero = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Endereco_UF = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CNPJ = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    CPF = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Codigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Telefone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estoques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    DataUltimaAtualizacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estoques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estoques_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fornecedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Endereco_CEP = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    Endereco_Cidade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Endereco_Logradouro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Endereco_Numero = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Endereco_UF = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CNPJ = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    Codigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    ClienteCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    DataVenda = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Removido = table.Column<bool>(type: "boolean", nullable: false),
                    RemovidoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimentacoesEstoque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EstoqueId = table.Column<int>(type: "integer", nullable: false),
                    DataMovimentacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Observacao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Origem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    TipoMovimentacao = table.Column<int>(type: "integer", nullable: false),
                    TransacaoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacoesEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimentacoesEstoque_Estoques_EstoqueId",
                        column: x => x.EstoqueId,
                        principalTable: "Estoques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entradas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FornecedorId = table.Column<int>(type: "integer", nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FornecedorCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Observacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entradas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entradas_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensVenda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    VendaId = table.Column<int>(type: "integer", nullable: false),
                    PrecoVenda = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ProdutoCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensVenda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensVenda_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensVenda_Vendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensEntrada",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntradaId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    PrecoCusto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ProdutoCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensEntrada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensEntrada_Entradas_EntradaId",
                        column: x => x.EntradaId,
                        principalTable: "Entradas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensEntrada_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_FornecedorId",
                table: "Entradas",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Estoques_ProdutoId",
                table: "Estoques",
                column: "ProdutoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedores_Codigo",
                table: "Fornecedores",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItensEntrada_EntradaId",
                table: "ItensEntrada",
                column: "EntradaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensEntrada_ProdutoId",
                table: "ItensEntrada",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_ProdutoId",
                table: "ItensVenda",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_VendaId",
                table: "ItensVenda",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_EstoqueId",
                table: "MovimentacoesEstoque",
                column: "EstoqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ClienteId",
                table: "Vendas",
                column: "ClienteId");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AtronStock.Infrastructure.PostgreSqlMigrations.Migrations
{
    public partial class InitialStockPostgreSql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                    table.UniqueConstraint("AK_Categorias_Codigo", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CPF = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    CNPJ = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Endereco_Logradouro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Endereco_Numero = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Endereco_Cidade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Endereco_UF = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Endereco_CEP = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    Email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fornecedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CNPJ = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Endereco_Logradouro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Endereco_Numero = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Endereco_Cidade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Endereco_UF = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Endereco_CEP = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataVenda = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Removido = table.Column<bool>(type: "boolean", nullable: false),
                    RemovidoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    ClienteCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false)
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
                name: "Entradas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FornecedorId = table.Column<int>(type: "integer", nullable: false),
                    FornecedorCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
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
                name: "Estoques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    DataUltimaAtualizacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
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
                name: "ProdutoCategorias",
                columns: table => new
                {
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    CategoriaCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoCategorias", x => new { x.ProdutoId, x.CategoriaId });
                    table.ForeignKey(
                        name: "FK_ProdutoCategorias_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdutoCategorias_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoFornecedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    FornecedorId = table.Column<int>(type: "integer", nullable: false),
                    FornecedorCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ItensVenda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VendaId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    PrecoVenda = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
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
                    ProdutoCodigo = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    PrecoCusto = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "MovimentacoesEstoque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EstoqueId = table.Column<int>(type: "integer", nullable: false),
                    TipoMovimentacao = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    DataMovimentacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Observacao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Origem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                name: "IX_ProdutoCategorias_CategoriaId",
                table: "ProdutoCategorias",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoFornecedor_FornecedorId",
                table: "ProdutoFornecedor",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoFornecedor_ProdutoId",
                table: "ProdutoFornecedor",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ClienteId",
                table: "Vendas",
                column: "ClienteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensEntrada");

            migrationBuilder.DropTable(
                name: "ItensVenda");

            migrationBuilder.DropTable(
                name: "MovimentacoesEstoque");

            migrationBuilder.DropTable(
                name: "ProdutoCategorias");

            migrationBuilder.DropTable(
                name: "ProdutoFornecedor");

            migrationBuilder.DropTable(
                name: "Entradas");

            migrationBuilder.DropTable(
                name: "Vendas");

            migrationBuilder.DropTable(
                name: "Estoques");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Fornecedores");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Produtos");
        }
    }
}

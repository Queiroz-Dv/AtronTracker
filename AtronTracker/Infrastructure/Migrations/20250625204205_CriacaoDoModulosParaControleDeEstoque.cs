using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class CriacaoDoModulosParaControleDeEstoque : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CriadoPor = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    AlteradoPor = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    ModuloCodigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Inativo = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    RemovidoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Removido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RemovidoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Id);
                    table.UniqueConstraint("AK_Categoria_Codigo", x => x.Codigo);
                    table.UniqueConstraint("AK_Categoria_Id_Codigo", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    CNPJ = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Endereco_Logradouro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Endereco_Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Endereco_Cidade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Endereco_UF = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Endereco_CEP = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.UniqueConstraint("AK_Clientes_Id_Codigo", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "Historico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditoriaId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Historico_Auditoria_AuditoriaId",
                        column: x => x.AuditoriaId,
                        principalTable: "Auditoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    CategoriaCodigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    QuantidadeEmEstoque = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Removido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RemovidoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VendaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Id);
                    table.UniqueConstraint("AK_Produto_Codigo", x => x.Codigo);
                    table.UniqueConstraint("AK_Produto_Id_Codigo", x => new { x.Id, x.Codigo });
                    table.ForeignKey(
                        name: "FK_Produto_Categoria_CategoriaCodigo",
                        column: x => x.CategoriaCodigo,
                        principalTable: "Categoria",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataVenda = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantidadeDeProdutoVendido = table.Column<int>(type: "int", nullable: false),
                    PrecoDoProduto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Removido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RemovidoEm = table.Column<DateTime>(type: "datetime", nullable: true),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    ProdutoCodigo = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    CategoriaCodigo = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    ClienteCodigo = table.Column<string>(type: "nvarchar(25)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendas_Categoria_CategoriaId_CategoriaCodigo",
                        columns: x => new { x.CategoriaId, x.CategoriaCodigo },
                        principalTable: "Categoria",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vendas_Clientes_ClienteId_ClienteCodigo",
                        columns: x => new { x.ClienteId, x.ClienteCodigo },
                        principalTable: "Clientes",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vendas_Produto_ProdutoId_ProdutoCodigo",
                        columns: x => new { x.ProdutoId, x.ProdutoCodigo },
                        principalTable: "Produto",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_Codigo",
                table: "Categoria",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Historico_AuditoriaId",
                table: "Historico",
                column: "AuditoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_CategoriaCodigo",
                table: "Produto",
                column: "CategoriaCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_Codigo",
                table: "Produto",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produto_VendaId",
                table: "Produto",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_CategoriaId_CategoriaCodigo",
                table: "Vendas",
                columns: new[] { "CategoriaId", "CategoriaCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ClienteId_ClienteCodigo",
                table: "Vendas",
                columns: new[] { "ClienteId", "ClienteCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ProdutoId_ProdutoCodigo",
                table: "Vendas",
                columns: new[] { "ProdutoId", "ProdutoCodigo" });

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_Vendas_VendaId",
                table: "Produto",
                column: "VendaId",
                principalTable: "Vendas",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produto_Categoria_CategoriaCodigo",
                table: "Produto");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Categoria_CategoriaId_CategoriaCodigo",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Produto_Vendas_VendaId",
                table: "Produto");

            migrationBuilder.DropTable(
                name: "Historico");

            migrationBuilder.DropTable(
                name: "Auditoria");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Vendas");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Produto");
        }
    }
}
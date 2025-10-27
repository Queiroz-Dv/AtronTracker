using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class AtualizandoAtronTracker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Categorias_CategoriaId_CategoriaCodigo",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId_ProdutoCodigo",
                table: "Vendas");

            migrationBuilder.DropTable(
                name: "Historicos");

            migrationBuilder.DropTable(
                name: "ProdutoCategoria");

            migrationBuilder.DropTable(
                name: "PropriedadeDeFluxoModulo");

            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "PropriedadesDeFluxo");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Vendas");

            migrationBuilder.DropTable(
                name: "Clientes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlteradoPor = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    CriadoPor = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Inativo = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    ModuloCodigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RemovidoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
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
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                    table.UniqueConstraint("AK_Categorias_Codigo", x => x.Codigo);
                    table.UniqueConstraint("AK_Categorias_Id_Codigo", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Endereco_CEP = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true),
                    Endereco_Cidade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Endereco_Logradouro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Endereco_Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Endereco_UF = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    CNPJ = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.UniqueConstraint("AK_Clientes_Id_Codigo", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "PropriedadesDeFluxo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropriedadesDeFluxo", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "Historicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditoriaId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Historicos_Auditorias_AuditoriaId",
                        column: x => x.AuditoriaId,
                        principalTable: "Auditorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropriedadeDeFluxoModulo",
                columns: table => new
                {
                    ModuloId = table.Column<int>(type: "int", nullable: false),
                    ModuloCodigo = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    PropriedadeDeFluxoId = table.Column<int>(type: "int", nullable: false),
                    PropriedadeDeFluxoCodigo = table.Column<string>(type: "nvarchar(25)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropriedadeDeFluxoModulo", x => new { x.ModuloId, x.ModuloCodigo, x.PropriedadeDeFluxoId, x.PropriedadeDeFluxoCodigo });
                    table.ForeignKey(
                        name: "FK_PropriedadeDeFluxoModulo_Modulos_ModuloId_ModuloCodigo",
                        columns: x => new { x.ModuloId, x.ModuloCodigo },
                        principalTable: "Modulos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropriedadeDeFluxoModulo_PropriedadesDeFluxo_PropriedadeDeFluxoId_PropriedadeDeFluxoCodigo",
                        columns: x => new { x.PropriedadeDeFluxoId, x.PropriedadeDeFluxoCodigo },
                        principalTable: "PropriedadesDeFluxo",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoCategoria",
                columns: table => new
                {
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    CategoriaCodigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    ProdutoCodigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoCategoria", x => new { x.ProdutoId, x.CategoriaId });
                    table.ForeignKey(
                        name: "FK_ProdutoCategoria_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendaId = table.Column<int>(type: "int", nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuantidadeEmEstoque = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Removido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RemovidoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.UniqueConstraint("AK_Produtos_Codigo", x => x.Codigo);
                    table.UniqueConstraint("AK_Produtos_Id_Codigo", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    CategoriaCodigo = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    ClienteCodigo = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    ProdutoCodigo = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    DataVenda = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrecoDoProduto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantidadeDeProdutoVendido = table.Column<int>(type: "int", nullable: false),
                    Removido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RemovidoEm = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendas_Categorias_CategoriaId_CategoriaCodigo",
                        columns: x => new { x.CategoriaId, x.CategoriaCodigo },
                        principalTable: "Categorias",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vendas_Clientes_ClienteId_ClienteCodigo",
                        columns: x => new { x.ClienteId, x.ClienteCodigo },
                        principalTable: "Clientes",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vendas_Produtos_ProdutoId_ProdutoCodigo",
                        columns: x => new { x.ProdutoId, x.ProdutoCodigo },
                        principalTable: "Produtos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "PropriedadesDeFluxo",
                columns: new[] { "Codigo", "Id" },
                values: new object[,]
                {
                    { "GRAVAR", 1 },
                    { "CONSULTAR", 2 },
                    { "DELETAR", 3 },
                    { "ATUALIZAR", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_Codigo",
                table: "Categorias",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Historicos_AuditoriaId",
                table: "Historicos",
                column: "AuditoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoCategoria_CategoriaId",
                table: "ProdutoCategoria",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_Codigo",
                table: "Produtos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_VendaId",
                table: "Produtos",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropriedadeDeFluxoModulo_PropriedadeDeFluxoId_PropriedadeDeFluxoCodigo",
                table: "PropriedadeDeFluxoModulo",
                columns: new[] { "PropriedadeDeFluxoId", "PropriedadeDeFluxoCodigo" });

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
                name: "FK_ProdutoCategoria_Produtos_ProdutoId",
                table: "ProdutoCategoria",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Vendas_VendaId",
                table: "Produtos",
                column: "VendaId",
                principalTable: "Vendas",
                principalColumn: "Id");
        }
    }
}

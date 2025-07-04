using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class AlterandoMapeamentoProdutoCategoria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Historico_Auditoria_AuditoriaId",
                table: "Historico");

            migrationBuilder.DropForeignKey(
                name: "FK_Produto_Categoria_CategoriaCodigo",
                table: "Produto");

            migrationBuilder.DropForeignKey(
                name: "FK_Produto_Vendas_VendaId",
                table: "Produto");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Categoria_CategoriaId_CategoriaCodigo",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Produto_ProdutoId_ProdutoCodigo",
                table: "Vendas");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Produto_Codigo",
                table: "Produto");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Produto_Id_Codigo",
                table: "Produto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Produto",
                table: "Produto");

            migrationBuilder.DropIndex(
                name: "IX_Produto_CategoriaCodigo",
                table: "Produto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Historico",
                table: "Historico");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Categoria_Codigo",
                table: "Categoria");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Categoria_Id_Codigo",
                table: "Categoria");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categoria",
                table: "Categoria");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Auditoria",
                table: "Auditoria");

            migrationBuilder.DropColumn(
                name: "CategoriaCodigo",
                table: "Produto");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Produto");

            migrationBuilder.RenameTable(
                name: "Produto",
                newName: "Produtos");

            migrationBuilder.RenameTable(
                name: "Historico",
                newName: "Historicos");

            migrationBuilder.RenameTable(
                name: "Categoria",
                newName: "Categorias");

            migrationBuilder.RenameTable(
                name: "Auditoria",
                newName: "Auditorias");

            migrationBuilder.RenameIndex(
                name: "IX_Produto_VendaId",
                table: "Produtos",
                newName: "IX_Produtos_VendaId");

            migrationBuilder.RenameIndex(
                name: "IX_Historico_AuditoriaId",
                table: "Historicos",
                newName: "IX_Historicos_AuditoriaId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Produtos_Codigo",
                table: "Produtos",
                column: "Codigo");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Produtos_Id_Codigo",
                table: "Produtos",
                columns: new[] { "Id", "Codigo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Produtos",
                table: "Produtos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Historicos",
                table: "Historicos",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Categorias_Codigo",
                table: "Categorias",
                column: "Codigo");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Categorias_Id_Codigo",
                table: "Categorias",
                columns: new[] { "Id", "Codigo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Auditorias",
                table: "Auditorias",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProdutoCategoria",
                columns: table => new
                {
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    ProdutoCodigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    CategoriaCodigo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
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
                    table.ForeignKey(
                        name: "FK_ProdutoCategoria_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoCategoria_CategoriaId",
                table: "ProdutoCategoria",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Historicos_Auditorias_AuditoriaId",
                table: "Historicos",
                column: "AuditoriaId",
                principalTable: "Auditorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Vendas_VendaId",
                table: "Produtos",
                column: "VendaId",
                principalTable: "Vendas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Categorias_CategoriaId_CategoriaCodigo",
                table: "Vendas",
                columns: new[] { "CategoriaId", "CategoriaCodigo" },
                principalTable: "Categorias",
                principalColumns: new[] { "Id", "Codigo" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId_ProdutoCodigo",
                table: "Vendas",
                columns: new[] { "ProdutoId", "ProdutoCodigo" },
                principalTable: "Produtos",
                principalColumns: new[] { "Id", "Codigo" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Historicos_Auditorias_AuditoriaId",
                table: "Historicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Vendas_VendaId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Categorias_CategoriaId_CategoriaCodigo",
                table: "Vendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId_ProdutoCodigo",
                table: "Vendas");

            migrationBuilder.DropTable(
                name: "ProdutoCategoria");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Produtos_Codigo",
                table: "Produtos");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Produtos_Id_Codigo",
                table: "Produtos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Produtos",
                table: "Produtos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Historicos",
                table: "Historicos");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Categorias_Codigo",
                table: "Categorias");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Categorias_Id_Codigo",
                table: "Categorias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Auditorias",
                table: "Auditorias");

            migrationBuilder.RenameTable(
                name: "Produtos",
                newName: "Produto");

            migrationBuilder.RenameTable(
                name: "Historicos",
                newName: "Historico");

            migrationBuilder.RenameTable(
                name: "Categorias",
                newName: "Categoria");

            migrationBuilder.RenameTable(
                name: "Auditorias",
                newName: "Auditoria");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_VendaId",
                table: "Produto",
                newName: "IX_Produto_VendaId");

            migrationBuilder.RenameIndex(
                name: "IX_Historicos_AuditoriaId",
                table: "Historico",
                newName: "IX_Historico_AuditoriaId");

            migrationBuilder.AddColumn<string>(
                name: "CategoriaCodigo",
                table: "Produto",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Produto",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Produto_Codigo",
                table: "Produto",
                column: "Codigo");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Produto_Id_Codigo",
                table: "Produto",
                columns: new[] { "Id", "Codigo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Produto",
                table: "Produto",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Historico",
                table: "Historico",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Categoria_Codigo",
                table: "Categoria",
                column: "Codigo");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Categoria_Id_Codigo",
                table: "Categoria",
                columns: new[] { "Id", "Codigo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categoria",
                table: "Categoria",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Auditoria",
                table: "Auditoria",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_CategoriaCodigo",
                table: "Produto",
                column: "CategoriaCodigo");

            migrationBuilder.AddForeignKey(
                name: "FK_Historico_Auditoria_AuditoriaId",
                table: "Historico",
                column: "AuditoriaId",
                principalTable: "Auditoria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_Categoria_CategoriaCodigo",
                table: "Produto",
                column: "CategoriaCodigo",
                principalTable: "Categoria",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_Vendas_VendaId",
                table: "Produto",
                column: "VendaId",
                principalTable: "Vendas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Categoria_CategoriaId_CategoriaCodigo",
                table: "Vendas",
                columns: new[] { "CategoriaId", "CategoriaCodigo" },
                principalTable: "Categoria",
                principalColumns: new[] { "Id", "Codigo" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Produto_ProdutoId_ProdutoCodigo",
                table: "Vendas",
                columns: new[] { "ProdutoId", "ProdutoCodigo" },
                principalTable: "Produto",
                principalColumns: new[] { "Id", "Codigo" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
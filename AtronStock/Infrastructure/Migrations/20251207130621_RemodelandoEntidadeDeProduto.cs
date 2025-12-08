using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtronStock.Infrastructure.Migrations
{
    public partial class RemodelandoEntidadeDeProduto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Vendas_VendaId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_ProdutoId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_VendaId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "ProdutoId",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "Removido",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "RemovidoEm",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "VendaId",
                table: "Produtos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProdutoId",
                table: "Vendas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Removido",
                table: "Produtos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemovidoEm",
                table: "Produtos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendaId",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ProdutoId",
                table: "Vendas",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_VendaId",
                table: "Produtos",
                column: "VendaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Vendas_VendaId",
                table: "Produtos",
                column: "VendaId",
                principalTable: "Vendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Produtos_ProdutoId",
                table: "Vendas",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id");
        }
    }
}

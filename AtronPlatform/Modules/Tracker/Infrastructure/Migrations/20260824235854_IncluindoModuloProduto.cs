using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class IncluindoModuloProduto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Modulos",
                columns: new[] { "Codigo", "Id", "Descricao" },
                values: new object[] { "PRD", 13, "Produtos" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Modulos",
                keyColumns: new[] { "Codigo", "Id" },
                keyValues: new object[] { "PRD", 13 });
        }
    }
}

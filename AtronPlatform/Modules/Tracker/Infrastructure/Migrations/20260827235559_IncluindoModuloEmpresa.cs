using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class IncluindoModuloEmpresa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Modulos",
                columns: new[] { "Codigo", "Id", "Descricao" },
                values: new object[] { "EMP", 14, "Empresa" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Modulos",
                keyColumns: new[] { "Codigo", "Id" },
                keyValues: new object[] { "EMP", 14 });
        }
    }
}

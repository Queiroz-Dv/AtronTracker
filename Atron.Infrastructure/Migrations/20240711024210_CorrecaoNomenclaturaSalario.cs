using Microsoft.EntityFrameworkCore.Migrations;

namespace Atron.Infrastructure.Migrations
{
    public partial class CorrecaoNomenclaturaSalario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantidadeTotal",
                table: "Salarios",
                newName: "SalarioMensal");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SalarioMensal",
                table: "Salarios",
                newName: "QuantidadeTotal");
        }
    }
}

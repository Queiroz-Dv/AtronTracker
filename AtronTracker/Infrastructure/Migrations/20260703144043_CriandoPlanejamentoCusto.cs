using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class CriandoPlanejamentoCusto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanejamentosCusto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    ValorMinimo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorTeto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ApenasDepartamento = table.Column<bool>(type: "bit", nullable: false),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false),
                    DepartamentoCodigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanejamentosCusto", x => new { x.Id, x.Codigo });
                    table.ForeignKey(
                        name: "FK_PlanejamentosCusto_Departamentos_DepartamentoId_DepartamentoCodigo",
                        columns: x => new { x.DepartamentoId, x.DepartamentoCodigo },
                        principalTable: "Departamentos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanejamentosCusto_DepartamentoId_DepartamentoCodigo_Ano",
                table: "PlanejamentosCusto",
                columns: new[] { "DepartamentoId", "DepartamentoCodigo", "Ano" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanejamentosCusto");
        }
    }
}

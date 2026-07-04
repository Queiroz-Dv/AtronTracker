using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class CriandoDetalhesCargoPlanejamentoCusto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanejamentosCustoCargo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanejamentoCustoId = table.Column<int>(type: "int", nullable: false),
                    PlanejamentoCustoCodigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CargoId = table.Column<int>(type: "int", nullable: false),
                    CargoCodigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Detalhado = table.Column<bool>(type: "bit", nullable: false),
                    ValorMinimo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ValorTeto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanejamentosCustoCargo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanejamentosCustoCargo_Cargos_CargoId_CargoCodigo",
                        columns: x => new { x.CargoId, x.CargoCodigo },
                        principalTable: "Cargos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanejamentosCustoCargo_PlanejamentosCusto_PlanejamentoCustoId_PlanejamentoCustoCodigo",
                        columns: x => new { x.PlanejamentoCustoId, x.PlanejamentoCustoCodigo },
                        principalTable: "PlanejamentosCusto",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanejamentosCustoCargo_CargoId_CargoCodigo",
                table: "PlanejamentosCustoCargo",
                columns: new[] { "CargoId", "CargoCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanejamentosCustoCargo_PlanejamentoCustoId_PlanejamentoCustoCodigo_CargoId_CargoCodigo",
                table: "PlanejamentosCustoCargo",
                columns: new[] { "PlanejamentoCustoId", "PlanejamentoCustoCodigo", "CargoId", "CargoCodigo" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanejamentosCustoCargo");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddDepartamentoTenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {          
            migrationBuilder.CreateTable(
                name: "DepartamentoTenant",
                columns: table => new
                {
                    DepartamentoId = table.Column<int>(type: "integer", nullable: false),
                    DepartamentoCodigo = table.Column<string>(type: "text", nullable: false),
                    WorkspaceId = table.Column<int>(type: "integer", nullable: false),
                    WorkspaceCodigo = table.Column<string>(type: "character varying(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartamentoTenant", x => new { x.WorkspaceId, x.WorkspaceCodigo, x.DepartamentoId, x.DepartamentoCodigo });
                    table.ForeignKey(
                        name: "FK_DepartamentoTenant_Departamentos_DepartamentoId_Departament~",
                        columns: x => new { x.DepartamentoId, x.DepartamentoCodigo },
                        principalTable: "Departamentos",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartamentoTenant_Workspaces_WorkspaceId_WorkspaceCodigo",
                        columns: x => new { x.WorkspaceId, x.WorkspaceCodigo },
                        principalTable: "Workspaces",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartamentoTenant_DepartamentoId_DepartamentoCodigo",
                table: "DepartamentoTenant",
                columns: new[] { "DepartamentoId", "DepartamentoCodigo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartamentoTenant");

            migrationBuilder.DropColumn(
                name: "DepartamentoId_Antigo",
                table: "Cargos");

            migrationBuilder.AlterColumn<string>(
                name: "DepartamentoCodigo",
                table: "UsuarioCargoDepartamento",
                type: "character varying(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CargoCodigo",
                table: "UsuarioCargoDepartamento",
                type: "character varying(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Departamentos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "DepartamentoCodigo",
                table: "Cargos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Cargos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}

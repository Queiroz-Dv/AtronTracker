using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtronTracker.Infrastructure.Migrations.Migrations
{
    public partial class RemodelandoModeloTarefaDestinoEscopo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_Usuarios_UsuarioId_UsuarioCodigo",
                table: "Tarefas");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Tarefas",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "CargoCodigo",
                table: "Tarefas",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CargoId",
                table: "Tarefas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartamentoCodigo",
                table: "Tarefas",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartamentoId",
                table: "Tarefas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DestinoInicial",
                table: "Tarefas",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Identificador",
                table: "Tarefas",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("UPDATE \"Tarefas\" SET \"Identificador\" = \"Id\" WHERE \"Identificador\" IS NULL;");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_CargoId_CargoCodigo",
                table: "Tarefas",
                columns: new[] { "CargoId", "CargoCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_DepartamentoId_DepartamentoCodigo",
                table: "Tarefas",
                columns: new[] { "DepartamentoId", "DepartamentoCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_Identificador",
                table: "Tarefas",
                column: "Identificador");

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_Cargos_CargoId_CargoCodigo",
                table: "Tarefas",
                columns: new[] { "CargoId", "CargoCodigo" },
                principalTable: "Cargos",
                principalColumns: new[] { "Id", "Codigo" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_Departamentos_DepartamentoId_DepartamentoCodigo",
                table: "Tarefas",
                columns: new[] { "DepartamentoId", "DepartamentoCodigo" },
                principalTable: "Departamentos",
                principalColumns: new[] { "Id", "Codigo" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_Usuarios_UsuarioId_UsuarioCodigo",
                table: "Tarefas",
                columns: new[] { "UsuarioId", "UsuarioCodigo" },
                principalTable: "Usuarios",
                principalColumns: new[] { "Id", "Codigo" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_Cargos_CargoId_CargoCodigo",
                table: "Tarefas");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_Departamentos_DepartamentoId_DepartamentoCodigo",
                table: "Tarefas");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_Usuarios_UsuarioId_UsuarioCodigo",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_CargoId_CargoCodigo",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_DepartamentoId_DepartamentoCodigo",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_Identificador",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "CargoCodigo",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "CargoId",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "DepartamentoCodigo",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "DepartamentoId",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "DestinoInicial",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "Identificador",
                table: "Tarefas");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Tarefas",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_Usuarios_UsuarioId_UsuarioCodigo",
                table: "Tarefas",
                columns: new[] { "UsuarioId", "UsuarioCodigo" },
                principalTable: "Usuarios",
                principalColumns: new[] { "Id", "Codigo" });
        }
    }
}

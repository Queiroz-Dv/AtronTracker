using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class UpdateIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cargos_Departamentos_DepartmentoId_DepartamentoCodigo",
                table: "Cargos");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_Usuarios_UsuarioId_UsuarioCodigo",
                table: "Tarefas");

            migrationBuilder.RenameColumn(
                name: "DepartmentoId",
                table: "Cargos",
                newName: "DepartamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Cargos_DepartmentoId_DepartamentoCodigo",
                table: "Cargos",
                newName: "IX_Cargos_DepartamentoId_DepartamentoCodigo");

            migrationBuilder.AlterColumn<string>(
                name: "DepartamentoCodigo",
                table: "UsuarioCargoDepartamento",
                type: "nvarchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "CargoCodigo",
                table: "UsuarioCargoDepartamento",
                type: "nvarchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCodigo",
                table: "Tarefas",
                type: "nvarchar(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "Conteudo",
                table: "Tarefas",
                type: "nvarchar(2500)",
                maxLength: 2500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2500)",
                oldMaxLength: 2500);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "TarefaEstados",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Departamentos",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "DepartamentoCodigo",
                table: "Cargos",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Cargos",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AddForeignKey(
                name: "FK_Cargos_Departamentos_DepartamentoId_DepartamentoCodigo",
                table: "Cargos",
                columns: new[] { "DepartamentoId", "DepartamentoCodigo" },
                principalTable: "Departamentos",
                principalColumns: new[] { "Id", "Codigo" });

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_Usuarios_UsuarioId_UsuarioCodigo",
                table: "Tarefas",
                columns: new[] { "UsuarioId", "UsuarioCodigo" },
                principalTable: "Usuarios",
                principalColumns: new[] { "Id", "Codigo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cargos_Departamentos_DepartamentoId_DepartamentoCodigo",
                table: "Cargos");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_Usuarios_UsuarioId_UsuarioCodigo",
                table: "Tarefas");

            migrationBuilder.RenameColumn(
                name: "DepartamentoId",
                table: "Cargos",
                newName: "DepartmentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Cargos_DepartamentoId_DepartamentoCodigo",
                table: "Cargos",
                newName: "IX_Cargos_DepartmentoId_DepartamentoCodigo");

            migrationBuilder.AlterColumn<string>(
                name: "DepartamentoCodigo",
                table: "UsuarioCargoDepartamento",
                type: "nvarchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "CargoCodigo",
                table: "UsuarioCargoDepartamento",
                type: "nvarchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCodigo",
                table: "Tarefas",
                type: "nvarchar(10)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Conteudo",
                table: "Tarefas",
                type: "nvarchar(2500)",
                maxLength: 2500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2500)",
                oldMaxLength: 2500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "TarefaEstados",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Departamentos",
                type: "nvarchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "DepartamentoCodigo",
                table: "Cargos",
                type: "nvarchar(10)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Cargos",
                type: "nvarchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddForeignKey(
                name: "FK_Cargos_Departamentos_DepartmentoId_DepartamentoCodigo",
                table: "Cargos",
                columns: new[] { "DepartmentoId", "DepartamentoCodigo" },
                principalTable: "Departamentos",
                principalColumns: new[] { "Id", "Codigo" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_Usuarios_UsuarioId_UsuarioCodigo",
                table: "Tarefas",
                columns: new[] { "UsuarioId", "UsuarioCodigo" },
                principalTable: "Usuarios",
                principalColumns: new[] { "Id", "Codigo" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class ReconfiguracaoDosModulos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdSequencial = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => new { x.Id, x.Codigo });
                });

            migrationBuilder.CreateTable(
                name: "Meses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    IdSequencial = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DataInicial = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFinal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PermissaoEstadoId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: true),
                    QuantidadeDeDias = table.Column<int>(type: "int", maxLength: 365, nullable: false),
                    IdSequencial = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissoesEstados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissoesEstados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TarefaEstados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarefaEstados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cargos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentoId = table.Column<int>(type: "int", nullable: false),
                    DepartamentoCodigo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IdSequencial = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargos", x => new { x.Id, x.Codigo });
                    table.ForeignKey(
                        name: "FK_Cargos_Departamentos_DepartmentoId_DepartamentoCodigo",
                        columns: x => new { x.DepartmentoId, x.DepartamentoCodigo },
                        principalTable: "Departamentos",
                        principalColumns: new[] { "Id", "Codigo" });
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Sobrenome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalarioAtual = table.Column<int>(type: "int", nullable: false),
                    CargoId = table.Column<int>(type: "int", nullable: false),
                    CargoCodigo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false),
                    DepartamentoCodigo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdSequencial = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => new { x.Id, x.Codigo });
                    table.ForeignKey(
                        name: "FK_Usuarios_Cargos_CargoId_CargoCodigo",
                        columns: x => new { x.CargoId, x.CargoCodigo },
                        principalTable: "Cargos",
                        principalColumns: new[] { "Id", "Codigo" });
                    table.ForeignKey(
                        name: "FK_Usuarios_Departamentos_DepartamentoId_DepartamentoCodigo",
                        columns: x => new { x.DepartamentoId, x.DepartamentoCodigo },
                        principalTable: "Departamentos",
                        principalColumns: new[] { "Id", "Codigo" });
                });

            migrationBuilder.CreateTable(
                name: "Salarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SalarioMensal = table.Column<int>(type: "int", nullable: false),
                    Ano = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MesId = table.Column<int>(type: "int", nullable: false),
                    IdSequencial = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Salarios_Meses_MesId",
                        column: x => x.MesId,
                        principalTable: "Meses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Salarios_Usuarios_UsuarioId_UsuarioCodigo",
                        columns: x => new { x.UsuarioId, x.UsuarioCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tarefas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Conteudo = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: true),
                    DataInicial = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFinal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TarefaEstadoId = table.Column<int>(type: "int", nullable: false),
                    IdSequencial = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarefas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tarefas_Usuarios_UsuarioId_UsuarioCodigo",
                        columns: x => new { x.UsuarioId, x.UsuarioCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" });
                });

            migrationBuilder.InsertData(
                table: "Meses",
                columns: new[] { "Id", "Descricao", "IdSequencial" },
                values: new object[,]
                {
                    { 1, "Janeiro", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, "Fevereiro", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 3, "Março", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 4, "Abril", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 5, "Maio", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 6, "Junho", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 7, "Julho", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 8, "Agosto", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 9, "Setembro", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 10, "Outubro", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 11, "Novembro", new Guid("00000000-0000-0000-0000-000000000000") },
                    { 12, "Dezembro", new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                table: "PermissoesEstados",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 1, "Em atividade" },
                    { 2, "Aprovada" },
                    { 3, "Desaprovada" }
                });

            migrationBuilder.InsertData(
                table: "TarefaEstados",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 1, "Em atividade" },
                    { 2, "Pendente de aprovação" },
                    { 3, "Entregue" },
                    { 4, "Finalizada" },
                    { 5, "Iniciada" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_DepartmentoId_DepartamentoCodigo",
                table: "Cargos",
                columns: new[] { "DepartmentoId", "DepartamentoCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Salarios_MesId",
                table: "Salarios",
                column: "MesId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Salarios_UsuarioId_UsuarioCodigo",
                table: "Salarios",
                columns: new[] { "UsuarioId", "UsuarioCodigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_UsuarioId_UsuarioCodigo",
                table: "Tarefas",
                columns: new[] { "UsuarioId", "UsuarioCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CargoId_CargoCodigo",
                table: "Usuarios",
                columns: new[] { "CargoId", "CargoCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_DepartamentoId_DepartamentoCodigo",
                table: "Usuarios",
                columns: new[] { "DepartamentoId", "DepartamentoCodigo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissoes");

            migrationBuilder.DropTable(
                name: "PermissoesEstados");

            migrationBuilder.DropTable(
                name: "Salarios");

            migrationBuilder.DropTable(
                name: "TarefaEstados");

            migrationBuilder.DropTable(
                name: "Tarefas");

            migrationBuilder.DropTable(
                name: "Meses");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Cargos");

            migrationBuilder.DropTable(
                name: "Departamentos");
        }
    }
}

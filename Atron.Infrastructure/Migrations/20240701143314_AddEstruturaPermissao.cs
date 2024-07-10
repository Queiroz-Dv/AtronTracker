using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Atron.Infrastructure.Migrations
{
    public partial class AddEstruturaPermissao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    DataInicial = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFinal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PermissaoEstadoId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: true),
                    QuantidadeDeDias = table.Column<int>(type: "int", maxLength: 365, nullable: false)
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

            migrationBuilder.InsertData(
                table: "PermissoesEstados",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 1, "Em atividade" });

            migrationBuilder.InsertData(
                table: "PermissoesEstados",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 2, "Aprovada" });

            migrationBuilder.InsertData(
                table: "PermissoesEstados",
                columns: new[] { "Id", "Descricao" },
                values: new object[] { 3, "Desaprovada" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissoes");

            migrationBuilder.DropTable(
                name: "PermissoesEstados");
        }
    }
}

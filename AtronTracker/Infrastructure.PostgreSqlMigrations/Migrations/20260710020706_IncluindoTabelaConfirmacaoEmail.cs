using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AtronTracker.Infrastructure.PostgreSqlMigrations.Migrations
{
    public partial class IncluindoTabelaConfirmacaoEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfirmacoesEmail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioCodigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IdentificadorHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExpiraEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ConfirmadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmacoesEmail", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmacoesEmail_UsuarioCodigo_ExpiraEm_ConfirmadoEm",
                table: "ConfirmacoesEmail",
                columns: new[] { "UsuarioCodigo", "ExpiraEm", "ConfirmadoEm" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfirmacoesEmail");
        }
    }
}

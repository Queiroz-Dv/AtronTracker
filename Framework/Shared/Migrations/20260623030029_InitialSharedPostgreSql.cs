using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Shared.Migrations
{
    public partial class InitialSharedPostgreSql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "HistoricoSeq");

            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() AT TIME ZONE 'UTC'"),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CriadoPor = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    AlteradoPor = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    CodigoRegistro = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Contexto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RemovidoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Historicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoHistorico = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('\"HistoricoSeq\"')"),
                    Contexto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CodigoRegistro = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() AT TIME ZONE 'UTC'"),
                    Descricao = table.Column<string>(type: "character varying(1500)", maxLength: 1500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historicos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_Contexto_CodigoRegistro",
                table: "Auditorias",
                columns: new[] { "Contexto", "CodigoRegistro" });

            migrationBuilder.CreateIndex(
                name: "IX_Historicos_Contexto_CodigoRegistro",
                table: "Historicos",
                columns: new[] { "Contexto", "CodigoRegistro" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "Historicos");

            migrationBuilder.DropSequence(
                name: "HistoricoSeq");
        }
    }
}

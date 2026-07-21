using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtronNotificacoes.Infrastructure.Migrations.Migrations
{
    public partial class InitialNotificacoes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "notificacoes_ids",
                startValue: 1000001L);

            migrationBuilder.CreateTable(
                name: "Notificacoes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('notificacoes_ids')"),
                    DestinatarioCodigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ModuloOrigem = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TipoEvento = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Titulo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Mensagem = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UrlDestino = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ReferenciaExterna = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ChaveIdempotencia = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    CorrelacaoId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Lida = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DataExclusao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DataLeitura = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacoes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_DestinatarioCodigo_DataExclusao_Lida_DataCriacao",
                table: "Notificacoes",
                columns: new[] { "DestinatarioCodigo", "DataExclusao", "Lida", "DataCriacao" });

            migrationBuilder.CreateIndex(
                name: "IX_Notificacoes_ModuloOrigem_ChaveIdempotencia",
                table: "Notificacoes",
                columns: new[] { "ModuloOrigem", "ChaveIdempotencia" },
                unique: true,
                filter: "\"ChaveIdempotencia\" IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notificacoes");

            migrationBuilder.DropSequence(
                name: "notificacoes_ids");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AtronTracker.Infrastructure.Migrations.Migrations
{
    public partial class CriandoNotificacoesInternas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificacoesInternas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioCodigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Titulo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Mensagem = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Modulo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TipoEvento = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    UrlDestino = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TarefaId = table.Column<int>(type: "integer", nullable: true),
                    Lida = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataLeitura = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacoesInternas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificacoesInternas_Tarefas_TarefaId",
                        column: x => x.TarefaId,
                        principalTable: "Tarefas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificacoesInternas_Usuarios_UsuarioId_UsuarioCodigo",
                        columns: x => new { x.UsuarioId, x.UsuarioCodigo },
                        principalTable: "Usuarios",
                        principalColumns: new[] { "Id", "Codigo" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificacoesInternas_TarefaId",
                table: "NotificacoesInternas",
                column: "TarefaId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacoesInternas_UsuarioId_UsuarioCodigo_Lida",
                table: "NotificacoesInternas",
                columns: new[] { "UsuarioId", "UsuarioCodigo", "Lida" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificacoesInternas");
        }
    }
}

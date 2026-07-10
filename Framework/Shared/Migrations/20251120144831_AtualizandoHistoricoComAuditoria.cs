using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations
{
    public partial class AtualizandoHistoricoComAuditoria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Historicos_Auditorias_AuditoriaId",
                schema: "AtronShared",
                table: "Historicos");

            migrationBuilder.DropIndex(
                name: "IX_Historicos_AuditoriaId",
                schema: "AtronShared",
                table: "Historicos");

            migrationBuilder.DropColumn(
                name: "AuditoriaId",
                schema: "AtronShared",
                table: "Historicos");

            migrationBuilder.DropColumn(
                name: "Inativo",
                schema: "AtronShared",
                table: "Auditorias");

            migrationBuilder.CreateSequence(
                name: "HistoricoSeq",
                schema: "AtronShared");

            migrationBuilder.AddColumn<long>(
                name: "CodigoHistorico",
                schema: "AtronShared",
                table: "Historicos",
                type: "bigint",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR AtronShared.HistoricoSeq");

            migrationBuilder.AddColumn<string>(
                name: "CodigoRegistro",
                schema: "AtronShared",
                table: "Historicos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                schema: "AtronShared",
                table: "Historicos",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "CodigoRegistro",
                schema: "AtronShared",
                table: "Auditorias",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Historicos_CodigoRegistro",
                schema: "AtronShared",
                table: "Historicos",
                column: "CodigoRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_CodigoRegistro",
                schema: "AtronShared",
                table: "Auditorias",
                column: "CodigoRegistro");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Historicos_CodigoRegistro",
                schema: "AtronShared",
                table: "Historicos");

            migrationBuilder.DropIndex(
                name: "IX_Auditorias_CodigoRegistro",
                schema: "AtronShared",
                table: "Auditorias");

            migrationBuilder.DropSequence(
                name: "HistoricoSeq",
                schema: "AtronShared");

            migrationBuilder.DropColumn(
                name: "CodigoHistorico",
                schema: "AtronShared",
                table: "Historicos");

            migrationBuilder.DropColumn(
                name: "CodigoRegistro",
                schema: "AtronShared",
                table: "Historicos");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                schema: "AtronShared",
                table: "Historicos");

            migrationBuilder.DropColumn(
                name: "CodigoRegistro",
                schema: "AtronShared",
                table: "Auditorias");

            migrationBuilder.AddColumn<int>(
                name: "AuditoriaId",
                schema: "AtronShared",
                table: "Historicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Inativo",
                schema: "AtronShared",
                table: "Auditorias",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Historicos_AuditoriaId",
                schema: "AtronShared",
                table: "Historicos",
                column: "AuditoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Historicos_Auditorias_AuditoriaId",
                schema: "AtronShared",
                table: "Historicos",
                column: "AuditoriaId",
                principalSchema: "AtronShared",
                principalTable: "Auditorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

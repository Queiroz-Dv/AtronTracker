using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtronStock.Infrastructure.Migrations
{
    public partial class FortalecendoReservaProcessamentosProdutosLote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReservaExpiraEm",
                table: "ProcessamentosProdutosLote",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReservadoEm",
                table: "ProcessamentosProdutosLote",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tentativas",
                table: "ProcessamentosProdutosLote",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TokenReserva",
                table: "ProcessamentosProdutosLote",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentosProdutosLote_Status_ReservaExpiraEm_Id",
                table: "ProcessamentosProdutosLote",
                columns: new[] { "Status", "ReservaExpiraEm", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessamentosProdutosLote_Status_ReservaExpiraEm_Id",
                table: "ProcessamentosProdutosLote");

            migrationBuilder.DropColumn(
                name: "ReservaExpiraEm",
                table: "ProcessamentosProdutosLote");

            migrationBuilder.DropColumn(
                name: "ReservadoEm",
                table: "ProcessamentosProdutosLote");

            migrationBuilder.DropColumn(
                name: "Tentativas",
                table: "ProcessamentosProdutosLote");

            migrationBuilder.DropColumn(
                name: "TokenReserva",
                table: "ProcessamentosProdutosLote");
        }
    }
}

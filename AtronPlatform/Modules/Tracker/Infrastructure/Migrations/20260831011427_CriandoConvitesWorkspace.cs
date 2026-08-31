using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class CriandoConvitesWorkspace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConvitesWorkspace",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkspaceId = table.Column<int>(type: "integer", nullable: false),
                    IdentificadorHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RemetenteCodigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ExpiraEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UtilizadoPorUsuarioCodigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    UtilizadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvitesWorkspace", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConvitesWorkspace_Usuarios_RemetenteCodigo",
                        column: x => x.RemetenteCodigo,
                        principalTable: "Usuarios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConvitesWorkspace_Usuarios_UtilizadoPorUsuarioCodigo",
                        column: x => x.UtilizadoPorUsuarioCodigo,
                        principalTable: "Usuarios",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConvitesWorkspace_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConvitesWorkspace_IdentificadorHash",
                table: "ConvitesWorkspace",
                column: "IdentificadorHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConvitesWorkspace_RemetenteCodigo",
                table: "ConvitesWorkspace",
                column: "RemetenteCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_ConvitesWorkspace_UtilizadoPorUsuarioCodigo",
                table: "ConvitesWorkspace",
                column: "UtilizadoPorUsuarioCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_ConvitesWorkspace_WorkspaceId_ExpiraEm_UtilizadoEm",
                table: "ConvitesWorkspace",
                columns: new[] { "WorkspaceId", "ExpiraEm", "UtilizadoEm" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConvitesWorkspace");
        }
    }
}

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using AtronTracker.Infrastructure.Context;

#nullable disable

namespace AtronTracker.Infrastructure.Migrations.Migrations
{
    [DbContext(typeof(AtronDbContext))]
    [Migration("20260720200000_RemoverNotificacoesInternasLegadas")]
    public partial class RemoverNotificacoesInternasLegadas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS \"NotificacoesInternas\";");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class AlterandoPropsUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoReativacao",
                table: "Usuarios",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Inativo",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefreshTokenExpireTime",
                table: "AppUser",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoReativacao",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Inativo",
                table: "Usuarios");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefreshTokenExpireTime",
                table: "AppUser",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtronStock.Infrastructure.Migrations
{
    public partial class PersistindoEnumsComoTexto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Produtos\" ALTER COLUMN \"Status\" DROP DEFAULT, ALTER COLUMN \"Status\" TYPE varchar(30) USING CASE \"Status\" WHEN 1 THEN 'Ativo' WHEN 2 THEN 'Baixado' ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"ProcessamentosProdutosLote\" ALTER COLUMN \"Status\" DROP DEFAULT, ALTER COLUMN \"Status\" TYPE varchar(30) USING CASE \"Status\" WHEN 1 THEN 'Pendente' WHEN 2 THEN 'EmExecucao' WHEN 3 THEN 'Concluido' WHEN 4 THEN 'Falha' ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"MovimentacoesEstoque\" ALTER COLUMN \"TipoMovimentacao\" TYPE varchar(30) USING CASE \"TipoMovimentacao\" WHEN 1 THEN 'Entrada' WHEN 2 THEN 'Saida' ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"Clientes\" ALTER COLUMN \"Status\" TYPE varchar(30) USING CASE \"Status\" WHEN 1 THEN 'Ativo' WHEN 2 THEN 'Inativo' WHEN 3 THEN 'Removido' ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"Categorias\" ALTER COLUMN \"Status\" DROP DEFAULT, ALTER COLUMN \"Status\" TYPE varchar(30) USING CASE \"Status\" WHEN 1 THEN 'Ativo' WHEN 2 THEN 'Inativo' WHEN 3 THEN 'Removido' ELSE NULL END;");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Produtos",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Ativo",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProcessamentosProdutosLote",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Pendente",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "TipoMovimentacao",
                table: "MovimentacoesEstoque",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Clientes",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Categorias",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Ativo",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Produtos\" ALTER COLUMN \"Status\" DROP DEFAULT, ALTER COLUMN \"Status\" TYPE integer USING CASE \"Status\" WHEN 'Ativo' THEN 1 WHEN 'Baixado' THEN 2 ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"ProcessamentosProdutosLote\" ALTER COLUMN \"Status\" DROP DEFAULT, ALTER COLUMN \"Status\" TYPE integer USING CASE \"Status\" WHEN 'Pendente' THEN 1 WHEN 'EmExecucao' THEN 2 WHEN 'Concluido' THEN 3 WHEN 'Falha' THEN 4 ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"MovimentacoesEstoque\" ALTER COLUMN \"TipoMovimentacao\" TYPE integer USING CASE \"TipoMovimentacao\" WHEN 'Entrada' THEN 1 WHEN 'Saida' THEN 2 ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"Clientes\" ALTER COLUMN \"Status\" TYPE integer USING CASE \"Status\" WHEN 'Ativo' THEN 1 WHEN 'Inativo' THEN 2 WHEN 'Removido' THEN 3 ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"Categorias\" ALTER COLUMN \"Status\" DROP DEFAULT, ALTER COLUMN \"Status\" TYPE integer USING CASE \"Status\" WHEN 'Ativo' THEN 1 WHEN 'Inativo' THEN 2 WHEN 'Removido' THEN 3 ELSE NULL END;");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Produtos",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldDefaultValue: "Ativo");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ProcessamentosProdutosLote",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldDefaultValue: "Pendente");

            migrationBuilder.AlterColumn<int>(
                name: "TipoMovimentacao",
                table: "MovimentacoesEstoque",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Clientes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Categorias",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldDefaultValue: "Ativo");
        }
    }
}


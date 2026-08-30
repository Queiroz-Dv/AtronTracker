using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class PersistindoEnumsComoTexto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Workspaces\" ALTER COLUMN \"Tipo\" DROP DEFAULT, ALTER COLUMN \"Tipo\" TYPE varchar(30) USING CASE \"Tipo\" WHEN 1 THEN 'Pessoal' WHEN 2 THEN 'Agencia' WHEN 3 THEN 'Empresa' ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"Tarefas\" ALTER COLUMN \"DestinoInicial\" DROP DEFAULT, ALTER COLUMN \"DestinoInicial\" TYPE varchar(40) USING CASE \"DestinoInicial\" WHEN 1 THEN 'Usuario' WHEN 2 THEN 'DepartamentoCargo' WHEN 3 THEN 'Equipe' ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"TarefaMovimentacoes\" ALTER COLUMN \"Tipo\" TYPE varchar(80) USING CASE \"Tipo\" WHEN 1 THEN 'Criação' WHEN 2 THEN 'Atualização' WHEN 3 THEN 'Obtenção' WHEN 4 THEN 'Solicitação de Obtenção' WHEN 5 THEN 'Aprovação de Obtenção' WHEN 6 THEN 'Recusa de Obtenção' ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"SolicitacoesObtencaoTarefa\" ALTER COLUMN \"Status\" DROP DEFAULT, ALTER COLUMN \"Status\" TYPE varchar(30) USING CASE \"Status\" WHEN 1 THEN 'Pendente' WHEN 2 THEN 'Aprovada' WHEN 3 THEN 'Recusada' ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"Empresas\" ALTER COLUMN \"Status\" DROP DEFAULT, ALTER COLUMN \"Status\" TYPE varchar(30) USING CASE \"Status\" WHEN 1 THEN 'Ativa' WHEN 2 THEN 'Suspensa' ELSE NULL END;");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Workspaces",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "DestinoInicial",
                table: "Tarefas",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "Usuario",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "TarefaMovimentacoes",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "SolicitacoesObtencaoTarefa",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Empresas",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Workspaces\" ALTER COLUMN \"Tipo\" DROP DEFAULT, ALTER COLUMN \"Tipo\" TYPE integer USING CASE \"Tipo\" WHEN 'Pessoal' THEN 1 WHEN 'Agencia' THEN 2 WHEN 'Empresa' THEN 3 ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"Tarefas\" ALTER COLUMN \"DestinoInicial\" DROP DEFAULT, ALTER COLUMN \"DestinoInicial\" TYPE integer USING CASE \"DestinoInicial\" WHEN 'Usuario' THEN 1 WHEN 'DepartamentoCargo' THEN 2 WHEN 'Equipe' THEN 3 ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"TarefaMovimentacoes\" ALTER COLUMN \"Tipo\" TYPE integer USING CASE \"Tipo\" WHEN 'Criação' THEN 1 WHEN 'Atualização' THEN 2 WHEN 'Obtenção' THEN 3 WHEN 'Solicitação de Obtenção' THEN 4 WHEN 'Aprovação de Obtenção' THEN 5 WHEN 'Recusa de Obtenção' THEN 6 ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"SolicitacoesObtencaoTarefa\" ALTER COLUMN \"Status\" DROP DEFAULT, ALTER COLUMN \"Status\" TYPE integer USING CASE \"Status\" WHEN 'Pendente' THEN 1 WHEN 'Aprovada' THEN 2 WHEN 'Recusada' THEN 3 ELSE NULL END;");
            migrationBuilder.Sql("ALTER TABLE \"Empresas\" ALTER COLUMN \"Status\" DROP DEFAULT, ALTER COLUMN \"Status\" TYPE integer USING CASE \"Status\" WHEN 'Ativa' THEN 1 WHEN 'Suspensa' THEN 2 ELSE NULL END;");

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "Workspaces",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<int>(
                name: "DestinoInicial",
                table: "Tarefas",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40,
                oldDefaultValue: "Usuario");

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "TarefaMovimentacoes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "SolicitacoesObtencaoTarefa",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Empresas",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);
        }
    }
}


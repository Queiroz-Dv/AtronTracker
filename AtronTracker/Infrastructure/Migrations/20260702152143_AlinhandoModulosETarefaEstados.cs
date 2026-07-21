using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtronTracker.Infrastructure.Migrations.Migrations
{
    public partial class AlinhandoModulosETarefaEstados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM ""Modulos"" WHERE ""Id"" = 6 AND ""Codigo"" = 'PERF') THEN
        UPDATE ""Modulos"" SET ""Descricao"" = 'Perfil de acesso' WHERE ""Id"" = 6 AND ""Codigo"" = 'PERF';
    ELSIF EXISTS (SELECT 1 FROM ""Modulos"" WHERE ""Id"" = 6 AND ""Codigo"" = 'PAC') THEN
        UPDATE ""Modulos"" SET ""Codigo"" = 'PERF', ""Descricao"" = 'Perfil de acesso' WHERE ""Id"" = 6 AND ""Codigo"" = 'PAC';
    ELSE
        INSERT INTO ""Modulos"" (""Codigo"", ""Id"", ""Descricao"") VALUES ('PERF', 6, 'Perfil de acesso');
    END IF;

    IF EXISTS (SELECT 1 FROM ""Modulos"" WHERE ""Id"" = 10 AND ""Codigo"" = 'RPERFUSR') THEN
        UPDATE ""Modulos"" SET ""Descricao"" = 'Relacionamento de perfil e usuários' WHERE ""Id"" = 10 AND ""Codigo"" = 'RPERFUSR';
    ELSE
        INSERT INTO ""Modulos"" (""Codigo"", ""Id"", ""Descricao"") VALUES ('RPERFUSR', 10, 'Relacionamento de perfil e usuários');
    END IF;
END $$;
");

            migrationBuilder.CreateTable(
                name: "TarefaEstados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarefaEstados", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TarefaEstados",
                columns: new[] { "Id", "Descricao" },
                values: new object[,]
                {
                    { 1, "Em atividade" },
                    { 2, "Pendente de aprovação" },
                    { 3, "Entregue" },
                    { 4, "Finalizada" },
                    { 5, "Iniciada" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_TarefaEstadoId",
                table: "Tarefas",
                column: "TarefaEstadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_TarefaEstados_TarefaEstadoId",
                table: "Tarefas",
                column: "TarefaEstadoId",
                principalTable: "TarefaEstados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_TarefaEstados_TarefaEstadoId",
                table: "Tarefas");

            migrationBuilder.DropTable(
                name: "TarefaEstados");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_TarefaEstadoId",
                table: "Tarefas");

            migrationBuilder.Sql(@"
DO $$
BEGIN
    DELETE FROM ""Modulos"" WHERE ""Id"" = 10 AND ""Codigo"" = 'RPERFUSR';

    IF EXISTS (SELECT 1 FROM ""Modulos"" WHERE ""Id"" = 6 AND ""Codigo"" = 'PERF') THEN
        UPDATE ""Modulos"" SET ""Codigo"" = 'PAC', ""Descricao"" = 'Políticas e Acessos' WHERE ""Id"" = 6 AND ""Codigo"" = 'PERF';
    ELSIF EXISTS (SELECT 1 FROM ""Modulos"" WHERE ""Id"" = 6 AND ""Codigo"" = 'PAC') THEN
        UPDATE ""Modulos"" SET ""Descricao"" = 'Políticas e Acessos' WHERE ""Id"" = 6 AND ""Codigo"" = 'PAC';
    ELSE
        INSERT INTO ""Modulos"" (""Codigo"", ""Id"", ""Descricao"") VALUES ('PAC', 6, 'Políticas e Acessos');
    END IF;
END $$;
");
        }
    }
}

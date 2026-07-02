using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AlinhandoModulosETarefaEstados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM [Modulos] WHERE [Id] = 6 AND [Codigo] = N'PERF')
    UPDATE [Modulos] SET [Descricao] = N'Perfil de acesso' WHERE [Id] = 6 AND [Codigo] = N'PERF';
ELSE IF EXISTS (SELECT 1 FROM [Modulos] WHERE [Id] = 6 AND [Codigo] = N'PAC')
    UPDATE [Modulos] SET [Codigo] = N'PERF', [Descricao] = N'Perfil de acesso' WHERE [Id] = 6 AND [Codigo] = N'PAC';
ELSE
    INSERT INTO [Modulos] ([Codigo], [Id], [Descricao]) VALUES (N'PERF', 6, N'Perfil de acesso');

IF EXISTS (SELECT 1 FROM [Modulos] WHERE [Id] = 10 AND [Codigo] = N'RPERFUSR')
    UPDATE [Modulos] SET [Descricao] = N'Relacionamento de perfil e usuários' WHERE [Id] = 10 AND [Codigo] = N'RPERFUSR';
ELSE
    INSERT INTO [Modulos] ([Codigo], [Id], [Descricao]) VALUES (N'RPERFUSR', 10, N'Relacionamento de perfil e usuários');
");

            migrationBuilder.CreateTable(
                name: "TarefaEstados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
DELETE FROM [Modulos] WHERE [Id] = 10 AND [Codigo] = N'RPERFUSR';

IF EXISTS (SELECT 1 FROM [Modulos] WHERE [Id] = 6 AND [Codigo] = N'PERF')
    UPDATE [Modulos] SET [Codigo] = N'PAC', [Descricao] = N'Políticas e Acessos' WHERE [Id] = 6 AND [Codigo] = N'PERF';
ELSE IF EXISTS (SELECT 1 FROM [Modulos] WHERE [Id] = 6 AND [Codigo] = N'PAC')
    UPDATE [Modulos] SET [Descricao] = N'Políticas e Acessos' WHERE [Id] = 6 AND [Codigo] = N'PAC';
ELSE
    INSERT INTO [Modulos] ([Codigo], [Id], [Descricao]) VALUES (N'PAC', 6, N'Políticas e Acessos');
");
        }
    }
}

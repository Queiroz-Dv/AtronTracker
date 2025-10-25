using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atron.Infrastructure.Migrations
{
    public partial class Incluindo_CargaDeDados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {                
            migrationBuilder.InsertData(
                table: "Modulos",
                columns: new[] { "Codigo", "Id", "Descricao" },
                values: new object[,]
                {
                    { "DPT", 1, "Departamentos" },
                    { "CRG", 2, "Cargos" },
                    { "USR", 3, "Usuários" },
                    { "TAR", 4, "Tarefas" },
                    { "SAL", 5, "Salários" },
                    { "PAC", 6, "Políticas e Acessos" }
                });

            migrationBuilder.InsertData(
                table: "PropriedadesDeFluxo",
                columns: new[] { "Codigo", "Id" },
                values: new object[,]
                {
                    { "GRAVAR", 1 },
                    { "CONSULTAR", 2 },
                    { "DELETAR", 3 },
                    { "ATUALIZAR", 4 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {                               
        }
    }
}

using Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Application.Policies.Tarefas
{
    public interface ITarefaObtencaoPolicy
    {
        Resultado AvaliarAssuncao(Tarefa tarefa, bool possuiResponsabilidadeGestao);

        Resultado AvaliarSolicitacao(Tarefa tarefa, bool possuiResponsabilidadeGestao);
    }
}

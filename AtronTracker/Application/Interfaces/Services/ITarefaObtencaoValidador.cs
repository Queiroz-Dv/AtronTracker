using Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Application.Interfaces.Services
{
    public interface ITarefaObtencaoValidador
    {
        Resultado ValidarAssuncao(Tarefa tarefa, bool possuiResponsabilidadeGestao);

        Resultado ValidarSolicitacao(Tarefa tarefa, bool possuiResponsabilidadeGestao);
    }
}

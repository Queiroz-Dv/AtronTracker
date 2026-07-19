using Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Application.Interfaces.Services
{
    public interface ITarefaObtencaoValidador
    {
        Resultado ValidarAssuncao(Usuario usuario, Tarefa tarefa);

        Resultado ValidarSolicitacao(Usuario usuario, Tarefa tarefa);
    }
}

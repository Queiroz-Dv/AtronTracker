using Application.DTO;
using Shared.Domain.ValueObjects;

namespace Application.Interfaces.Services
{
    public interface ITarefaNotificacaoService
    {
        Task<Resultado> NotificarAtribuicaoAsync(TarefaDTO tarefa, UsuarioDTO usuario);
    }
}
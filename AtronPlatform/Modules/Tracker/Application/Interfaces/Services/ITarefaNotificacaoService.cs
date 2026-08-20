using Application.DTO;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ITarefaNotificacaoService
    {
        Task<Resultado> NotificarAtribuicaoAsync(TarefaDTO tarefa, UsuarioDTO usuario);
    }
}

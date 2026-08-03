using Application.DTO;
using Application.DTO.Request;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ITarefaConfiguracoesService
    {
        Task<Resultado<TarefaConfiguracoesDTO>> ObterAsync();

        Task<Resultado<TarefaConfiguracoesDTO>> AtualizarAsync(TarefaConfiguracoesRequest request);
    }
}

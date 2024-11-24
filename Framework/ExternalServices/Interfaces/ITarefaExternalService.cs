using Atron.Application.DTO;

namespace ExternalServices.Interfaces
{
    public interface ITarefaExternalService
    {
        Task<List<TarefaDTO>> ObterTodos();
    }
}
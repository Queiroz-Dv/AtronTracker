using Atron.Application.DTO;

namespace ExternalServices.Interfaces
{
    public interface ITarefaExternalService
    {
        Task Criar(TarefaDTO tarefaDTO);
        Task<List<TarefaDTO>> ObterTodos();
    }
}
using Atron.Application.DTO;

namespace ExternalServices.Interfaces
{
    public interface ITarefaExternalService
    {
        Task Criar(TarefaDTO tarefaDTO);

        Task<List<TarefaDTO>> ObterTodos();

        Task<TarefaDTO> ObterPorId();
        Task Atualizar(string id, TarefaDTO tarefaDTO);
        Task Remover(string id);
    }
}
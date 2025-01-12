using Atron.Domain.Entities;

namespace ExternalServices.Interfaces
{
    public interface ITarefaEstadoExternalService
    {
        Task<List<TarefaEstado>> ObterTodosAsync();
    }
}

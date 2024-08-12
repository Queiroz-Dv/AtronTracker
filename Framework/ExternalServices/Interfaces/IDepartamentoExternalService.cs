using Atron.Application.DTO;
using ExternalServices.Interfaces.ExternalMessage;

namespace ExternalServices.Interfaces
{
    public interface IDepartamentoExternalService : IExternalMessageService
    {
        Task<List<DepartamentoDTO>> ObterTodos();
        Task Criar(DepartamentoDTO departamento);
        Task Atualizar(string codigo, DepartamentoDTO departamentoDTO);
        Task Remover(string codigo);
    }
}
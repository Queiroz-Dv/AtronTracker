using Atron.Application.DTO;
using ExternalServices.Interfaces.ExternalMessage;
using Shared.DTO;

namespace ExternalServices.Interfaces
{
    public interface IDepartamentoExternalService : IExternalMessageService
    {
        Task<List<DepartamentoDTO>> ObterTodos();
        Task Criar(DepartamentoDTO departamento);
        Task<(bool isSucess, List<ResultResponse> responses)> Atualizar(string codigo, DepartamentoDTO departamentoDTO);
        Task<(bool isSuccess, List<ResultResponse> responses)> Remover(string codigo);
    }
}
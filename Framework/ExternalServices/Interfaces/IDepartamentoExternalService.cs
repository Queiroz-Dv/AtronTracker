using Atron.Application.DTO;
using Shared.DTO;

namespace ExternalServices.Interfaces
{
    public interface IDepartamentoExternalService
    {
        Task<List<DepartamentoDTO>> ObterTodos();
        Task<(bool isSucess, List<ResultResponse> responses)> Criar(DepartamentoDTO departamento);
        Task<(bool isSucess, List<ResultResponse> responses)> Atualizar(string codigo, DepartamentoDTO departamentoDTO);
        Task<(bool isSuccess, List<ResultResponse> responses)> Remover(string codigo);
    }
}
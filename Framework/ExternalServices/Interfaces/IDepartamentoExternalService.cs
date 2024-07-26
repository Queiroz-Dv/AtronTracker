using Atron.Application.DTO;
using Shared.DTO;

namespace ExternalServices.Interfaces
{
    public interface IDepartamentoExternalService
    {
        Task<List<DepartamentoDTO>> ObterDepartamentos();
        Task<(bool isSucess, List<ResultResponse> responses)> CriarDepartamento(DepartamentoDTO departamento);
        Task<(bool isSucess, List<ResultResponse> responses)> Atualizar(string codigo, DepartamentoDTO departamentoDTO);
    }
}
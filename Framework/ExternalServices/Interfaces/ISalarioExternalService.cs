using Atron.Application.DTO;

namespace ExternalServices.Interfaces
{
    public interface ISalarioExternalService
    {
        Task Criar(SalarioDTO salarioDTO);

        Task<List<SalarioDTO>> ObterTodos();

        Task<List<MesDTO>> ObterMeses();

        Task Atualizar(string id, SalarioDTO salarioDTO);

        Task Remover(string id);
        Task<SalarioDTO> ObterPorId();
    }
}

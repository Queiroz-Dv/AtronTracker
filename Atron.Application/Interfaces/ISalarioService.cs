using Atron.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface ISalarioService
    {
        Task CriarAsync(SalarioDTO salarioDTO);

        Task<List<SalarioDTO>> ObterTodosAsync();

        Task<List<MesDTO>> ObterMeses();

        Task AtualizarServiceAsync(SalarioDTO salarioDTO);

        Task ExcluirServiceAsync(int id);
        Task ExcluirAsync(string id);
        Task<SalarioDTO> ObterPorId(int id);
    }
}
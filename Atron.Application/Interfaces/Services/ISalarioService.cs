using Atron.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces.Services
{
    public interface ISalarioService
    {
        Task CriarAsync(SalarioDTO salarioDTO);

        Task<List<SalarioDTO>> ObterTodosAsync();

        Task AtualizarServiceAsync(int id, SalarioDTO salarioDTO);
        
        Task ExcluirAsync(string id);
        Task<SalarioDTO> ObterPorId(int id);
    }
}
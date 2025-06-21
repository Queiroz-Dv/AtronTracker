using Atron.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface IModuloService
    {
        Task<IEnumerable<ModuloDTO>> ObterTodosService();
        List<string> ObterTodosOsCodigos();
        Task<ModuloDTO> ObterPorIdService(int id);        
        Task<ModuloDTO> ObterPorCodigoService(string codigo);
    }
}

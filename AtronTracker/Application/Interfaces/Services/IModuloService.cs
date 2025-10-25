using Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IModuloService
    {
        Task<IEnumerable<ModuloDTO>> ObterTodosService();
        List<string> ObterTodosOsCodigos();
        Task<ModuloDTO> ObterPorIdService(int id);
        Task<ModuloDTO> ObterPorCodigoService(string codigo);
    }
}

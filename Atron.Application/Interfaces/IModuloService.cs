using Atron.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface IModuloService
    {
        Task<IEnumerable<ModuloDTO>> ObterTodosService();
        Task<ModuloDTO> ObterPorIdService(int id);
        Task CriarModuloServiceAsync(ModuloDTO moduloDTO);
        Task<ModuloDTO> AtualizarModuloServiceAsync(string codigo, ModuloDTO moduloDTO);
        Task<bool> RemoverModuloServiceAsync(string codigo);
        Task<ModuloDTO> ObterPorCodigoService(string codigo);
    }
}

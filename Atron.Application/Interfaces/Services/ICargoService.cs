using Atron.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces.Services
{
    public interface ICargoService
    {
        Task<List<CargoDTO>> ObterTodosAsync();

        Task<CargoDTO> ObterPorCodigoAsync(string codigo);

        Task CriarAsync(CargoDTO cargoDTO);

        Task AtualizarAsync(string codigo, CargoDTO cargoDTO);

        Task RemoverAsync(string codigo);
    }
}
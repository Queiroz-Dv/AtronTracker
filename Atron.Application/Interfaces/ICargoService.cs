using Atron.Application.DTO;
using Shared.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface ICargoService : IMessageModelService
    {
        Task<List<CargoDTO>> ObterTodosAsync();

        Task<CargoDTO> ObterPorCodigoAsync(string codigo);

        Task CriarAsync(CargoDTO cargoDTO);

        Task AtualizarAsync(string codigo, CargoDTO cargoDTO);

        Task RemoverAsync(string codigo);
    }
}
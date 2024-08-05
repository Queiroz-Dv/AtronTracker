using Atron.Application.DTO;
using Notification.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface ICargoService
    {
        public List<NotificationMessage> notificationMessages { get; }

        Task<List<CargoDTO>> ObterTodosAsync();

        Task<CargoDTO> ObterPorCodigoAsync(string codigo);

        Task CriarAsync(CargoDTO cargoDTO);

        Task AtualizarAsync(CargoDTO cargoDTO);

        Task RemoverAsync(int? id);
    }
}
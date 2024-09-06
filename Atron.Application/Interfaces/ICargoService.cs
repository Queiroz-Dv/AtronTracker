using Atron.Application.DTO;
using Notification.Models;
using Shared.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface ICargoService : IMessageModelService
    {
        public List<NotificationMessage> notificationMessages { get; }

        Task<List<CargoDTO>> ObterTodosAsync();

        Task<CargoDTO> ObterPorCodigoAsync(string codigo);

        Task CriarAsync(CargoDTO cargoDTO);

        Task AtualizarAsync(CargoDTO cargoDTO);

        Task RemoverAsync(int? id);
    }
}
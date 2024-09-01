using Atron.Application.DTO;
using Notification.Interfaces.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface ISalarioService : INotificationMessage
    {
        Task CriarAsync(SalarioDTO salarioDTO);

        Task<List<SalarioDTO>> ObterTodosAsync();

        Task AtualizarServiceAsync(SalarioDTO salarioDTO);

        Task ExcluirServiceAsync(int id);
    }
}
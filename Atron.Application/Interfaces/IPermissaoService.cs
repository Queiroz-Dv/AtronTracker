using Atron.Application.DTO;
using Notification.Interfaces.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface IPermissaoService : INotificationDTO
    {
        Task CriarPermissaoServiceAsync(PermissaoDTO permissaoDTO);

        Task<List<PermissaoDTO>> ObterTodasPermissoesServiceAsync();
    }
}
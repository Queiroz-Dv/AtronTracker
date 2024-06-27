using Atron.Application.DTO;
using Notification.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface IUsuarioService
    {
        public List<NotificationMessage> notificationMessages { get; }

        Task<List<UsuarioDTO>> ObterTodosAsync();

        Task<UsuarioDTO> ObterPorCodigoAsync(string codigo);

        Task CriarAsync(UsuarioDTO usuarioDTO);

        Task AtualizarAsync(UsuarioDTO usuarioDTO);

        Task RemoverAsync(int? id);
    }
}
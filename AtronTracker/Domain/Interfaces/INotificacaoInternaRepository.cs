using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface INotificacaoInternaRepository : IRepository<NotificacaoInterna>
    {
        Task<IEnumerable<NotificacaoInterna>> ObterPorUsuarioAsync(int usuarioId, string usuarioCodigo);

        Task<NotificacaoInterna> ObterPorIdEUsuarioAsync(int id, int usuarioId, string usuarioCodigo);

        Task<bool> CriarAsync(NotificacaoInterna notificacao);

        Task<bool> MarcarComoLidaAsync(int id, int usuarioId, string usuarioCodigo);

        Task<bool> MarcarTodasComoLidasAsync(int usuarioId, string usuarioCodigo);
    }
}

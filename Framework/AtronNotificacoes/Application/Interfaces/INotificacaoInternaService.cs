using AtronNotificacoes.Contracts.DTO.Request;
using AtronNotificacoes.Contracts.DTO.Response;

namespace AtronNotificacoes.Application.Interfaces;

public interface INotificacaoInternaService
{
    Task<NotificacaoInternaResponse> CriarAsync(
        PublicarNotificacaoInternaRequest request);

    Task<IReadOnlyList<NotificacaoInternaResponse>> ObterMinhasAsync(
        string destinatarioCodigo);

    Task<NotificacaoInternaResponse?> MarcarComoLidaAsync(
        long id,
        string destinatarioCodigo);

    Task<IReadOnlyList<NotificacaoInternaResponse>> MarcarTodasComoLidasAsync(
        string destinatarioCodigo);

    Task<bool> ExcluirAsync(long id, string destinatarioCodigo);
}
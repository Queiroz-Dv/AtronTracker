using AtronNotificacoes.Contracts.DTO.Request;
using AtronNotificacoes.Contracts.DTO.Response;

namespace AtronNotificacoes.Contracts.Interfaces;

public interface INotificacoesInternasPublisher
{
    Task<ResultadoPublicacaoNotificacaoInterna> PublicarAsync(
        PublicarNotificacaoInternaRequest request,
        CancellationToken cancellationToken = default);
}

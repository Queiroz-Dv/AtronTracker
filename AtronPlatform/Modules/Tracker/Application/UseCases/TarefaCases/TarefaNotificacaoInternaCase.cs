using AtronNotificacoes.Contracts.DTO;
using AtronNotificacoes.Contracts.DTO.Request;
using AtronNotificacoes.Contracts.Interfaces;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases
{
    public sealed class TarefaNotificacaoInternaCase(INotificacoesInternasPublisher publisher)
    {
        private readonly INotificacoesInternasPublisher _publisher = publisher;

        public async Task ExecutarAsync(PublicarNotificacaoInternaDto? notificacao)
        {
            if (notificacao is null ||
                string.IsNullOrWhiteSpace(notificacao.DestinatarioCodigo))
                return;

            var request = new PublicarNotificacaoInternaRequest
            {
                DestinatarioCodigo = notificacao.DestinatarioCodigo,
                ModuloOrigem = notificacao.ModuloOrigem.ToString(),
                TipoEvento = notificacao.TipoEvento,
                Titulo = notificacao.Titulo,
                Mensagem = notificacao.Mensagem,
                UrlDestino = notificacao.UrlDestino,
                ReferenciaExterna = notificacao.ReferenciaExterna,
                DataCriacao = notificacao.DataCriacao,
                ChaveIdempotencia = notificacao.ChaveIdempotencia,
                CorrelacaoId = notificacao.CorrelacaoId
            };

            await _publisher.PublicarAsync(request);
        }
    }
}

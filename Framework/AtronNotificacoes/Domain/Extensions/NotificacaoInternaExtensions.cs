using AtronNotificacoes.Contracts.DTO.Response;

namespace AtronNotificacoes.Domain.Extensions
{
    public static class NotificacaoInternaExtensions
    {
        public static NotificacaoInternaResponse MapToResponse(this NotificacaoInterna notificacao)
        {
            return new NotificacaoInternaResponse(
            notificacao.Id,
            notificacao.ModuloOrigem,
            notificacao.TipoEvento,
            notificacao.Titulo,
            notificacao.Mensagem,
            notificacao.UrlDestino,
            notificacao.ReferenciaExterna,
            notificacao.Lida,
            notificacao.DataCriacao,
            notificacao.DataLeitura);
        }
    }
}
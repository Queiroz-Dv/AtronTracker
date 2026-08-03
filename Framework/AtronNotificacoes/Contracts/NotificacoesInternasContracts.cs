namespace AtronNotificacoes.Contracts;

public sealed record PublicarNotificacaoInternaRequest(
    string DestinatarioCodigo,
    string ModuloOrigem,
    string TipoEvento,
    string Titulo,
    string Mensagem,
    string? UrlDestino,
    string? ReferenciaExterna,
    DateTimeOffset DataCriacao,
    string? ChaveIdempotencia,
    string? CorrelacaoId = null);

public interface INotificacoesInternasPublisher
{
    Task<ResultadoPublicacaoNotificacaoInterna> PublicarAsync(
        PublicarNotificacaoInternaRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record ResultadoPublicacaoNotificacaoInterna(
    bool Publicada,
    NotificacaoInternaResponse? Notificacao,
    string? MotivoDaFalha)
{
    public static ResultadoPublicacaoNotificacaoInterna Sucesso(NotificacaoInternaResponse notificacao) =>
        new(true, notificacao, null);

    public static ResultadoPublicacaoNotificacaoInterna Falha(string motivoDaFalha) =>
        new(false, null, motivoDaFalha);
}

public sealed record NotificacaoInternaResponse(
    long Id,
    string ModuloOrigem,
    string TipoEvento,
    string Titulo,
    string Mensagem,
    string? UrlDestino,
    string? ReferenciaExterna,
    bool Lida,
    DateTimeOffset DataCriacao,
    DateTimeOffset? DataLeitura);

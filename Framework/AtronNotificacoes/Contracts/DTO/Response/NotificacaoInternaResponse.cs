namespace AtronNotificacoes.Contracts.DTO.Response;

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

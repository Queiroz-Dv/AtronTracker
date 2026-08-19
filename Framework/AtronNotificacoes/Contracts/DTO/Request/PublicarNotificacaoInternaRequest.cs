namespace AtronNotificacoes.Contracts.DTO.Request;

public sealed record PublicarNotificacaoInternaRequest
{
    public required string DestinatarioCodigo { get; init; }

    public required string ModuloOrigem { get; init; }

    public required string TipoEvento { get; init; }

    public required string Titulo { get; init; }

    public required string Mensagem { get; init; }

    public string? UrlDestino { get; init; }

    public string? ReferenciaExterna { get; init; }

    public required DateTimeOffset DataCriacao { get; init; }

    public string? ChaveIdempotencia { get; init; }

    public string? CorrelacaoId { get; init; }
}

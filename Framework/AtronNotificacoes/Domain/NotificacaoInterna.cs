namespace AtronNotificacoes.Domain;

public class NotificacaoInterna
{
    public long Id { get; set; }
    public string DestinatarioCodigo { get; set; } = string.Empty;
    public string ModuloOrigem { get; set; } = string.Empty;
    public string TipoEvento { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string? UrlDestino { get; set; }
    public string? ReferenciaExterna { get; set; }
    public string? ChaveIdempotencia { get; set; }
    public string? CorrelacaoId { get; set; }
    public bool Lida { get; private set; }
    public DateTimeOffset DataCriacao { get; set; }
    public DateTimeOffset? DataLeitura { get; private set; }
    public DateTimeOffset? DataExclusao { get; private set; }

    public void MarcarComoLida(DateTimeOffset dataLeitura)
    {
        if (Lida)
            return;

        Lida = true;
        DataLeitura = dataLeitura;
    }

    public void Excluir(DateTimeOffset dataExclusao)
    {
        if (DataExclusao is not null)
            return;

        DataExclusao = dataExclusao;
    }
}

using System.Diagnostics.Metrics;
using AtronNotificacoes.Contracts;

namespace AtronNotificacoes;

public static class ObservabilidadeNotificacoes
{
    public const string NomeDoMedidor = "Atron.Notificacoes";
    public const string NomeContadorPublicacoes = "atron_notificacoes_publicacoes_total";
    public const string NomeContadorFalhasDePublicacao = "atron_notificacoes_falhas_publicacao_total";

    private static readonly Meter Medidor = new(NomeDoMedidor, "1.0.0");
    private static readonly Counter<long> Publicacoes = Medidor.CreateCounter<long>(NomeContadorPublicacoes);
    private static readonly Counter<long> FalhasDePublicacao = Medidor.CreateCounter<long>(NomeContadorFalhasDePublicacao);

    public static void RegistrarPublicacao(PublicarNotificacaoInternaRequest request)
    {
        Publicacoes.Add(1,
            new KeyValuePair<string, object?>("modulo_origem", request.ModuloOrigem),
            new KeyValuePair<string, object?>("tipo_evento", request.TipoEvento));
    }

    public static void RegistrarFalhaDePublicacao(PublicarNotificacaoInternaRequest request)
    {
        FalhasDePublicacao.Add(1,
            new KeyValuePair<string, object?>("modulo_origem", request.ModuloOrigem),
            new KeyValuePair<string, object?>("tipo_evento", request.TipoEvento));
    }
}

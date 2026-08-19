namespace AtronNotificacoes.Contracts.DTO.Response;

public sealed record ResultadoPublicacaoNotificacaoInterna(
    bool Publicada,
    NotificacaoInternaResponse? Notificacao,
    string? MotivoDaFalha)
{
    public static ResultadoPublicacaoNotificacaoInterna Sucesso(
        NotificacaoInternaResponse notificacao) =>
        new(true, notificacao, null);

    public static ResultadoPublicacaoNotificacaoInterna Falha(
        string motivoDaFalha) =>
        new(false, null, motivoDaFalha);
}

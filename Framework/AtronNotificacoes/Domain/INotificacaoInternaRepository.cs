namespace AtronNotificacoes.Domain;

public interface INotificacaoInternaRepository
{
    void Adicionar(NotificacaoInterna notificacao);
    Task<NotificacaoInterna?> ObterPorChaveIdempotenciaAsync(string moduloOrigem, string chaveIdempotencia);
    Task<IReadOnlyList<NotificacaoInterna>> ObterPorDestinatarioAsync(string destinatarioCodigo);
    Task<NotificacaoInterna?> ObterPorIdEDestinatarioAsync(long id, string destinatarioCodigo);
    Task<bool> ExcluirAsync(long id, string destinatarioCodigo, DateTimeOffset dataExclusao);
    Task<bool> MarcarTodasComoLidasAsync(string destinatarioCodigo, DateTimeOffset dataLeitura);
    Task SalvarAlteracoesAsync();
}

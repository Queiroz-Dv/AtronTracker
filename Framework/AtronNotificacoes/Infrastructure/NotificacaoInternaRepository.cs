using AtronNotificacoes.Domain;
using Microsoft.EntityFrameworkCore;

namespace AtronNotificacoes.Infrastructure;

public sealed class NotificacaoInternaRepository(NotificacoesDbContext context) : INotificacaoInternaRepository
{
    public void Adicionar(NotificacaoInterna notificacao) => context.NotificacoesInternas.Add(notificacao);

    public Task<NotificacaoInterna?> ObterPorChaveIdempotenciaAsync(string moduloOrigem, string chaveIdempotencia) =>
        context.NotificacoesInternas.FirstOrDefaultAsync(notificacao =>
            notificacao.ModuloOrigem == moduloOrigem && notificacao.ChaveIdempotencia == chaveIdempotencia);

    public async Task<IReadOnlyList<NotificacaoInterna>> ObterPorDestinatarioAsync(string destinatarioCodigo) =>
        await context.NotificacoesInternas
            .Where(notificacao => notificacao.DestinatarioCodigo == destinatarioCodigo && notificacao.DataExclusao == null)
            .OrderBy(notificacao => notificacao.Lida)
            .ThenByDescending(notificacao => notificacao.DataCriacao)
            .ToListAsync();

    public Task<NotificacaoInterna?> ObterPorIdEDestinatarioAsync(long id, string destinatarioCodigo) =>
        context.NotificacoesInternas.FirstOrDefaultAsync(notificacao =>
            notificacao.Id == id && notificacao.DestinatarioCodigo == destinatarioCodigo && notificacao.DataExclusao == null);

    public async Task<bool> ExcluirAsync(long id, string destinatarioCodigo, DateTimeOffset dataExclusao)
    {
        var notificacao = await ObterPorIdEDestinatarioAsync(id, destinatarioCodigo);
        if (notificacao is null)
            return false;

        notificacao.Excluir(dataExclusao);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarcarTodasComoLidasAsync(string destinatarioCodigo, DateTimeOffset dataLeitura)
    {
        var notificacoes = await context.NotificacoesInternas
            .Where(notificacao => notificacao.DestinatarioCodigo == destinatarioCodigo && notificacao.DataExclusao == null && !notificacao.Lida)
            .ToListAsync();

        foreach (var notificacao in notificacoes)
            notificacao.MarcarComoLida(dataLeitura);

        if (notificacoes.Count == 0)
            return false;

        await context.SaveChangesAsync();
        return true;
    }

    public Task SalvarAlteracoesAsync() => context.SaveChangesAsync();
}

using AtronNotificacoes.Contracts;
using AtronNotificacoes.Domain;

namespace AtronNotificacoes.Application;

public interface INotificacaoInternaService
{
    Task<NotificacaoInternaResponse> CriarAsync(PublicarNotificacaoInternaRequest request);
    Task<IReadOnlyList<NotificacaoInternaResponse>> ObterMinhasAsync(string destinatarioCodigo);
    Task<NotificacaoInternaResponse?> MarcarComoLidaAsync(long id, string destinatarioCodigo);
    Task<IReadOnlyList<NotificacaoInternaResponse>> MarcarTodasComoLidasAsync(string destinatarioCodigo);
    Task<bool> ExcluirAsync(long id, string destinatarioCodigo);
}

public sealed class NotificacaoInternaService(INotificacaoInternaRepository repository) : INotificacaoInternaService
{
    public async Task<NotificacaoInternaResponse> CriarAsync(PublicarNotificacaoInternaRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.ChaveIdempotencia))
        {
            var notificacaoExistente = await repository.ObterPorChaveIdempotenciaAsync(
                request.ModuloOrigem,
                request.ChaveIdempotencia);

            if (notificacaoExistente is not null)
                return Mapear(notificacaoExistente);
        }

        var notificacao = new NotificacaoInterna
        {
            DestinatarioCodigo = request.DestinatarioCodigo,
            ModuloOrigem = request.ModuloOrigem,
            TipoEvento = request.TipoEvento,
            Titulo = request.Titulo,
            Mensagem = request.Mensagem,
            UrlDestino = request.UrlDestino,
            ReferenciaExterna = request.ReferenciaExterna,
            DataCriacao = request.DataCriacao,
            ChaveIdempotencia = request.ChaveIdempotencia,
            CorrelacaoId = request.CorrelacaoId
        };

        repository.Adicionar(notificacao);
        await repository.SalvarAlteracoesAsync();
        return Mapear(notificacao);
    }

    public async Task<IReadOnlyList<NotificacaoInternaResponse>> ObterMinhasAsync(string destinatarioCodigo)
    {
        var notificacoes = await repository.ObterPorDestinatarioAsync(destinatarioCodigo);
        return notificacoes.Select(Mapear).ToList();
    }

    public async Task<NotificacaoInternaResponse?> MarcarComoLidaAsync(long id, string destinatarioCodigo)
    {
        var notificacao = await repository.ObterPorIdEDestinatarioAsync(id, destinatarioCodigo);
        if (notificacao is null)
            return null;

        notificacao.MarcarComoLida(DateTimeOffset.UtcNow);
        await repository.SalvarAlteracoesAsync();
        return Mapear(notificacao);
    }

    public async Task<IReadOnlyList<NotificacaoInternaResponse>> MarcarTodasComoLidasAsync(string destinatarioCodigo)
    {
        await repository.MarcarTodasComoLidasAsync(destinatarioCodigo, DateTimeOffset.UtcNow);
        return await ObterMinhasAsync(destinatarioCodigo);
    }

    public Task<bool> ExcluirAsync(long id, string destinatarioCodigo) =>
        repository.ExcluirAsync(id, destinatarioCodigo, DateTimeOffset.UtcNow);

    private static NotificacaoInternaResponse Mapear(NotificacaoInterna notificacao) =>
        new(
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

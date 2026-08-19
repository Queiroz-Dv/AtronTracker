using AtronNotificacoes.Application.Interfaces;
using AtronNotificacoes.Contracts.DTO.Request;
using AtronNotificacoes.Contracts.DTO.Response;
using AtronNotificacoes.Domain;
using AtronNotificacoes.Domain.Extensions;

namespace AtronNotificacoes.Application.Services;

public sealed class NotificacaoInternaService(
    INotificacaoInternaRepository repository) : INotificacaoInternaService
{
    public async Task<NotificacaoInternaResponse> CriarAsync(
        PublicarNotificacaoInternaRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.ChaveIdempotencia))
        {
            var notificacaoExistente = await repository.ObterPorChaveIdempotenciaAsync(
                request.ModuloOrigem,
                request.ChaveIdempotencia);

            if (notificacaoExistente is not null)
                return notificacaoExistente.MapToResponse();
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
        return notificacao.MapToResponse();
    }

    public async Task<IReadOnlyList<NotificacaoInternaResponse>> ObterMinhasAsync(
        string destinatarioCodigo)
    {
        var notificacoes = await repository.ObterPorDestinatarioAsync(
            destinatarioCodigo);

        return notificacoes.Select(r => r.MapToResponse()).ToList();
    }

    public async Task<NotificacaoInternaResponse?> MarcarComoLidaAsync(
        long id,
        string destinatarioCodigo)
    {
        var notificacao = await repository.ObterPorIdEDestinatarioAsync(
            id,
            destinatarioCodigo);

        if (notificacao is null)
            return null;

        notificacao.MarcarComoLida(DateTimeOffset.UtcNow);
        await repository.SalvarAlteracoesAsync();
        return notificacao.MapToResponse();
    }

    public async Task<IReadOnlyList<NotificacaoInternaResponse>> MarcarTodasComoLidasAsync(
        string destinatarioCodigo)
    {
        await repository.MarcarTodasComoLidasAsync(
            destinatarioCodigo,
            DateTimeOffset.UtcNow);

        return await ObterMinhasAsync(destinatarioCodigo);
    }

    public Task<bool> ExcluirAsync(long id, string destinatarioCodigo) =>
        repository.ExcluirAsync(id, destinatarioCodigo, DateTimeOffset.UtcNow);
   
}
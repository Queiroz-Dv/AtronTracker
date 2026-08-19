using AtronNotificacoes.Application.Services;
using AtronNotificacoes.Contracts.DTO.Request;
using AtronNotificacoes.Domain;
using AtronNotificacoes.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Notificacoes.Tests.Integracao;

public sealed class NotificacaoInternaIntegrationTests
{
    [Fact]
    public async Task CriarAsync_persiste_o_conteudo_final_sem_dependencia_do_modulo_produtor()
    {
        await using var context = CriarContexto("criar");
        var service = new NotificacaoInternaService(new NotificacaoInternaRepository(context));

        var resultado = await service.CriarAsync(
            new PublicarNotificacaoInternaRequest
            {
                DestinatarioCodigo = "USR001",
                ModuloOrigem = "Sales",
                TipoEvento = "PropostaAprovada",
                Titulo = "Proposta aprovada",
                Mensagem = "A proposta foi aprovada.",
                UrlDestino = "/propostas/42",
                ReferenciaExterna = "proposta:42",
                DataCriacao = DateTimeOffset.Parse("2026-07-19T12:00:00Z"),
                ChaveIdempotencia = "evento:42"
            });

        var persistida = await context.NotificacoesInternas.SingleAsync();

        Assert.Equal(persistida.Id, resultado.Id);
        Assert.Equal("USR001", persistida.DestinatarioCodigo);
        Assert.Equal("Sales", persistida.ModuloOrigem);
        Assert.Equal("Proposta aprovada", persistida.Titulo);
        Assert.Equal("proposta:42", persistida.ReferenciaExterna);
        Assert.False(persistida.Lida);
    }

    [Fact]
    public async Task CriarAsync_reutiliza_a_notificacao_quando_a_chave_idempotente_ja_foi_publicada()
    {
        await using var context = CriarContexto("idempotencia");
        var service = new NotificacaoInternaService(new NotificacaoInternaRepository(context));
        var request = new PublicarNotificacaoInternaRequest
        {
            DestinatarioCodigo = "USR001",
            ModuloOrigem = "Tracker",
            TipoEvento = "TarefaCriada",
            Titulo = "Nova tarefa",
            Mensagem = "Uma tarefa foi criada.",
            UrlDestino = null,
            ReferenciaExterna = "tarefa:123",
            DataCriacao = DateTimeOffset.Parse("2026-07-19T12:00:00Z"),
            ChaveIdempotencia = "tracker:tarefa:123",
            CorrelacaoId = "correlacao-123"
        };

        var primeira = await service.CriarAsync(request);
        var segunda = await service.CriarAsync(request);

        Assert.Equal(primeira.Id, segunda.Id);
        Assert.Single(await context.NotificacoesInternas.ToListAsync());
    }

    [Fact]
    public async Task ObterMinhasAsync_retorna_somente_notificacoes_do_destinatario_em_ordem_de_nao_lida()
    {
        await using var context = CriarContexto("consulta");
        context.NotificacoesInternas.AddRange(
            CriarNotificacao(1000001, "USR001", "Tracker", "TarefaCriada", DateTimeOffset.Parse("2026-07-19T12:00:00Z")),
            CriarNotificacao(1000002, "USR001", "Tracker", "TarefaAtualizada", DateTimeOffset.Parse("2026-07-19T13:00:00Z")),
            CriarNotificacao(1000003, "USR002", "Stock", "EstoqueBaixo", DateTimeOffset.Parse("2026-07-19T14:00:00Z")));
        await context.SaveChangesAsync();

        var service = new NotificacaoInternaService(new NotificacaoInternaRepository(context));

        var resultado = await service.ObterMinhasAsync("USR001");

        Assert.Collection(
            resultado,
            notificacao =>
            {
                Assert.Equal(1000002, notificacao.Id);
                Assert.Equal("Tracker", notificacao.ModuloOrigem);
                Assert.Equal("TarefaAtualizada", notificacao.TipoEvento);
            },
            notificacao => Assert.Equal(1000001, notificacao.Id));
    }

    [Fact]
    public async Task MarcarComoLidaAsync_atualiza_apenas_a_notificacao_do_destinatario()
    {
        await using var context = CriarContexto("marcar-uma");
        context.NotificacoesInternas.AddRange(
            CriarNotificacao(1000001, "USR001", "Tracker", "TarefaCriada", DateTimeOffset.Parse("2026-07-19T12:00:00Z")),
            CriarNotificacao(1000002, "USR002", "Sales", "PropostaAprovada", DateTimeOffset.Parse("2026-07-19T13:00:00Z")));
        await context.SaveChangesAsync();

        var service = new NotificacaoInternaService(new NotificacaoInternaRepository(context));

        var notificacaoMarcada = await service.MarcarComoLidaAsync(1000001, "USR001");
        var notificacaoDeOutroDestinatario = await service.MarcarComoLidaAsync(1000002, "USR001");

        Assert.NotNull(notificacaoMarcada);
        Assert.True(notificacaoMarcada.Lida);
        Assert.NotNull(notificacaoMarcada.DataLeitura);
        Assert.Null(notificacaoDeOutroDestinatario);
        Assert.False((await context.NotificacoesInternas.SingleAsync(notificacao => notificacao.Id == 1000002)).Lida);
    }

    [Fact]
    public async Task MarcarTodasComoLidasAsync_atualiza_todas_as_notificacoes_pendentes_do_destinatario()
    {
        await using var context = CriarContexto("marcar-todas");
        context.NotificacoesInternas.AddRange(
            CriarNotificacao(1000001, "USR001", "Tracker", "TarefaCriada", DateTimeOffset.Parse("2026-07-19T12:00:00Z")),
            CriarNotificacao(1000002, "USR001", "Stock", "EstoqueBaixo", DateTimeOffset.Parse("2026-07-19T13:00:00Z")),
            CriarNotificacao(1000003, "USR002", "Sales", "PropostaAprovada", DateTimeOffset.Parse("2026-07-19T14:00:00Z")));
        await context.SaveChangesAsync();

        var service = new NotificacaoInternaService(new NotificacaoInternaRepository(context));

        var resultado = await service.MarcarTodasComoLidasAsync("USR001");

        Assert.All(resultado, notificacao =>
        {
            Assert.True(notificacao.Lida);
            Assert.NotNull(notificacao.DataLeitura);
        });
        Assert.False((await context.NotificacoesInternas.SingleAsync(notificacao => notificacao.Id == 1000003)).Lida);
    }

    [Fact]
    public async Task ExcluirAsync_oculta_apenas_a_notificacao_do_destinatario_sem_apagar_o_historico()
    {
        await using var context = CriarContexto("excluir");
        context.NotificacoesInternas.AddRange(
            CriarNotificacao(1000001, "USR001", "Tracker", "TarefaCriada", DateTimeOffset.Parse("2026-07-20T12:00:00Z")),
            CriarNotificacao(1000002, "USR002", "Stock", "EstoqueBaixo", DateTimeOffset.Parse("2026-07-20T13:00:00Z")));
        await context.SaveChangesAsync();

        var service = new NotificacaoInternaService(new NotificacaoInternaRepository(context));

        var excluida = await service.ExcluirAsync(1000001, "USR001");
        var exclusaoDeOutroDestinatario = await service.ExcluirAsync(1000002, "USR001");
        var visiveis = await service.ObterMinhasAsync("USR001");
        var persistida = await context.NotificacoesInternas.SingleAsync(notificacao => notificacao.Id == 1000001);

        Assert.True(excluida);
        Assert.False(exclusaoDeOutroDestinatario);
        Assert.Empty(visiveis);
        Assert.NotNull(persistida.DataExclusao);
        Assert.Null((await context.NotificacoesInternas.SingleAsync(notificacao => notificacao.Id == 1000002)).DataExclusao);
    }

    private static NotificacoesDbContext CriarContexto(string cenario)
    {
        var opcoes = new DbContextOptionsBuilder<NotificacoesDbContext>()
            .UseInMemoryDatabase($"notificacoes-integracao-{cenario}-{DateTime.UtcNow.Ticks}")
            .Options;

        return new NotificacoesDbContext(opcoes);
    }

    private static NotificacaoInterna CriarNotificacao(
        long id,
        string destinatarioCodigo,
        string moduloOrigem,
        string tipoEvento,
        DateTimeOffset dataCriacao) =>
        new()
        {
            Id = id,
            DestinatarioCodigo = destinatarioCodigo,
            ModuloOrigem = moduloOrigem,
            TipoEvento = tipoEvento,
            Titulo = "Título da notificação",
            Mensagem = "Mensagem da notificação",
            DataCriacao = dataCriacao
        };
}

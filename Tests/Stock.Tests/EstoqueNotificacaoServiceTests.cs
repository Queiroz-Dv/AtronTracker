using AtronNotificacoes.Contracts;
using AtronStock.Application.Services;
using AtronStock.Domain.Entities;
using Xunit;

namespace Stock.Tests;

public sealed class EstoqueNotificacaoServiceTests
{
    [Fact]
    public async Task NotificarSaidaRegistradaAsync_publica_evento_do_stock_para_o_responsavel_configurado()
    {
        var publisher = new PublisherCapturador();
        var service = new EstoqueNotificacaoService(
            publisher,
            new ResponsavelNotificacaoEstoqueResolver("USR_ESTOQUE"));

        await service.NotificarSaidaRegistradaAsync(
            new Venda { Id = 12 },
            new Produto { Id = 7, Codigo = "PRD-007", Descricao = "Caderno" },
            new ItemVenda { Id = 32, ProdutoId = 7, Quantidade = 3 },
            17);

        var request = Assert.Single(publisher.Requests);
        Assert.Equal("USR_ESTOQUE", request.DestinatarioCodigo);
        Assert.Equal("Stock", request.ModuloOrigem);
        Assert.Equal("SaidaEstoqueRegistrada", request.TipoEvento);
        Assert.Equal("produto:7", request.ReferenciaExterna);
        Assert.Equal("stock:venda:12:item:32", request.ChaveIdempotencia);
        Assert.Contains("Saldo atual: 17", request.Mensagem);
    }

    [Fact]
    public async Task NotificarSaidaRegistradaAsync_nao_publica_sem_responsavel_configurado()
    {
        var publisher = new PublisherCapturador();
        var service = new EstoqueNotificacaoService(
            publisher,
            new ResponsavelNotificacaoEstoqueResolver(string.Empty));

        await service.NotificarSaidaRegistradaAsync(
            new Venda { Id = 12 },
            new Produto { Id = 7, Codigo = "PRD-007", Descricao = "Caderno" },
            new ItemVenda { Id = 32, ProdutoId = 7, Quantidade = 3 },
            17);

        Assert.Empty(publisher.Requests);
    }

    private sealed class PublisherCapturador : INotificacoesInternasPublisher
    {
        public List<PublicarNotificacaoInternaRequest> Requests { get; } = [];

        public Task<ResultadoPublicacaoNotificacaoInterna> PublicarAsync(
            PublicarNotificacaoInternaRequest request,
            CancellationToken cancellationToken = default)
        {
            Requests.Add(request);
            return Task.FromResult(ResultadoPublicacaoNotificacaoInterna.Sucesso(
                new NotificacaoInternaResponse(1000001, "Stock", request.TipoEvento, request.Titulo, request.Mensagem,
                    request.UrlDestino, request.ReferenciaExterna, false, request.DataCriacao, null)));
        }
    }
}

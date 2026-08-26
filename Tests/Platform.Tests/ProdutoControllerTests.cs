using AtronPlatform.WebApi.Controllers.Stock;
using AtronStock.Application.DTO.Request;
using AtronStock.Application.DTO.Response;
using AtronStock.Application.Interfaces;
using AtronStock.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Platform.Tests;

public sealed class ProdutoControllerTests
{
    [Fact]
    public async Task Post_DeveResponderSomenteComNotificacoes()
    {
        var resultado = Resultado.Sucesso();
        resultado.MensagemRegistroSalvo("Produto");
        var controller = new ProdutoController(new ProdutoServiceFake(gravacao: resultado));

        var action = await controller.Post(new ProdutoRequest());

        var ok = Assert.IsType<OkObjectResult>(action);
        var mensagens = Assert.IsAssignableFrom<IEnumerable<NotificationMessage>>(ok.Value);
        Assert.Equal("Produto salvo com sucesso.", Assert.Single(mensagens).Descricao);
    }

    [Fact]
    public async Task Put_DeveResponderSomenteComNotificacoes()
    {
        var resultado = Resultado.Sucesso();
        resultado.MensagemRegistroAtualizado("MTG30");
        var controller = new ProdutoController(new ProdutoServiceFake(gravacao: resultado));

        var action = await controller.Put("MTG30", new ProdutoAtualizacaoRequest());

        var ok = Assert.IsType<OkObjectResult>(action);
        var mensagens = Assert.IsAssignableFrom<IEnumerable<NotificationMessage>>(ok.Value);
        Assert.Equal(
            "Registro MTG30 atualizado com sucesso.",
            Assert.Single(mensagens).Descricao);
    }

    [Fact]
    public async Task PostLote_DeveResponderAcceptedComIdentificador()
    {
        var response = new SolicitacaoGeracaoProdutosLoteResponse(
            37,
            EStatusProcessamentoProdutoLote.Pendente);
        var controller = new ProdutoController(new ProdutoServiceFake(response));

        var action = await controller.PostLote(new GerarProdutosLoteRequest());

        var accepted = Assert.IsType<AcceptedResult>(action.Result);
        Assert.Equal(StatusCodes.Status202Accepted, accepted.StatusCode);
        Assert.Same(response, accepted.Value);
    }

    private sealed class ProdutoServiceFake(
        SolicitacaoGeracaoProdutosLoteResponse? response = null,
        Resultado? gravacao = null) : IProdutoService
    {
        public Task<Resultado<SolicitacaoGeracaoProdutosLoteResponse>>
            SolicitarGeracaoLoteAsync(GerarProdutosLoteRequest request)
            => Task.FromResult(Resultado<SolicitacaoGeracaoProdutosLoteResponse>
                .Sucesso(response!));

        public Task<Resultado> CriarAsync(ProdutoRequest request)
            => Task.FromResult(gravacao ?? throw new NotSupportedException());

        public Task<Resultado> AtualizarAsync(
            string codigo,
            ProdutoAtualizacaoRequest request)
            => Task.FromResult(gravacao ?? throw new NotSupportedException());

        public Task<Resultado<ICollection<ProdutoResponse>>> ObterTodosAsync()
            => throw new NotSupportedException();

        public Task<Resultado<ProdutoResponse>> ObterPorCodigoAsync(string codigo)
            => throw new NotSupportedException();
    }
}

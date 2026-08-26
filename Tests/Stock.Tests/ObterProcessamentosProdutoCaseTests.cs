using AtronStock.Application.Mapping;
using AtronStock.Application.UseCases.ProdutoCases;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using AtronStock.Domain.ValueObjects;
using Stock.Tests.TestSupport.Fakes;
using Xunit;

namespace Stock.Tests;

public sealed class ObterProcessamentosProdutoCaseTests
{
    [Fact]
    public async Task ObterMeusAsync_DeveUsarSolicitanteLogadoEMapearProgresso()
    {
        var repository = new ProcessamentoProdutoLoteRepositoryFake
        {
            ProcessamentosDoSolicitante = [CriarProcessamento()]
        };
        var useCase = new ObterMeusProcessamentosProdutoCase(
            repository,
            new ProcessamentoProdutoMapping(),
            new UserAccessorFake("usr001"));

        var resultado = await useCase.ExecutarAsync();

        Assert.True(resultado.TeveSucesso);
        Assert.Equal("USR001", repository.UltimoSolicitanteConsultado);
        var item = Assert.Single(resultado.Dados!);
        Assert.Equal(3, item.QuantidadeSolicitada);
        Assert.Equal(3, item.QuantidadeProcessada);
        Assert.Equal("MTG_2026", item.LoteProdutoCodigo);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveConsultarIdNoEscopoDoSolicitante()
    {
        var repository = new ProcessamentoProdutoLoteRepositoryFake
        {
            ProcessamentosDoSolicitante = [CriarProcessamento()]
        };
        var useCase = new ObterProcessamentoProdutoCase(
            repository,
            new ProcessamentoProdutoMapping(),
            new UserAccessorFake("USR002"));

        var resultado = await useCase.ExecutarAsync(10);

        Assert.True(resultado.TeveSucesso);
        Assert.Equal("USR002", repository.UltimoSolicitanteConsultado);
        Assert.Equal(10, resultado.Dados!.Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveOcultarProcessamentoForaDoEscopo()
    {
        var repository = new ProcessamentoProdutoLoteRepositoryFake();
        var useCase = new ObterProcessamentoProdutoCase(
            repository,
            new ProcessamentoProdutoMapping(),
            new UserAccessorFake("USR002"));

        var resultado = await useCase.ExecutarAsync(99);

        Assert.True(resultado.TeveFalha);
        Assert.Null(resultado.Dados);
    }

    private static ProcessamentoProdutoLote CriarProcessamento()
    {
        var lote = new LoteProduto { Id = 42, Codigo = "MTG_2026" };
        var processamento = new ProcessamentoProdutoLote(
            new SolicitacaoGeracaoProdutosLote(
                "MTG",
                3,
                "USR001",
                "Monitor novo",
                null,
                new DateTime(2026, 8, 24),
                1500m,
                []))
        {
            Id = 10,
            LoteProduto = lote,
            LoteProdutoId = lote.Id
        };
        var token = Guid.NewGuid();
        processamento.Reservar(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(5), token);
        processamento.Concluir(lote.Id, 3, token);
        return processamento;
    }
}

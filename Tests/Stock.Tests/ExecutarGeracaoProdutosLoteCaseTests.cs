using AtronStock.Application.DTO.Request;
using AtronStock.Application.Mapping;
using AtronStock.Application.UseCases.ProdutoCases;
using AtronStock.Application.Validacoes;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using Stock.Tests.TestSupport.Fakes;
using Xunit;

namespace Stock.Tests;

public sealed class ExecutarGeracaoProdutosLoteCaseTests
{
    [Theory]
    [InlineData("152", "152_2026", "1521", "1522", "1523")]
    [InlineData("mtg", "MTG_2026", "MTG1", "MTG2", "MTG3")]
    public async Task ExecutarAsync_DeveGerarSequenciaNumericaOuAlfanumerica(
        string codigoBase,
        string codigoLote,
        params string[] codigosEsperados)
    {
        var repository = new LoteProdutoRepositoryFake();
        var useCase = CriarUseCase(repository);

        var resultado = await useCase.ExecutarAsync(CriarCommand(codigoBase, 3));

        Assert.True(resultado.TeveSucesso);
        var lote = Assert.IsType<LoteProduto>(repository.LoteAdicionado);
        Assert.Equal(codigoLote, lote.Codigo);
        Assert.Equal(codigosEsperados, lote.Produtos.Select(produto => produto.Codigo));
        Assert.All(lote.Produtos, produto =>
        {
            Assert.Equal(EStatusProduto.Ativo, produto.Status);
            Assert.Same(lote, produto.LoteProduto);
        });
    }

    [Fact]
    public async Task ExecutarAsync_DeveUsarProximoSufixoLivreNoCodigoDoLote()
    {
        var repository = new LoteProdutoRepositoryFake
        {
            CodigosLoteExistentes = ["MTG_2026", "MTG_2026_2"]
        };

        var resultado = await CriarUseCase(repository)
            .ExecutarAsync(CriarCommand("MTG", 2));

        Assert.True(resultado.TeveSucesso);
        Assert.Equal("MTG_2026_3", repository.LoteAdicionado!.Codigo);
    }

    [Fact]
    public async Task ExecutarAsync_DeveGerarQuantidadeAcimaDeDezMil()
    {
        var repository = new LoteProdutoRepositoryFake();

        var resultado = await CriarUseCase(repository)
            .ExecutarAsync(CriarCommand("MTG", 10_001));

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(10_001, repository.LoteAdicionado!.Produtos.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ExecutarAsync_DeveRecusarQuantidadeNaoPositiva(int quantidade)
    {
        var repository = new LoteProdutoRepositoryFake();

        var resultado = await CriarUseCase(repository)
            .ExecutarAsync(CriarCommand("MTG", quantidade));

        Assert.True(resultado.TeveFalha);
        Assert.Null(repository.LoteAdicionado);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRecusarCodigosDeProdutoJaExistentes()
    {
        var repository = new LoteProdutoRepositoryFake
        {
            CodigosProdutoExistentes = ["MTG2"]
        };

        var resultado = await CriarUseCase(repository)
            .ExecutarAsync(CriarCommand("MTG", 3));

        Assert.True(resultado.TeveFalha);
        Assert.Contains("MTG2", resultado.Messages.Single().Descricao);
        Assert.Null(repository.LoteAdicionado);
    }

    private static ExecutarGeracaoProdutosLoteCase CriarUseCase(
        LoteProdutoRepositoryFake repository)
        => new(
            repository,
            new GeracaoProdutosLoteValidador(new ProdutoValidador()),
            new SelecionarCategoriasProdutoCase(new CategoriaRepositoryProdutoFake()),
            new CriarLoteParaPersistenciaCase(
                repository,
                new ProdutoMapping(),
                new FixedTimeProvider(new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero))));

    internal static GeracaoProdutosLoteCommand CriarCommand(string codigoBase, int quantidade)
        => new(codigoBase, quantidade, "Monitor novo", null,
            new DateTime(2026, 8, 24), 1500m, []);
}

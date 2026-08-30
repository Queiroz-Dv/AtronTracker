using AtronStock.Application.DTO.Request;
using AtronStock.Application.UseCases.ProdutoCases;
using AtronStock.Application.Validacoes;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using Stock.Tests.TestSupport.Fakes;
using Xunit;

namespace Stock.Tests;

public sealed class SolicitarGeracaoProdutosLoteCaseTests
{
    [Fact]
    public async Task ExecutarAsync_DevePersistirSolicitacaoPendenteSemCriarProdutos()
    {
        var repository = new ProcessamentoProdutoLoteRepositoryFake();
        var useCase = CriarUseCase(repository, "usr001");

        var resultado = await useCase.ExecutarAsync(CriarRequest());

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(7, resultado.Dados!.ProcessamentoId);
        Assert.Equal(EStatusProcessamentoProdutoLote.Pendente, resultado.Dados.Status);
        var processamento = repository.ProcessamentoAdicionado!;
        Assert.Equal("usr001", processamento.Solicitacao.SolicitanteCodigo);
        Assert.Equal("mtg", processamento.Solicitacao.CodigoBase);
        Assert.Equal(3, processamento.Solicitacao.QuantidadeSolicitada);
        Assert.Null(processamento.LoteProdutoId);
        Assert.Equal(0, processamento.Resultado.QuantidadeProcessada);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRecusarSolicitanteNaoIdentificado()
    {
        var repository = new ProcessamentoProdutoLoteRepositoryFake();

        var resultado = await CriarUseCase(repository, string.Empty)
            .ExecutarAsync(CriarRequest());

        Assert.True(resultado.TeveFalha);
        Assert.Null(repository.ProcessamentoAdicionado);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRecusarCategoriaInativa()
    {
        var repository = new ProcessamentoProdutoLoteRepositoryFake();
        var categorias = new CategoriaRepositoryProdutoFake
        {
            CategoriasSelecionadas =
            [
                new Categoria { Codigo = "INA", Status = EStatus.Inativo }
            ]
        };
        var request = CriarRequest();
        request.CategoriaCodigos = ["INA"];

        var resultado = await CriarUseCase(repository, "USR001", categorias)
            .ExecutarAsync(request);

        Assert.True(resultado.TeveFalha);
        Assert.Null(repository.ProcessamentoAdicionado);
    }

    private static SolicitarGeracaoProdutosLoteCase CriarUseCase(
        ProcessamentoProdutoLoteRepositoryFake repository,
        string usuarioCodigo,
        CategoriaRepositoryProdutoFake? categorias = null)
        => new(
            repository,
            new GeracaoProdutosLoteValidador(new ProdutoValidador()),
            new SelecionarCategoriasProdutoCase(
                categorias ?? new CategoriaRepositoryProdutoFake()),
            new UserAccessorFake(usuarioCodigo));

    private static GerarProdutosLoteRequest CriarRequest()
        => new()
        {
            CodigoBase = "mtg",
            Quantidade = 3,
            Descricao = "Monitor novo",
            DataAquisicao = new DateTime(2026, 8, 24),
            PrecoUnitario = 1500m
        };
}

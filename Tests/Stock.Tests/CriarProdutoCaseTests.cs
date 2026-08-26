using AtronStock.Application.DTO.Request;
using AtronStock.Application.Mapping;
using AtronStock.Application.UseCases.ProdutoCases;
using AtronStock.Application.Validacoes;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using Stock.Tests.TestSupport.Fakes;
using Xunit;

namespace Stock.Tests;

public sealed class CriarProdutoCaseTests
{
    [Fact]
    public async Task DeveCriarProdutoAtivoSemCategoriaELote()
    {
        var repository = new ProdutoRepositoryFake();
        var auditoria = new AuditoriaServiceProdutoFake();
        var useCase = CriarUseCase(repository, new(), auditoria);

        var resultado = await useCase.ExecutarAsync(CriarRequest(" mtg30 "));

        Assert.True(resultado.TeveSucesso);
        Assert.Null(resultado.Dados);
        Assert.Equal("Produto salvo com sucesso.", Assert.Single(resultado.Messages).Descricao);
        var produto = Assert.IsType<Produto>(repository.ProdutoAdicionado);
        Assert.Equal("MTG30", produto.Codigo);
        Assert.Equal(EStatusProduto.Ativo, produto.Status);
        Assert.Null(produto.LoteProdutoId);
        Assert.Null(produto.DataEfetivaBaixa);
        Assert.Empty(produto.Categorias);
        Assert.Single(auditoria.Criacoes);
    }

    [Fact]
    public async Task DeveRecusarCodigoDuplicadoAposNormalizacao()
    {
        var repository = new ProdutoRepositoryFake
        {
            ProdutoPorCodigo = new Produto { Codigo = "MTG30" }
        };
        var useCase = CriarUseCase(repository, new(), new());

        var resultado = await useCase.ExecutarAsync(CriarRequest("mtg30"));

        Assert.True(resultado.TeveFalha);
        Assert.Null(repository.ProdutoAdicionado);
    }

    [Fact]
    public async Task DeveRecusarCategoriaInativa()
    {
        var categorias = new CategoriaRepositoryProdutoFake
        {
            CategoriasSelecionadas =
            [
                new Categoria { Id = 1, Codigo = "CAT", Status = EStatus.Inativo }
            ]
        };
        var request = CriarRequest("MTG30");
        request.CategoriaCodigos = ["CAT"];
        var useCase = CriarUseCase(new(), categorias, new());

        var resultado = await useCase.ExecutarAsync(request);

        Assert.True(resultado.TeveFalha);
    }

    private static CriarProdutoCase CriarUseCase(
        ProdutoRepositoryFake repository,
        CategoriaRepositoryProdutoFake categorias,
        AuditoriaServiceProdutoFake auditoria)
        => new(
            repository,
            new ProdutoValidador(),
            new ProdutoMapping(),
            new SelecionarCategoriasProdutoCase(categorias),
            new AuditoriaProdutoCase(auditoria));

    private static ProdutoRequest CriarRequest(string codigo)
        => new()
        {
            Codigo = codigo,
            Descricao = "Notebook patrimonial",
            DataAquisicao = new DateTime(2026, 8, 24),
            PrecoUnitario = 3500m
        };
}

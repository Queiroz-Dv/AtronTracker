using AtronStock.Application.Mapping;
using AtronStock.Application.UseCases.ProdutoCases;
using AtronStock.Domain.Entities;
using Stock.Tests.TestSupport.Fakes;
using Xunit;

namespace Stock.Tests;

public sealed class ObterProdutoCaseTests
{
    [Fact]
    public async Task DeveObterProdutoPorCodigoNormalizado()
    {
        var repository = new ProdutoRepositoryFake
        {
            ProdutoPorCodigo = new Produto
            {
                Id = 7,
                Codigo = "MTG30",
                Descricao = "Notebook patrimonial"
            }
        };
        var useCase = new ObterProdutoCase(repository, new ProdutoMapping());

        var resultado = await useCase.ObterPorCodigoAsync("mtg30");

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(7, resultado.Dados!.Id);
        Assert.Equal("MTG30", resultado.Dados.Codigo);
    }
}

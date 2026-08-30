using AtronStock.Application.DTO.Request;
using AtronStock.Application.Mapping;
using AtronStock.Application.UseCases.ProdutoCases;
using AtronStock.Application.Validacoes;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using Stock.Tests.TestSupport.Fakes;
using Xunit;

namespace Stock.Tests;

public sealed class AtualizarProdutoCaseTests
{
    [Fact]
    public async Task DeveAtualizarCadastroPreservandoCamposControladosPelaBaixaELote()
    {
        var lote = new LoteProduto { Id = 5, Codigo = "MTG_2026" };
        var produto = new Produto
        {
            Codigo = "MTG30",
            Descricao = "Antes",
            Status = EStatusProduto.Baixado,
            DataEfetivaBaixa = new DateTime(2026, 8, 20),
            LoteProdutoId = lote.Id,
            LoteProduto = lote
        };
        var repository = new ProdutoRepositoryFake { ProdutoPorCodigo = produto };
        var auditoria = new AuditoriaServiceProdutoFake();
        var useCase = new AtualizarProdutoCase(
            repository,
            new ProdutoValidador(),
            new ProdutoMapping(),
            new SelecionarCategoriasProdutoCase(new CategoriaRepositoryProdutoFake()),
            new AuditoriaProdutoCase(auditoria));
        var request = new ProdutoAtualizacaoRequest
        {
            Descricao = "Depois",
            DataAquisicao = new DateTime(2025, 1, 2),
            PrecoUnitario = 4200m
        };

        var resultado = await useCase.ExecutarAsync("mtg30", request);

        Assert.True(resultado.TeveSucesso);
        Assert.Equal("mtg30", repository.UltimoCodigoConsultado);
        Assert.Null(resultado.Dados);
        Assert.Equal(
            "Registro MTG30 atualizado com sucesso.",
            Assert.Single(resultado.Messages).Descricao);
        Assert.Equal("MTG30", produto.Codigo);
        Assert.Equal(EStatusProduto.Baixado, produto.Status);
        Assert.Equal(new DateTime(2026, 8, 20), produto.DataEfetivaBaixa);
        Assert.Equal(5, produto.LoteProdutoId);
        Assert.Same(produto, repository.ProdutoAtualizado);
        Assert.Single(auditoria.Atualizacoes);
    }
}

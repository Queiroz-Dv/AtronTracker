using AtronStock.Application.Resources;
using System.Globalization;
using Xunit;

namespace AtronStock.Application.Tests.Resources;

public class ResourceFoundationTests
{
    [Fact]
    public void ResourcesDoStockDevemCarregarAsChavesPeloAssembly()
    {
        var casos = new[]
        {
            (CategoriaResource.ResourceManager, "Erro_CategoriaNaoEncontrada", "Categoria não encontrada."),
            (ClienteResource.ResourceManager, "Erro_ClienteNaoEncontrado", "Cliente não encontrado."),
            (FornecedorResource.ResourceManager, "Erro_FornecedorNaoEncontrado", "Fornecedor não encontrado."),
            (ProdutoResource.ResourceManager, "Erro_ProdutoNaoEncontrado", "Produto não encontrado.")
        };

        foreach (var (resourceManager, chave, valorEsperado) in casos)
        {
            Assert.Equal(valorEsperado, resourceManager.GetString(chave, CultureInfo.GetCultureInfo("pt-BR")));
        }
    }

    [Fact]
    public void ResourceParametrizadoDeveFormatarNaOrdemDefinida()
    {
        var mensagem = string.Format(
            CultureInfo.GetCultureInfo("pt-BR"),
            FornecedorResource.Mensagem_FornecedorCriado,
            "FORN-01");

        Assert.Equal("Fornecedor FORN-01 criado com sucesso.", mensagem);
    }

    [Fact]
    public void ResourcesDevemPreservarAcentuacaoPtBr()
    {
        Assert.Equal("Categoria não encontrada.", CategoriaResource.Erro_CategoriaNaoEncontrada);
        Assert.Equal("Produto não encontrado.", ProdutoResource.Erro_ProdutoNaoEncontrado);
    }
}

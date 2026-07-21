using Application.Resources;
using System.Globalization;
using Xunit;

namespace Tracker.Tests.Resources;

public class ResourceFoundationTests
{
    [Fact]
    public void ResourcesDoTrackerDevemCarregarAsChavesPeloAssembly()
    {
        var casos = new[]
        {
            (TarefaResource.ResourceManager, "Erro_TarefaNaoEncontrada", "Tarefa não encontrada."),
            (PlanejamentoCustoResource.ResourceManager, "Erro_PlanejamentoNaoEncontrado", "Planejamento de custo não encontrado."),
            (PerfilDeAcessoResource.ResourceManager, "Erro_PerfilNaoEncontrado", "Perfil de acesso não encontrado."),
            (ModuloResource.ResourceManager, "Erro_ModuloNaoEncontrado", "Módulo não encontrado.")
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
            TarefaResource.Mensagem_TarefaAtribuidaUsuario,
            "000123");

        Assert.Equal("A tarefa 000123 foi atribuída a você.", mensagem);
    }

    [Fact]
    public void ResourcesDevemPreservarAcentuacaoPtBr()
    {
        Assert.Equal("Módulo não encontrado.", ModuloResource.Erro_ModuloNaoEncontrado);
    }
}

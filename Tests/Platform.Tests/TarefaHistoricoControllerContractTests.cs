using System.Reflection;
using Application.DTO;
using AtronPlatform.WebApi.Controllers.Tracker;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Platform.Tests;

public class TarefaHistoricoControllerContractTests
{
    [Fact]
    public void ObterHistorico_DeveExporColecaoCompletaSemParametrosDePaginacao()
    {
        var metodo = typeof(TarefaController).GetMethod(
            nameof(TarefaController.ObterHistorico),
            BindingFlags.Instance | BindingFlags.Public);

        Assert.NotNull(metodo);
        var parametro = Assert.Single(metodo.GetParameters());
        Assert.Equal("id", parametro.Name);
        Assert.Equal(typeof(int), parametro.ParameterType);
        Assert.Equal(
            typeof(Task<ActionResult<IReadOnlyCollection<TarefaMovimentacaoDTO>>>),
            metodo.ReturnType);
        var rota = Assert.Single(metodo.GetCustomAttributes<HttpGetAttribute>());
        Assert.Equal("{id}/Movimentacoes", rota.Template);
    }
}

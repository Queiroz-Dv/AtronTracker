using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Filters;
using Shared.Infrastructure.Repositories;
using Xunit;

namespace Shared.Tests.Infrastructure.Filters;

public class TransactionalAttributeTests
{
    [Fact]
    public async Task DeveConcluirTransacaoQuandoRespostaForSucesso()
    {
        var escopo = await ExecutarAsync(new OkResult());

        Assert.True(escopo.FoiConcluido);
    }

    [Theory]
    [InlineData(StatusCodes.Status400BadRequest)]
    [InlineData(StatusCodes.Status404NotFound)]
    [InlineData(StatusCodes.Status500InternalServerError)]
    public async Task NaoDeveConcluirTransacaoQuandoRespostaFalhar(int statusCode)
    {
        var escopo = await ExecutarAsync(new StatusCodeResult(statusCode));

        Assert.False(escopo.FoiConcluido);
    }

    [Fact]
    public async Task NaoDeveConcluirTransacaoQuandoActionLancarExcecao()
    {
        var escopo = await ExecutarAsync(
            resultado: null,
            excecao: new InvalidOperationException("Falha durante a action"));

        Assert.False(escopo.FoiConcluido);
    }

    private static async Task<EscopoTransacaoFalso> ExecutarAsync(
        IActionResult? resultado,
        Exception? excecao = null)
    {
        var gerenciador = new GerenciadorTransacaoFalso();
        var servicos = new ServiceCollection()
            .AddSingleton<ITransactionManager>(gerenciador)
            .BuildServiceProvider();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = servicos
        };
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());
        var contextoExecucao = new ActionExecutingContext(
            actionContext,
            [],
            new Dictionary<string, object?>(),
            new object());
        var contextoExecutado = new ActionExecutedContext(
            actionContext,
            [],
            new object())
        {
            Result = resultado,
            Exception = excecao
        };

        await new TransactionalAttribute().OnActionExecutionAsync(
            contextoExecucao,
            () => Task.FromResult(contextoExecutado));

        return gerenciador.Escopo;
    }

    private sealed class GerenciadorTransacaoFalso : ITransactionManager
    {
        public EscopoTransacaoFalso Escopo { get; } = new();

        public ITransactionScope CreateScope() => Escopo;
    }

    private sealed class EscopoTransacaoFalso : ITransactionScope
    {
        public bool FoiConcluido { get; private set; }

        public void Complete() => FoiConcluido = true;

        public void Dispose()
        {
        }
    }
}

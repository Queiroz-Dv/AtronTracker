using AtronPlatform.WebApi.Controllers.Transversais;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Xunit;

namespace Notificacoes.Tests.Autorizacao;

public class ProntidaoControllerTests
{
    [Fact]
    public void Prontidao_DeveSerEndpointProtegidoEDocumentado()
    {
        var controllerType = typeof(ProntidaoNotificacoesController);

        var authorize = controllerType.GetCustomAttribute<AuthorizeAttribute>();
        var route = controllerType.GetCustomAttribute<RouteAttribute>();
        var action = controllerType.GetMethod(nameof(ProntidaoNotificacoesController.ObterProntidao));
        var httpGet = action?.GetCustomAttribute<HttpGetAttribute>();

        Assert.NotNull(authorize);
        Assert.Equal("api/notificacoes", route?.Template);
        Assert.Equal("prontidao", httpGet?.Template);
    }
}

using System.Reflection;
using AtronNotificacoes.Security;
using AtronPlatform.WebApi.Controllers.Transversais;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Notificacoes.Tests.Integracao;

public sealed class NotificacoesInternasControllerTests
{
    [Fact]
    public void Consulta_deve_manter_a_rota_publica_e_exigir_usuario_com_codigo()
    {
        var controllerType = typeof(NotificacoesInternasController);
        var authorize = controllerType.GetCustomAttribute<AuthorizeAttribute>();
        var route = controllerType.GetCustomAttribute<RouteAttribute>();

        Assert.Equal(SegurancaNotificacoes.PoliticaUsuario, authorize?.Policy);
        Assert.Equal("api/notificacoes", route?.Template);
    }

    [Fact]
    public void Publicacao_nao_deve_ser_exposta_por_controller()
    {
        var controllers = typeof(AtronPlatform.WebApi.Program).Assembly
            .GetTypes()
            .Where(type => type.IsAssignableTo(typeof(ControllerBase)));

        Assert.DoesNotContain(
            controllers,
            controller => controller.Name == "PublicacaoNotificacoesInternasController");
    }
}

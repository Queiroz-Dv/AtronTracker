using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Users;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using System.Security.Claims;
using Xunit;
using Infrastructure.Authorization;

namespace Tracker.Tests.Autorizacao;

public class ModuloHandlerTests
{
    private const string CodigoUsuario = "USR-TESTE";

    [Theory]
    [InlineData("CAT")]
    [InlineData("EMP")]
    public async Task HandleAsync_DeveAutorizarUsuarioComModulo(string codigo)
    {
        var handler = CriarHandlerComModulos(codigo);
        var requirement = new ModuloRequirement(codigo);
        var context = CriarContexto(requirement);

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Theory]
    [InlineData("CAT")]
    [InlineData("EMP")]
    public async Task HandleAsync_NaoDeveAutorizarUsuarioSemModulo(string codigo)
    {
        var handler = CriarHandlerComModulos("TAR");
        var requirement = new ModuloRequirement(codigo);
        var context = CriarContexto(requirement);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_NaoDeveAutorizarUsuarioSemPerfil()
    {
        var handler = CriarHandler(new DadosComplementaresDoUsuarioDTO());
        var requirement = new ModuloRequirement("USR");
        var context = CriarContexto(requirement);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    private static ModuloHandler CriarHandlerComModulos(params string[] codigosDosModulos)
    {
        var dadosComplementares = new DadosComplementaresDoUsuarioDTO
        {
            DadosDoPerfil =
            [
                new DadosDoPerfilDTO
                {
                    CodigoPerfil = "PERFIL-TESTE",
                    Modulos = codigosDosModulos
                        .Select(codigo => new DadosDoModuloDTO(codigo, codigo))
                        .ToList()
                }
            ]
        };

        return CriarHandler(dadosComplementares);
    }

    private static ModuloHandler CriarHandler(
        DadosComplementaresDoUsuarioDTO dadosComplementares)
    {
        var cacheService = new Mock<ICacheService>();
        cacheService
            .Setup(service => service.ObterCache<DadosComplementaresDoUsuarioDTO>(
                It.Is<ChaveCache>(chave =>
                    chave.Chave == ECacheKeysInfo.Acesso
                    && chave.Descricao.EndsWith($":{CodigoUsuario}"))))
            .Returns(dadosComplementares);

        return new ModuloHandler(
            cacheService.Object,
            Mock.Of<IUsuarioService>(),
            Mock.Of<IAccessorService>());
    }

    private static AuthorizationHandlerContext CriarContexto(ModuloRequirement requirement)
    {
        var usuario = new ClaimsPrincipal(
            new ClaimsIdentity(
                [new Claim(ClaimCode.CODIGO_USUARIO, CodigoUsuario)],
                authenticationType: "Teste"));

        return new AuthorizationHandlerContext([requirement], usuario, resource: null);
    }
}

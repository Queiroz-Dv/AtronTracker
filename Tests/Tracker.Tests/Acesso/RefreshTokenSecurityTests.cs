using Application.DTO.Request;
using Application.Services.Identity;
using Domain.Entities;
using Domain.Interfaces.Identity;
using Microsoft.AspNetCore.Http;
using Moq;
using Shared.Application.DTOS.Auth;
using Shared.Application.Security;
using Shared.Application.Services.Factory;
using Xunit;

namespace Tracker.Tests.Acesso;

public class RefreshTokenSecurityTests
{
    [Fact]
    public void Hash_DeveSerSha256HexadecimalENaoExporToken()
    {
        const string token = "refresh-token-opaco";

        var hash = RefreshTokenHash.Obter(token);

        Assert.Equal(64, hash.Length);
        Assert.NotEqual(token, hash);
        Assert.Equal(hash, RefreshTokenHash.Obter(token));
    }

    [Fact]
    public async Task UserIdentityService_DevePersistirEBuscarSomenteHash()
    {
        var repository = new Mock<IUsuarioIdentityRepository>();
        var service = new UserIdentityService(repository.Object);
        var expiracao = DateTime.UtcNow.AddDays(7);

        await service.GravarRefreshTokenAsync("USR001", "token-bruto", expiracao);
        await service.ObterSessaoRefreshTokenAsync("token-bruto");
        await service.RotacionarRefreshTokenAsync(new RotacaoRefreshToken(
            "USR001", "token-bruto", "token-novo", expiracao));

        var hashAtual = RefreshTokenHash.Obter("token-bruto");
        var hashNovo = RefreshTokenHash.Obter("token-novo");
        repository.Verify(repo => repo.AtualizarRefreshTokenUsuarioRepositoryAsync("USR001", hashAtual, expiracao));
        repository.Verify(repo => repo.ObterSessaoRefreshTokenRepositoryAsync(hashAtual));
        repository.Verify(repo => repo.RotacionarRefreshTokenRepositoryAsync(It.Is<RotacaoRefreshTokenHash>(rotacao =>
            rotacao.HashAtual == hashAtual && rotacao.NovoHash == hashNovo)));
    }

    [Fact]
    public async Task CookieFixo_DeveSerLidoPorNovaInstanciaSemDataProtection()
    {
        var cookiesResposta = new Mock<IResponseCookies>();
        var primeiraInstancia = new CookieFactory(cookiesResposta.Object);
        var expiracao = DateTime.UtcNow.AddDays(7);
        CookieOptions? opcoes = null;

        cookiesResposta
            .Setup(cookies => cookies.Append(
                CookieFactory.NomeCookieRefreshToken,
                "token-opaco",
                It.IsAny<CookieOptions>()))
            .Callback<string, string, CookieOptions>((_, _, valor) => opcoes = valor);

        primeiraInstancia.CriarCookieDeRefreshToken(new DadosDoRefrehTokenDTO("token-opaco", expiracao));

        var contextoReiniciado = new DefaultHttpContext();
        contextoReiniciado.Request.Headers.Cookie = $"{CookieFactory.NomeCookieRefreshToken}=token-opaco";
        var novaInstancia = new CookieFactory(new Mock<IResponseCookies>().Object);
        var lido = await novaInstancia.ObterRefreshTokenPorRequest(contextoReiniciado.Request);

        Assert.Equal("token-opaco", lido.RefreshToken);
        Assert.True(opcoes!.HttpOnly);
        Assert.True(opcoes.Secure);
        Assert.Equal(SameSiteMode.None, opcoes.SameSite);
        Assert.Equal("/", opcoes.Path);
    }
}

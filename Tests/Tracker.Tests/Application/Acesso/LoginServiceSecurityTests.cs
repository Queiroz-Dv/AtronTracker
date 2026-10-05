using Application.UseCases.UsuarioCases;
using Application.DTO;
using Application.DTO.Request;
using Application.Interfaces.Services;
using Application.UseCases.UsuarioCases;
using Application.Records.Autenticacao;
using Application.Services.AuthServices;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Entities;
using Domain.Interfaces.Identity;
using Moq;
using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Users;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Application.Security;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Acesso;

public class LoginServiceSecurityTests
{
    [Fact]
    public async Task Refresh_DeveRotacionarTokenERecusarReuso()
    {
        var usuario = new UsuarioDTO { Codigo = "USR001", EmailConfirmado = true };
        var dependencias = CriarDependencias(usuario);
        var dadosComplementares = new DadosComplementaresDoUsuarioDTO
        {
            DadosDoUsuario = new DadosDoUsuarioDTO { CodigoDoUsuario = "USR001" }
        };
        var novosTokens = new DadosDeTokenComRefreshToken
        {
            TokenDTO = new DadosDoTokenDTO("access-novo", DateTime.UtcNow.AddMinutes(15)),
            RefrehTokenDTO = new DadosDoRefrehTokenDTO("refresh-novo", DateTime.UtcNow.AddDays(7))
        };

        dependencias.UserIdentityRepository
            .SetupSequence(repo => repo.ObterSessaoRefreshTokenRepositoryAsync(RefreshTokenHash.Obter("refresh-antigo")))
            .ReturnsAsync(new Domain.Entities.SessaoRefreshToken("USR001", DateTime.UtcNow.AddDays(1)))
            .ReturnsAsync((Domain.Entities.SessaoRefreshToken)null!);
        dependencias.UserIdentityRepository
            .Setup(repo => repo.RotacionarRefreshTokenRepositoryAsync(It.IsAny<RotacaoRefreshTokenHash>()))
            .ReturnsAsync(true);
        dadosComplementares.DadosDoUsuario.Workspace = new WorkspaceDoUsuarioDTO { Codigo = "WS001", Descricao = "Workspace" };
        dadosComplementares.DadosDoUsuario.EhResponsavelDoWorkspace = true;
        dependencias.DadosComplementaresService
            .Setup(service => service.ObterInformacoesComplementaresDoUsuario(It.IsAny<UsuarioDTO>()))
            .ReturnsAsync(dadosComplementares);
        dependencias.TokenService
            .Setup(service => service.ObterTokenComRefreshToken(It.IsAny<DadosComplementaresDoUsuarioDTO>()))
            .ReturnsAsync(novosTokens);

        var cookie = new DadosDoRefreshTokenCookieDTO { RefreshToken = "refresh-antigo" };
        var primeiraTentativa = await dependencias.Service.RefreshAcesso(cookie);
        var reuso = await dependencias.Service.RefreshAcesso(cookie);

        Assert.False(primeiraTentativa.TeveFalha);
        Assert.Equal("USR001", primeiraTentativa.Dados.UsuarioCodigo);
        Assert.True(reuso.TeveFalha);
        dependencias.UserIdentityRepository.Verify(
            repo => repo.RotacionarRefreshTokenRepositoryAsync(It.Is<RotacaoRefreshTokenHash>(rotacao =>
                rotacao.UsuarioCodigo == "USR001" &&
                rotacao.HashAtual == RefreshTokenHash.Obter("refresh-antigo") &&
                rotacao.NovoHash == RefreshTokenHash.Obter("refresh-novo"))),
            Times.Once);
        dependencias.CookieService.Verify(
            service => service.CriarCookieDeRefreshToken(novosTokens.RefrehTokenDTO),
            Times.Once);
    }

    [Fact]
    public async Task Logout_DeveRevogarSessaoDoUsuarioAutenticadoELimparCookieECache()
    {
        var dependencias = CriarDependencias(null);
        dependencias.UserIdentityRepository
            .Setup(repo => repo.RedefinirRefreshTokenRepositoryAsync("USR001"))
            .ReturnsAsync(true);

        var resultado = await dependencias.Service.Logout("USR001");

        Assert.False(resultado.TeveFalha);
        dependencias.CookieService.Verify(service => service.RemoverCookieDeRefreshToken(), Times.Once);
        dependencias.CacheUsuarioService.Verify(
            service => service.RemoverCacheDeAcessoTokenInfo("USR001"),
            Times.Once);
    }

    [Fact]
    public async Task CredenciaisInvalidas_NaoDevemGerarNemPersistirTokens()
    {
        var dependencias = CriarDependencias(new UsuarioDTO
        {
            Codigo = "USR001",
            EmailConfirmado = true
        });
        dependencias.LoginRepository
            .Setup(repository => repository.ValidarCredenciaisAsync("USR001", "senha-incorreta"))
            .ReturnsAsync(false);

        var resultado = await dependencias.Service.Autenticar(new LoginRequestDTO
        {
            CodigoDoUsuario = "USR001",
            Senha = "senha-incorreta"
        });

        Assert.True(resultado.TeveFalha);
        Assert.Equal(AuthResource.Erro_Autenticacao, Assert.Single(resultado.Messages).Descricao);
        dependencias.TokenService.Verify(
            service => service.ObterTokenComRefreshToken(It.IsAny<Shared.Application.DTOS.Users.DadosComplementaresDoUsuarioDTO>()),
            Times.Never);
        dependencias.UserIdentityRepository.Verify(
            repo => repo.AtualizarRefreshTokenUsuarioRepositoryAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>()),
            Times.Never);
        dependencias.CookieService.Verify(
            service => service.CriarCookieDeRefreshToken(It.IsAny<DadosDoRefrehTokenDTO>()),
            Times.Never);
    }

    [Fact]
    public async Task Login_NaoDeveDistinguirUsuarioInexistenteDeEmailNaoConfirmado()
    {
        var usuarioInexistente = CriarDependencias(null);
        var emailNaoConfirmado = CriarDependencias(new UsuarioDTO
        {
            Codigo = "USR001",
            EmailConfirmado = false
        });
        emailNaoConfirmado.LoginRepository
            .Setup(repository => repository.ValidarCredenciaisAsync("USR001", "senha-correta"))
            .ReturnsAsync(true);

        var request = new LoginRequestDTO
        {
            CodigoDoUsuario = "USR001",
            Senha = "senha-correta"
        };

        var inexistente = await usuarioInexistente.Service.Autenticar(request);
        var naoConfirmado = await emailNaoConfirmado.Service.Autenticar(request);

        Assert.True(inexistente.TeveFalha);
        Assert.True(naoConfirmado.TeveFalha);
        Assert.Equal(
            inexistente.Messages.Select(mensagem => mensagem.Descricao),
            naoConfirmado.Messages.Select(mensagem => mensagem.Descricao));
        emailNaoConfirmado.TokenService.Verify(
            service => service.ObterTokenComRefreshToken(It.IsAny<Shared.Application.DTOS.Users.DadosComplementaresDoUsuarioDTO>()),
            Times.Never);
    }

    [Fact]
    public async Task Dono_SemCargoDepartamento_DeveConseguirLogar()
    {
        var usuario = new UsuarioDTO { Codigo = "USR001", EmailConfirmado = true };
        var dependencias = CriarDependencias(usuario);
        dependencias.LoginRepository
            .Setup(repository => repository.ValidarCredenciaisAsync("USR001", "senha"))
            .ReturnsAsync(true);
            
        var dadosComplementares = new DadosComplementaresDoUsuarioDTO
        {
            DadosDoUsuario = new DadosDoUsuarioDTO 
            { 
                CodigoDoUsuario = "USR001",
                Workspace = new WorkspaceDoUsuarioDTO { Codigo = "WS001" },
                EhResponsavelDoWorkspace = true,
                CodigoDoCargo = "", // Sem cargo
                CodigoDoDepartamento = "" // Sem departamento
            }
        };
        
        dependencias.DadosComplementaresService
            .Setup(service => service.ObterInformacoesComplementaresDoUsuario(It.IsAny<UsuarioDTO>()))
            .ReturnsAsync(dadosComplementares);
            
        dependencias.TokenService
            .Setup(service => service.ObterTokenComRefreshToken(It.IsAny<DadosComplementaresDoUsuarioDTO>()))
            .ReturnsAsync(new DadosDeTokenComRefreshToken
            {
                TokenDTO = new DadosDoTokenDTO("access", DateTime.UtcNow.AddMinutes(15)),
                RefrehTokenDTO = new DadosDoRefrehTokenDTO("refresh", DateTime.UtcNow.AddDays(7))
            });

        dependencias.UserIdentityRepository
            .Setup(repo => repo.AtualizarRefreshTokenUsuarioRepositoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);

        var request = new LoginRequestDTO { CodigoDoUsuario = "USR001", Senha = "senha" };
        var resultado = await dependencias.Service.Autenticar(request);

        Assert.False(resultado.TeveFalha);
    }

    [Fact]
    public async Task Membro_SemCargoDepartamento_NaoDeveLogar()
    {
        var usuario = new UsuarioDTO { Codigo = "USR001", EmailConfirmado = true };
        var dependencias = CriarDependencias(usuario);
        dependencias.LoginRepository
            .Setup(repository => repository.ValidarCredenciaisAsync("USR001", "senha"))
            .ReturnsAsync(true);
            
        var dadosComplementares = new DadosComplementaresDoUsuarioDTO
        {
            DadosDoUsuario = new DadosDoUsuarioDTO 
            { 
                CodigoDoUsuario = "USR001",
                Workspace = new WorkspaceDoUsuarioDTO { Codigo = "WS001" },
                EhResponsavelDoWorkspace = false, // É membro comum
                CodigoDoCargo = "", // Sem cargo
                CodigoDoDepartamento = "" // Sem departamento
            }
        };
        
        dependencias.DadosComplementaresService
            .Setup(service => service.ObterInformacoesComplementaresDoUsuario(It.IsAny<UsuarioDTO>()))
            .ReturnsAsync(dadosComplementares);

        var request = new LoginRequestDTO { CodigoDoUsuario = "USR001", Senha = "senha" };
        var resultado = await dependencias.Service.Autenticar(request);

        Assert.True(resultado.TeveFalha);
        Assert.Equal(AuthResource.Erro_AcessoBloqueadoMembroSemVinculo, resultado.Messages.First().Descricao);
    }

    [Fact]
    public async Task Refresh_Dono_SemCargoDepartamento_DeveRenovarToken()
    {
        var usuario = new UsuarioDTO { Codigo = "USR001", EmailConfirmado = true };
        var dependencias = CriarDependencias(usuario);
        var dadosComplementares = new DadosComplementaresDoUsuarioDTO
        {
            DadosDoUsuario = new DadosDoUsuarioDTO 
            { 
                CodigoDoUsuario = "USR001",
                Workspace = new WorkspaceDoUsuarioDTO { Codigo = "WS001" },
                EhResponsavelDoWorkspace = true,
                CodigoDoCargo = "",
                CodigoDoDepartamento = ""
            }
        };

        dependencias.UserIdentityRepository
            .Setup(repo => repo.ObterSessaoRefreshTokenRepositoryAsync(RefreshTokenHash.Obter("refresh-antigo")))
            .ReturnsAsync(new Domain.Entities.SessaoRefreshToken("USR001", DateTime.UtcNow.AddDays(1)));
        dependencias.UserIdentityRepository
            .Setup(repo => repo.RotacionarRefreshTokenRepositoryAsync(It.IsAny<RotacaoRefreshTokenHash>()))
            .ReturnsAsync(true);
        dependencias.DadosComplementaresService
            .Setup(service => service.ObterInformacoesComplementaresDoUsuario(It.IsAny<UsuarioDTO>()))
            .ReturnsAsync(dadosComplementares);
        dependencias.TokenService
            .Setup(service => service.ObterTokenComRefreshToken(It.IsAny<DadosComplementaresDoUsuarioDTO>()))
            .ReturnsAsync(new DadosDeTokenComRefreshToken
            {
                TokenDTO = new DadosDoTokenDTO("access", DateTime.UtcNow.AddMinutes(15)),
                RefrehTokenDTO = new DadosDoRefrehTokenDTO("refresh", DateTime.UtcNow.AddDays(7))
            });

        var cookie = new DadosDoRefreshTokenCookieDTO { RefreshToken = "refresh-antigo" };
        var resultado = await dependencias.Service.RefreshAcesso(cookie);

        Assert.False(resultado.TeveFalha);
    }

    [Fact]
    public async Task Refresh_Membro_SemCargoDepartamento_NaoDeveRenovar()
    {
        var usuario = new UsuarioDTO { Codigo = "USR001", EmailConfirmado = true };
        var dependencias = CriarDependencias(usuario);
        var dadosComplementares = new DadosComplementaresDoUsuarioDTO
        {
            DadosDoUsuario = new DadosDoUsuarioDTO 
            { 
                CodigoDoUsuario = "USR001",
                Workspace = new WorkspaceDoUsuarioDTO { Codigo = "WS001" },
                EhResponsavelDoWorkspace = false,
                CodigoDoCargo = "",
                CodigoDoDepartamento = ""
            }
        };

        dependencias.UserIdentityRepository
            .Setup(repo => repo.ObterSessaoRefreshTokenRepositoryAsync(RefreshTokenHash.Obter("refresh-antigo")))
            .ReturnsAsync(new Domain.Entities.SessaoRefreshToken("USR001", DateTime.UtcNow.AddDays(1)));
            
        dependencias.DadosComplementaresService
            .Setup(service => service.ObterInformacoesComplementaresDoUsuario(It.IsAny<UsuarioDTO>()))
            .ReturnsAsync(dadosComplementares);

        var cookie = new DadosDoRefreshTokenCookieDTO { RefreshToken = "refresh-antigo" };
        var resultado = await dependencias.Service.RefreshAcesso(cookie);

        Assert.True(resultado.TeveFalha);
        Assert.Equal(AuthResource.Erro_Autenticacao, resultado.Messages.First().Descricao);
    }

    private static DependenciasLogin CriarDependencias(UsuarioDTO? usuario)
    {
        var loginRepository = new Mock<ILoginRepository>();
        var usuarioService = new Mock<ObterUsuarioCase>();
        var dadosComplementaresService = new Mock<IDadosComplementaresDoUsuarioService>();
        var tokenService = new Mock<ITokenService>();
        var cacheUsuarioService = new Mock<ICacheUsuarioService>();
        var cookieService = new Mock<ICookieService>();
        var userIdentityRepository = new Mock<IUsuarioIdentityRepository>();

        usuarioService
            .Setup(service => service.ExecutarAsync(It.IsAny<string>()))
            .ReturnsAsync(usuario is null
                ? Resultado<UsuarioDTO>.Falha(AuthResource.Erro_Autenticacao)
                : Resultado<UsuarioDTO>.Sucesso(usuario));

        var service = new LoginService(
            loginRepository.Object,
            usuarioService.Object,
            dadosComplementaresService.Object,
            tokenService.Object,
            cacheUsuarioService.Object,
            cookieService.Object,
            userIdentityRepository.Object);

        return new DependenciasLogin(
            service,
            loginRepository,
            tokenService,
            cookieService,
            userIdentityRepository,
            dadosComplementaresService,
            cacheUsuarioService);
    }

    private sealed record DependenciasLogin(
        LoginService Service,
        Mock<ILoginRepository> LoginRepository,
        Mock<ITokenService> TokenService,
        Mock<ICookieService> CookieService,
        Mock<IUsuarioIdentityRepository> UserIdentityRepository,
        Mock<IDadosComplementaresDoUsuarioService> DadosComplementaresService,
        Mock<ICacheUsuarioService> CacheUsuarioService);
}





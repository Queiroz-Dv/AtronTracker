using Application.DTO.Request;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.UseCases.UsuarioCases;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Moq;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Usuarios;

public sealed class AtualizarUsuarioCaseTests
{
    [Fact]
    public async Task ExecutarAsync_DeveOrquestrarAtualizacaoAuditoriaEInvalidacaoDoCache()
    {
        var usuario = CriarUsuario();
        var request = CriarRequest();
        var cenario = CriarCenario(usuario);

        var resultado = await cenario.UseCase.ExecutarAsync(request);

        Assert.True(resultado.TeveSucesso);
        Assert.Same(request, resultado.Dados);
        Assert.Equal("Nome atualizado", usuario.Nome);
        Assert.Equal("Sobrenome atualizado", usuario.Sobrenome);
        cenario.UsuarioRepository.Verify(
            repository => repository.AtualizarUsuarioAsync(usuario),
            Times.Once);
        cenario.Auditoria.Verify(
            service => service.AtualizarServiceAsync(It.IsAny<IAuditoriaDTO>()),
            Times.Once);
        cenario.Cache.Verify(
            service => service.RemoverCacheDeAcessoTokenInfo(usuario.Codigo),
            Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveInterromperFluxoQuandoCredenciaisFalharem()
    {
        var usuario = CriarUsuario();
        var request = CriarRequest();
        request.Senha = "NovaSenha!123";
        var cenario = CriarCenario(usuario, credenciaisAtualizadas: false);

        var resultado = await cenario.UseCase.ExecutarAsync(request);

        Assert.True(resultado.TeveFalha);
        cenario.Auditoria.Verify(
            service => service.AtualizarServiceAsync(It.IsAny<IAuditoriaDTO>()),
            Times.Never);
        cenario.Cache.Verify(
            service => service.RemoverCacheDeAcessoTokenInfo(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Associacao_DeveSubstituirRelacionamentoQuandoCargoOuDepartamentoMudarem()
    {
        var usuario = CriarUsuario();
        var request = CriarRequest();
        request.DepartamentoCodigo = "NOVO";
        request.CargoCodigo = "CRG";
        var departamento = new Departamento { Id = 20, Codigo = "NOVO", Descricao = "Novo" };
        var cargo = new Cargo { Id = 30, Codigo = "CRG", Descricao = "Cargo" };
        var relacionamento = new UsuarioCargoDepartamento
        {
            UsuarioId = usuario.Id,
            UsuarioCodigo = usuario.Codigo,
            DepartamentoId = 10,
            DepartamentoCodigo = "ANT",
            CargoId = 11,
            CargoCodigo = "OLD"
        };
        var departamentos = new Mock<IDepartamentoRepository>();
        departamentos
            .Setup(repository => repository.ObterDepartamentoPorCodigoRepository("NOVO"))
            .ReturnsAsync(departamento);
        var cargos = new Mock<ICargoRepository>();
        cargos
            .Setup(repository => repository.ObterCargoPorCodigoAsync("CRG"))
            .ReturnsAsync(cargo);
        var relacionamentos = new Mock<IUsuarioCargoDepartamentoRepository>();
        relacionamentos
            .Setup(repository => repository.ObterPorChaveDoUsuario(usuario.Id, usuario.Codigo))
            .ReturnsAsync(relacionamento);
        relacionamentos
            .Setup(repository => repository.RemoverAssociacaoUsuarioCargoDepartamento(relacionamento))
            .ReturnsAsync(true);
        relacionamentos
            .Setup(repository => repository.GravarAssociacaoUsuarioCargoDepartamento(usuario, cargo, departamento))
            .ReturnsAsync(true);
        var useCase = new AtualizarAssociacaoUsuarioCargoDepartamentoCase(
            departamentos.Object,
            cargos.Object,
            relacionamentos.Object);

        var resultado = await useCase.ExecutarAsync(request, usuario);

        Assert.True(resultado.TeveSucesso);
        relacionamentos.Verify(
            repository => repository.RemoverAssociacaoUsuarioCargoDepartamento(relacionamento),
            Times.Once);
        relacionamentos.Verify(
            repository => repository.GravarAssociacaoUsuarioCargoDepartamento(usuario, cargo, departamento),
            Times.Once);
    }

    private static Cenario CriarCenario(Usuario usuario, bool credenciaisAtualizadas = true)
    {
        var validador = new Mock<IValidador<UsuarioRequest>>();
        validador
            .Setup(service => service.Validar(It.IsAny<UsuarioRequest>()))
            .Returns([]);

        var usuarios = new Mock<IUsuarioRepository>();
        usuarios
            .Setup(repository => repository.ObterUsuarioPorCodigoAsync(usuario.Codigo))
            .ReturnsAsync(usuario);
        usuarios
            .Setup(repository => repository.AtualizarUsuarioAsync(usuario))
            .ReturnsAsync(true);

        var identity = new Mock<IUsuarioIdentityRepository>();
        identity
            .Setup(repository => repository.AtualizarUserIdentityRepositoryAsync(
                usuario.Codigo,
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(credenciaisAtualizadas);

        var departamentos = new Mock<IDepartamentoRepository>();
        var cargos = new Mock<ICargoRepository>();
        var relacionamentos = new Mock<IUsuarioCargoDepartamentoRepository>();

        var auditoria = new Mock<IAuditoriaService>();
        auditoria
            .Setup(service => service.AtualizarServiceAsync(It.IsAny<IAuditoriaDTO>()))
            .ReturnsAsync(Resultado.Sucesso());

        var cache = new Mock<ICacheUsuarioService>();

        var useCase = new AtualizarUsuarioCase(
            new VerificarAtualizacaoUsuarioCase(validador.Object, usuarios.Object),
            new VincularGestorImediatoCase(usuarios.Object),
            new AtualizarCredenciaisUsuarioCase(identity.Object),
            new AtualizarAssociacaoUsuarioCargoDepartamentoCase(
                departamentos.Object,
                cargos.Object,
                relacionamentos.Object),
            new AuditoriaUsuarioCase(auditoria.Object),
            new UsuarioRequestMapping(),
            usuarios.Object,
            cache.Object);

        return new Cenario(useCase, usuarios, auditoria, cache);
    }

    private static Usuario CriarUsuario()
    {
        return new Usuario
        {
            Id = 7,
            Codigo = "USR001",
            Nome = "Nome anterior",
            Sobrenome = "Sobrenome anterior",
            Email = "usuario@teste.com"
        };
    }

    private static UsuarioRequest CriarRequest()
    {
        return new UsuarioRequest
        {
            Codigo = "USR001",
            Nome = "Nome atualizado",
            Sobrenome = "Sobrenome atualizado",
            Email = "usuario@teste.com"
        };
    }

    private sealed record Cenario(
        AtualizarUsuarioCase UseCase,
        Mock<IUsuarioRepository> UsuarioRepository,
        Mock<IAuditoriaService> Auditoria,
        Mock<ICacheUsuarioService> Cache);
}

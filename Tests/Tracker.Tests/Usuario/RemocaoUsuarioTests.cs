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

public class RemocaoUsuarioTests
{
    private const string CodigoUsuario = "USR001";

    [Fact]
    public async Task DeveRepassarTarefasAoDepartamentoRemoverUsuarioERegistrarHistorico()
    {
        var usuario = new Usuario
        {
            Id = 1,
            Codigo = CodigoUsuario,
            Nome = "Usuário",
            Email = "usuario@teste.com"
        };
        var associacao = new UsuarioCargoDepartamento
        {
            UsuarioId = usuario.Id,
            UsuarioCodigo = usuario.Codigo,
            DepartamentoId = 10,
            DepartamentoCodigo = "DPT001",
            CargoId = 20,
            CargoCodigo = "CRG001"
        };
        var tarefa = new Tarefa
        {
            Id = 100,
            UsuarioId = usuario.Id,
            UsuarioCodigo = usuario.Codigo,
            CargoId = associacao.CargoId,
            CargoCodigo = associacao.CargoCodigo,
            TarefaEstadoId = 1
        };
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository
            .Setup(repository => repository.ObterUsuarioPorCodigoAsync(CodigoUsuario))
            .ReturnsAsync(usuario);
        usuarioRepository
            .Setup(repository => repository.RemoverUsuarioAsync(usuario))
            .ReturnsAsync(true);

        var associacaoRepository = new Mock<IUsuarioCargoDepartamentoRepository>();
        associacaoRepository
            .Setup(repository => repository.ObterPorChaveDoUsuario(usuario.Id, usuario.Codigo))
            .ReturnsAsync(associacao);

        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository
            .Setup(repository => repository.ObterTodasTarefasPorUsuario(usuario.Id, usuario.Codigo))
            .ReturnsAsync([tarefa]);
        tarefaRepository
            .Setup(repository => repository.AtualizarTarefaAsync(tarefa.Id, tarefa))
            .ReturnsAsync(true);

        var identityRepository = new Mock<IUsuarioIdentityRepository>();
        identityRepository
            .Setup(repository => repository.ObterUsuarioIdentityPorCodigo(CodigoUsuario))
            .ReturnsAsync((UsuarioIdentity)null!);

        var auditoriaService = new Mock<IAuditoriaService>();
        auditoriaService
            .Setup(service => service.RemoverServiceAsync(It.IsAny<IAuditoriaDTO>()))
            .ReturnsAsync(Resultado.Sucesso());

        var casoDeUso = new RemoverUsuario(
            usuarioRepository.Object,
            associacaoRepository.Object,
            tarefaRepository.Object,
            identityRepository.Object,
            auditoriaService.Object);

        var resultado = await casoDeUso.ExecutarAsync(CodigoUsuario);

        Assert.True(resultado.TeveSucesso);
        Assert.Null(tarefa.UsuarioId);
        Assert.Null(tarefa.UsuarioCodigo);
        Assert.Equal(associacao.DepartamentoId, tarefa.DepartamentoId);
        Assert.Equal(associacao.DepartamentoCodigo, tarefa.DepartamentoCodigo);
        Assert.Equal(associacao.CargoId, tarefa.CargoId);
        Assert.Equal(associacao.CargoCodigo, tarefa.CargoCodigo);
        tarefaRepository.Verify(
            repository => repository.AtualizarTarefaAsync(tarefa.Id, tarefa),
            Times.Once);
        tarefaRepository.Verify(
            repository => repository.RemoverRepositoryAsync(It.IsAny<Tarefa>()),
            Times.Never);
        usuarioRepository.Verify(
            repository => repository.RemoverUsuarioAsync(usuario),
            Times.Once);
        auditoriaService.Verify(
            service => service.RemoverServiceAsync(It.Is<IAuditoriaDTO>(
                auditoria =>
                    auditoria.CodigoRegistro == CodigoUsuario &&
                    auditoria.Contexto == "Usuario" &&
                    auditoria.Historico.Descricao.Contains("removido"))),
            Times.Once);
    }

    [Fact]
    public async Task NaoDeveRemoverUsuarioComTarefasSemDepartamento()
    {
        var usuario = new Usuario
        {
            Id = 1,
            Codigo = CodigoUsuario,
            Nome = "Usuário",
            Email = "usuario@teste.com"
        };
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository
            .Setup(repository => repository.ObterUsuarioPorCodigoAsync(CodigoUsuario))
            .ReturnsAsync(usuario);

        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository
            .Setup(repository => repository.ObterTodasTarefasPorUsuario(usuario.Id, usuario.Codigo))
            .ReturnsAsync([new Tarefa { Id = 100 }]);

        var associacaoRepository = new Mock<IUsuarioCargoDepartamentoRepository>();
        associacaoRepository
            .Setup(repository => repository.ObterPorChaveDoUsuario(usuario.Id, usuario.Codigo))
            .ReturnsAsync((UsuarioCargoDepartamento)null!);

        var identityRepository = new Mock<IUsuarioIdentityRepository>();
        var casoDeUso = new RemoverUsuario(
            usuarioRepository.Object,
            associacaoRepository.Object,
            tarefaRepository.Object,
            identityRepository.Object,
            Mock.Of<IAuditoriaService>());

        var resultado = await casoDeUso.ExecutarAsync(CodigoUsuario);

        Assert.True(resultado.TeveFalha);
        identityRepository.Verify(
            repository => repository.DeletarContaUserRepositoryAsync(It.IsAny<string>()),
            Times.Never);
        usuarioRepository.Verify(
            repository => repository.RemoverUsuarioAsync(It.IsAny<Usuario>()),
            Times.Never);
    }

    [Fact]
    public async Task NaoDeveRemoverUsuarioQuandoIdentityNaoPuderSerExcluido()
    {
        var usuario = new Usuario
        {
            Id = 1,
            Codigo = CodigoUsuario,
            Nome = "Usuário",
            Email = "usuario@teste.com"
        };
        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository
            .Setup(repository => repository.ObterUsuarioPorCodigoAsync(CodigoUsuario))
            .ReturnsAsync(usuario);

        var identityRepository = new Mock<IUsuarioIdentityRepository>();
        identityRepository
            .Setup(repository => repository.ObterUsuarioIdentityPorCodigo(CodigoUsuario))
            .ReturnsAsync(new UsuarioIdentity { Codigo = CodigoUsuario });
        identityRepository
            .Setup(repository => repository.DeletarContaUserRepositoryAsync(CodigoUsuario))
            .ReturnsAsync(false);

        var casoDeUso = new RemoverUsuario(
            usuarioRepository.Object,
            Mock.Of<IUsuarioCargoDepartamentoRepository>(),
            Mock.Of<ITarefaRepository>(),
            identityRepository.Object,
            Mock.Of<IAuditoriaService>());

        var resultado = await casoDeUso.ExecutarAsync(CodigoUsuario);

        Assert.True(resultado.TeveFalha);
        usuarioRepository.Verify(
            repository => repository.RemoverUsuarioAsync(It.IsAny<Usuario>()),
            Times.Never);
    }
}

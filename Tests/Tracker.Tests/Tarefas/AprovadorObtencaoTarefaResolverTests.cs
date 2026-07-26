using Application.Services.EntitiesServices.Tarefas;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Moq;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class AprovadorObtencaoTarefaResolverTests
{
    [Fact]
    public async Task ResolverAsync_DevePriorizarGestorImediato()
    {
        var solicitante = CriarSolicitante(
            gestorImediatoCodigo: "GST-IMEDIATO",
            ("DPT-SOLICITANTE", "GST-SOLICITANTE"));
        var tarefa = CriarTarefa("GST-TAREFA");
        var gestorImediato = new Usuario { Codigo = "GST-IMEDIATO" };
        var repositorio = new Mock<IUsuarioRepository>();
        repositorio
            .Setup(item => item.ObterUsuarioPorCodigoAsync("GST-IMEDIATO"))
            .ReturnsAsync(gestorImediato);

        var resolver = new AprovadorObtencaoTarefaResolver(repositorio.Object);

        var resultado = await resolver.ResolverAsync(solicitante, tarefa);

        Assert.Same(gestorImediato, resultado);
        repositorio.Verify(
            item => item.ObterUsuarioPorCodigoAsync("GST-IMEDIATO"),
            Times.Once);
        repositorio.Verify(
            item => item.ObterUsuarioPorCodigoAsync("GST-TAREFA"),
            Times.Never);
        repositorio.Verify(
            item => item.ObterUsuarioPorCodigoAsync("GST-SOLICITANTE"),
            Times.Never);
    }

    [Fact]
    public async Task ResolverAsync_DeveUsarGestorDaTarefaQuandoGestorImediatoNaoForValido()
    {
        var solicitante = CriarSolicitante(
            gestorImediatoCodigo: "GST-IMEDIATO",
            ("DPT-SOLICITANTE", "GST-SOLICITANTE"));
        var tarefa = CriarTarefa("GST-TAREFA");
        var gestorTarefa = new Usuario { Codigo = "GST-TAREFA" };
        var repositorio = new Mock<IUsuarioRepository>();
        repositorio
            .Setup(item => item.ObterUsuarioPorCodigoAsync("GST-IMEDIATO"))
            .ReturnsAsync((Usuario)null!);
        repositorio
            .Setup(item => item.ObterUsuarioPorCodigoAsync("GST-TAREFA"))
            .ReturnsAsync(gestorTarefa);

        var resolver = new AprovadorObtencaoTarefaResolver(repositorio.Object);

        var resultado = await resolver.ResolverAsync(solicitante, tarefa);

        Assert.Same(gestorTarefa, resultado);
        repositorio.Verify(
            item => item.ObterUsuarioPorCodigoAsync("GST-SOLICITANTE"),
            Times.Never);
    }

    [Fact]
    public async Task ResolverAsync_DeveConsultarCadaGestorUmaVezEAvaliarTodosOsDepartamentosDoSolicitante()
    {
        var solicitante = CriarSolicitante(
            gestorImediatoCodigo: null!,
            ("DPT-B", "GST-B"),
            ("DPT-A", "GST-A"));
        var tarefa = CriarTarefa("GST-A");
        var gestorB = new Usuario { Codigo = "GST-B" };
        var repositorio = new Mock<IUsuarioRepository>();
        repositorio
            .Setup(item => item.ObterUsuarioPorCodigoAsync("GST-A"))
            .ReturnsAsync((Usuario)null!);
        repositorio
            .Setup(item => item.ObterUsuarioPorCodigoAsync("GST-B"))
            .ReturnsAsync(gestorB);

        var resolver = new AprovadorObtencaoTarefaResolver(repositorio.Object);

        var resultado = await resolver.ResolverAsync(solicitante, tarefa);

        Assert.Same(gestorB, resultado);
        repositorio.Verify(
            item => item.ObterUsuarioPorCodigoAsync("GST-A"),
            Times.Once);
        repositorio.Verify(
            item => item.ObterUsuarioPorCodigoAsync("GST-B"),
            Times.Once);
    }

    [Fact]
    public async Task ResolverAsync_DeveIgnorarSolicitanteComoAprovador()
    {
        var solicitante = CriarSolicitante(
            gestorImediatoCodigo: "USR",
            ("DPT-SOLICITANTE", "GST-VALIDO"));
        var tarefa = CriarTarefa("USR");
        var gestorValido = new Usuario { Codigo = "GST-VALIDO" };
        var repositorio = new Mock<IUsuarioRepository>();
        repositorio
            .Setup(item => item.ObterUsuarioPorCodigoAsync("GST-VALIDO"))
            .ReturnsAsync(gestorValido);

        var resolver = new AprovadorObtencaoTarefaResolver(repositorio.Object);

        var resultado = await resolver.ResolverAsync(solicitante, tarefa);

        Assert.Same(gestorValido, resultado);
        repositorio.Verify(
            item => item.ObterUsuarioPorCodigoAsync("USR"),
            Times.Never);
    }

    private static Usuario CriarSolicitante(
        string gestorImediatoCodigo,
        params (string DepartamentoCodigo, string GestorCodigo)[] departamentos)
    {
        return new Usuario
        {
            Codigo = "USR",
            GestorImediatoCodigo = gestorImediatoCodigo,
            UsuarioCargoDepartamentos =
            [
                .. departamentos.Select(item => new UsuarioCargoDepartamento
                {
                    DepartamentoCodigo = item.DepartamentoCodigo,
                    Departamento = new Departamento
                    {
                        Codigo = item.DepartamentoCodigo,
                        GestorDepartamentoCodigo = item.GestorCodigo
                    }
                })
            ]
        };
    }

    private static Tarefa CriarTarefa(string gestorDepartamentoCodigo)
    {
        return new Tarefa
        {
            Departamento = new Departamento
            {
                GestorDepartamentoCodigo = gestorDepartamentoCodigo
            }
        };
    }
}

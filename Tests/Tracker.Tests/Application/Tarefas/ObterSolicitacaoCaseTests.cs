using Application.DTO;
using Application.Interfaces.Services;
using Application.UseCases.TarefaCases;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class ObterSolicitacaoCaseTests
{
    [Fact]
    public async Task ExecutarAsync_DeveConsultarPendentesDoAprovadorEDosDepartamentosGeridos()
    {
        var usuario = new Usuario { Id = 7, Codigo = "GESTOR" };
        var solicitacoes = new List<SolicitacaoObtencaoTarefa>
        {
            new() { Id = 11 },
            new() { Id = 12 }
        };
        var usuarioService = new Mock<IUsuarioService>();
        usuarioService
            .Setup(service => service.ObterUsuarioAtual())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(usuario));

        var departamentoService = new Mock<IDepartamentoService>();
        departamentoService
            .Setup(service => service.ObterDepartamentosPorGestor(usuario.Codigo))
            .ReturnsAsync(Resultado<IEnumerable<DepartamentoDTO>>.Sucesso(
                [new DepartamentoDTO { Codigo = "ADM" }, new DepartamentoDTO { Codigo = "FIN" }]));

        var repository = new Mock<ISolicitacaoObtencaoTarefaRepository>();
        repository
            .Setup(repo => repo.ObterPendentesPorAprovadorOuDepartamentosAsync(
                usuario.Id,
                usuario.Codigo,
                It.Is<IEnumerable<string>>(codigos => codigos.SequenceEqual(new[] { "ADM", "FIN" }))))
            .ReturnsAsync(solicitacoes);

        var mapper = new Mock<IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO>>();
        mapper
            .Setup(map => map.MapToDtos(solicitacoes))
            .Returns(solicitacoes.Select(solicitacao => new SolicitacaoObtencaoTarefaDTO { Id = solicitacao.Id }));
        var caso = new ObterSolicitacaoCase(
            mapper.Object,
            usuarioService.Object,
            departamentoService.Object,
            repository.Object);

        var resultado = await caso.ExecutarAsync();

        Assert.True(resultado.TeveSucesso);
        Assert.Equal([11, 12], resultado.Dados.Select(solicitacao => solicitacao.Id));
        repository.VerifyAll();
    }

    [Fact]
    public async Task ExecutarAsync_DeveConsultarAprovadorMesmoSemDepartamentoGerido()
    {
        var usuario = new Usuario { Id = 7, Codigo = "GESTOR" };
        var usuarioService = new Mock<IUsuarioService>();
        usuarioService
            .Setup(service => service.ObterUsuarioAtual())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(usuario));

        var departamentoService = new Mock<IDepartamentoService>();
        departamentoService
            .Setup(service => service.ObterDepartamentosPorGestor(usuario.Codigo))
            .ReturnsAsync(Resultado<IEnumerable<DepartamentoDTO>>.Sucesso([]));

        var repository = new Mock<ISolicitacaoObtencaoTarefaRepository>();
        repository
            .Setup(repo => repo.ObterPendentesPorAprovadorOuDepartamentosAsync(
                usuario.Id,
                usuario.Codigo,
                It.Is<IEnumerable<string>>(codigos => !codigos.Any())))
            .ReturnsAsync([]);

        var mapper = new Mock<IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO>>();
        mapper.Setup(map => map.MapToDtos(It.IsAny<IEnumerable<SolicitacaoObtencaoTarefa>>())).Returns([]);
        var caso = new ObterSolicitacaoCase(
            mapper.Object,
            usuarioService.Object,
            departamentoService.Object,
            repository.Object);

        var resultado = await caso.ExecutarAsync();

        Assert.True(resultado.TeveSucesso);
        Assert.Empty(resultado.Dados);
        repository.VerifyAll();
    }
}

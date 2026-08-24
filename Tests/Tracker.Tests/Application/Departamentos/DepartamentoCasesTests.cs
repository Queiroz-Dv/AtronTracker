using Application.DTO;
using Application.Mapping;
using Application.UseCases.DepartamentoCases;
using Application.Validador;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Moq;
using Xunit;

namespace Tracker.Tests.Departamentos;

public sealed class DepartamentoCasesTests
{
    [Fact]
    public async Task CriarAsync_DeveRetornarResultadoSemDados()
    {
        var repository = new Mock<IDepartamentoRepository>();
        repository
            .Setup(repositorio => repositorio.ObterDepartamentoPorCodigoRepositoryAsync("DPT"))
            .ReturnsAsync((Departamento)null!);
        repository
            .Setup(repositorio => repositorio.CriarDepartamentoRepositoryAsync(It.IsAny<Departamento>()))
            .ReturnsAsync(true);

        var useCase = CriarCase(repository.Object);

        var resultado = await useCase.ExecutarAsync(CriarDto());

        Assert.True(resultado.TeveSucesso);
        Assert.Null(resultado.Dados);
        Assert.NotEmpty(resultado.Messages);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarResultadoSemDados()
    {
        var departamento = new Departamento
        {
            Id = 10,
            Codigo = "DPT",
            Descricao = "Antes"
        };
        var repository = new Mock<IDepartamentoRepository>();
        repository
            .Setup(repositorio => repositorio.ObterDepartamentoPorCodigoRepositoryAsync("DPT"))
            .ReturnsAsync(departamento);
        repository
            .Setup(repositorio => repositorio.AtualizarDepartamentoRepositoryAsync(departamento))
            .ReturnsAsync(true);

        var useCase = AtualizarCase(repository.Object);

        var resultado = await useCase.ExecutarAsync("DPT", CriarDto("Depois"));

        Assert.True(resultado.TeveSucesso);
        Assert.Null(resultado.Dados);
        Assert.NotEmpty(resultado.Messages);
    }

    private static CriarDepartamentoCase CriarCase(IDepartamentoRepository repository)
    {
        return new CriarDepartamentoCase(
            CriarVinculacaoGestor(),
            new DepartamentoMapping(),
            repository,
            new DepartamentoValidador());
    }

    private static AtualizarDepartamentoCase AtualizarCase(IDepartamentoRepository repository)
    {
        return new AtualizarDepartamentoCase(
            CriarVinculacaoGestor(),
            new DepartamentoMapping(),
            repository,
            new DepartamentoValidador());
    }

    private static VincularGestorDepartamentoCase CriarVinculacaoGestor()
    {
        return new VincularGestorDepartamentoCase(new Mock<IUsuarioRepository>().Object);
    }

    private static DepartamentoDTO CriarDto(string descricao = "Departamento")
    {
        return new DepartamentoDTO
        {
            Codigo = "DPT",
            Descricao = descricao
        };
    }
}

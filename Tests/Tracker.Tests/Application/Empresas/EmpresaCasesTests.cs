using Application.DTO;
using Application.Mapping;
using Application.UseCases.EmpresaCases;
using Application.Validador;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tracker.Tests.Empresas;

public sealed class EmpresaCasesTests
{
    [Fact]
    public async Task CriarAsync_DevePreservarCodigoInformado()
    {
        var repository = new Mock<IEmpresaRepository>();
        repository
            .Setup(item => item.CodigoExisteAsync(" atron ", null))
            .ReturnsAsync(false);

        Empresa? empresaPersistida = null;
        repository
            .Setup(item => item.CriarAsync(It.IsAny<Empresa>()))
            .Callback<Empresa>(empresa => empresaPersistida = empresa)
            .ReturnsAsync(true);

        var useCase = new CriarEmpresaCase(
            new EmpresaMapping(),
            repository.Object,
            new EmpresaValidacoes());

        var resultado = await useCase.ExecutarAsync(CriarDto(codigo: " atron "));

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.NotNull(empresaPersistida);
        Assert.Equal(" atron ", empresaPersistida.Codigo);
        Assert.Equal("contato@atron.com", empresaPersistida.Email);
        Assert.Equal(" atron ", resultado.Dados.Codigo);
    }

    [Fact]
    public async Task CriarAsync_QuandoCodigoJaExiste_NaoDevePersistir()
    {
        var repository = new Mock<IEmpresaRepository>();
        repository
            .Setup(item => item.CodigoExisteAsync("ATRON", null))
            .ReturnsAsync(true);

        var useCase = new CriarEmpresaCase(
            new EmpresaMapping(),
            repository.Object,
            new EmpresaValidacoes());

        var resultado = await useCase.ExecutarAsync(CriarDto());

        Assert.True(resultado.TeveFalha);
        repository.Verify(item => item.CriarAsync(It.IsAny<Empresa>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DevePreservarCodigoEAlterarCamposPermitidos()
    {
        var empresa = CriarEmpresa();
        var repository = new Mock<IEmpresaRepository>();
        repository
            .Setup(item => item.ObterPorCodigoAsync(" atron ", true))
            .ReturnsAsync(empresa);
        repository
            .Setup(item => item.AtualizarAsync(empresa))
            .ReturnsAsync(true);

        var useCase = new AtualizarEmpresaCase(
            new EmpresaMapping(),
            repository.Object,
            new EmpresaValidacoes());

        var dto = CriarDto(nomeFantasia: "Atron Atualizada");
        dto.Status = StatusEmpresa.Suspensa;
        var resultado = await useCase.ExecutarAsync(" atron ", dto);

        Assert.True(resultado.TeveSucesso);
        Assert.Equal("ATRON", empresa.Codigo);
        Assert.Equal("Atron Atualizada", empresa.NomeFantasia);
        Assert.Equal(StatusEmpresa.Suspensa, empresa.Status);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveMapearEmpresas()
    {
        var repository = new Mock<IEmpresaRepository>();
        repository
            .Setup(item => item.ObterTodosAsync())
            .ReturnsAsync([CriarEmpresa()]);

        var useCase = new ObterEmpresaCase(new EmpresaMapping(), repository.Object);

        var resultado = await useCase.ObterTodosAsync();

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        var empresa = Assert.Single(resultado.Dados);
        Assert.Equal("ATRON", empresa.Codigo);
    }

    [Fact]
    public async Task ExcluirAsync_QuandoEmpresaExiste_DeveRemover()
    {
        var empresa = CriarEmpresa();
        var repository = new Mock<IEmpresaRepository>();
        repository
            .Setup(item => item.ObterPorCodigoAsync("atron", true))
            .ReturnsAsync(empresa);
        repository
            .Setup(item => item.RemoverAsync(empresa))
            .ReturnsAsync(true);

        var useCase = new ExcluirEmpresaCase(repository.Object);

        var resultado = await useCase.ExecutarAsync("atron");

        Assert.True(resultado.TeveSucesso);
        repository.Verify(item => item.RemoverAsync(empresa), Times.Once);
    }

    [Fact]
    public void Validar_QuandoEmailOuStatusSaoInvalidos_DeveFalhar()
    {
        var dto = CriarDto();
        dto.Email = "email-invalido";
        dto.Status = (StatusEmpresa)999;

        var mensagens = new EmpresaValidacoes().Validar(dto);

        Assert.NotEmpty(mensagens);
    }

    private static EmpresaDTO CriarDto(
        string codigo = "ATRON",
        string nomeFantasia = "Atron")
        => new()
        {
            Codigo = codigo,
            NomeFantasia = nomeFantasia,
            Endereco = "Rua Principal",
            Numero = "100",
            Email = "Contato@Atron.com",
            Status = StatusEmpresa.Ativa
        };

    private static Empresa CriarEmpresa()
        => new()
        {
            Id = 10,
            Codigo = "ATRON",
            NomeFantasia = "Atron",
            Endereco = "Rua Principal",
            Numero = "100",
            Email = "contato@atron.com",
            Status = StatusEmpresa.Ativa
        };
}

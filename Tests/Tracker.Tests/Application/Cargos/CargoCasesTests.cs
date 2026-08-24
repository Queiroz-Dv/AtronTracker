using Application.DTO;
using Application.Mapping;
using Application.Policies.PlanejamentoCustos;
using Application.UseCases.CargoCases;
using Application.Validador;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Moq;
using Xunit;

namespace Tracker.Tests.Cargos;

public sealed class CargoCasesTests
{
    [Fact]
    public async Task CriarAsync_DeveMapearVincularDepartamentoEPersistir()
    {
        var departamento = CriarDepartamento();
        var cargoRepository = new Mock<ICargoRepository>();
        cargoRepository
            .Setup(repository => repository.ObterCargoPorCodigoAsync("CRG"))
            .ReturnsAsync((Cargo)null!);

        Cargo? cargoPersistido = null;
        cargoRepository
            .Setup(repository => repository.CriarCargoAsync(It.IsAny<Cargo>()))
            .Callback<Cargo>(cargo => cargoPersistido = cargo)
            .ReturnsAsync(true);

        var departamentoRepository = new Mock<IDepartamentoRepository>();
        departamentoRepository
            .Setup(repository => repository.ObterDepartamentoPorCodigoRepositoryAsync("DPT"))
            .ReturnsAsync(departamento);

        var useCase = new CriarCargoCase(
            new CargoValidador(),
            new CargoMapping(),
            cargoRepository.Object,
            departamentoRepository.Object);

        var resultado = await useCase.ExecutarAsync(CriarDto());

        Assert.True(resultado.TeveSucesso);
        Assert.Null(resultado.Dados);
        Assert.NotNull(cargoPersistido);
        Assert.Equal("CRG", cargoPersistido.Codigo);
        Assert.Equal(departamento.Id, cargoPersistido.DepartamentoId);
        Assert.Equal(departamento.Codigo, cargoPersistido.DepartamentoCodigo);
    }

    [Fact]
    public async Task AtualizarAsync_DevePreservarIdentidadeEAplicarCamposPermitidos()
    {
        var cargo = new Cargo
        {
            Id = 20,
            Codigo = "CRG",
            Descricao = "Antes",
            DepartamentoId = 10,
            DepartamentoCodigo = "DPA"
        };
        var novoDepartamento = CriarDepartamento(id: 11, codigo: "DPB");
        var cargoRepository = new Mock<ICargoRepository>();
        cargoRepository
            .Setup(repository => repository.ObterCargoPorCodigoAsync("CRG"))
            .ReturnsAsync(cargo);
        cargoRepository
            .Setup(repository => repository.AtualizarCargoAsync(cargo))
            .ReturnsAsync(true);

        var departamentoRepository = new Mock<IDepartamentoRepository>();
        departamentoRepository
            .Setup(repository => repository.ObterDepartamentoPorCodigoRepositoryAsync("DPB"))
            .ReturnsAsync(novoDepartamento);

        var planejamentoRepository = new Mock<IPlanejamentoCustoRepository>();
        planejamentoRepository
            .Setup(repository => repository.ExisteCargoEmPlanejamentoAtualOuFuturoAsync(
                cargo.Id,
                cargo.Codigo,
                cargo.DepartamentoId,
                cargo.DepartamentoCodigo,
                It.IsAny<int>()))
            .ReturnsAsync(false);

        var useCase = new AtualizarCargoCase(
            new CargoValidador(),
            new CargoMapping(),
            cargoRepository.Object,
            departamentoRepository.Object,
            new EstruturaPlanejadaPolicy(planejamentoRepository.Object));

        var resultado = await useCase.ExecutarAsync(
            "CRG",
            CriarDto(descricao: "Depois", departamentoCodigo: "DPB"));

        Assert.True(resultado.TeveSucesso);
        Assert.Null(resultado.Dados);
        Assert.Equal(20, cargo.Id);
        Assert.Equal("CRG", cargo.Codigo);
        Assert.Equal("DEPOIS", cargo.Descricao);
        Assert.Equal(novoDepartamento.Id, cargo.DepartamentoId);
        Assert.Equal(novoDepartamento.Codigo, cargo.DepartamentoCodigo);
    }

    [Fact]
    public async Task ExcluirAsync_DeveValidarRelacionamentosERemover()
    {
        var cargo = new Cargo
        {
            Id = 20,
            Codigo = "CRG",
            Descricao = "Cargo",
            DepartamentoId = 10,
            DepartamentoCodigo = "DPT"
        };
        var cargoRepository = new Mock<ICargoRepository>();
        cargoRepository
            .Setup(repository => repository.ObterCargoPorCodigoAsync("CRG"))
            .ReturnsAsync(cargo);
        cargoRepository
            .Setup(repository => repository.RemoverCargoAsync(cargo))
            .ReturnsAsync(true);

        var planejamentoRepository = new Mock<IPlanejamentoCustoRepository>();
        planejamentoRepository
            .Setup(repository => repository.ExisteCargoEmPlanejamentoAtualOuFuturoAsync(
                cargo.Id,
                cargo.Codigo,
                cargo.DepartamentoId,
                cargo.DepartamentoCodigo,
                It.IsAny<int>()))
            .ReturnsAsync(false);

        var relacionamentoRepository = new Mock<IUsuarioCargoDepartamentoRepository>();
        relacionamentoRepository
            .Setup(repository => repository.ObterPorCargo(cargo.Id, cargo.Codigo))
            .ReturnsAsync([]);

        var useCase = new ExcluirCargoCase(
            cargoRepository.Object,
            new EstruturaPlanejadaPolicy(planejamentoRepository.Object),
            relacionamentoRepository.Object);

        var resultado = await useCase.ExecutarAsync("CRG");

        Assert.True(resultado.TeveSucesso);
        cargoRepository.Verify(repository => repository.RemoverCargoAsync(cargo), Times.Once);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveMapearEntidadesParaDtos()
    {
        var departamento = CriarDepartamento();
        var cargo = new Cargo
        {
            Id = 20,
            Codigo = "CRG",
            Descricao = "Cargo",
            DepartamentoId = departamento.Id,
            DepartamentoCodigo = departamento.Codigo,
            Departamento = departamento
        };
        var cargoRepository = new Mock<ICargoRepository>();
        cargoRepository
            .Setup(repository => repository.ObterCargosAsync())
            .ReturnsAsync([cargo]);
        var useCase = new ObterCargoCase(new CargoMapping(), cargoRepository.Object);

        var resultado = await useCase.ObterTodosAsync();

        Assert.NotNull(resultado.Dados);
        var dto = Assert.Single(resultado.Dados);
        Assert.Equal(cargo.Id, dto.Id);
        Assert.Equal(cargo.Codigo, dto.Codigo);
        Assert.Equal(departamento.Codigo, dto.DepartamentoCodigo);
    }

    private static CargoDTO CriarDto(
        string descricao = "Cargo",
        string departamentoCodigo = "DPT")
    {
        return new CargoDTO
        {
            Codigo = "CRG",
            Descricao = descricao,
            DepartamentoCodigo = departamentoCodigo
        };
    }

    private static Departamento CriarDepartamento(
        int id = 10,
        string codigo = "DPT")
    {
        return new Departamento
        {
            Id = id,
            Codigo = codigo,
            Descricao = "Departamento"
        };
    }
}

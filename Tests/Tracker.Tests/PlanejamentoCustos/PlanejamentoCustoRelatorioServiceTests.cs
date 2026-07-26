using Application.Services.EntitiesServices.PlanejamentoCustos;
using Domain.Entities;
using Domain.Interfaces;
using Xunit;

namespace Tracker.Tests.PlanejamentoCustos;

public class PlanejamentoCustoRelatorioServiceTests
{
    [Fact]
    public async Task ObterRelatorioGeralAsync_DeveListarApenasPlanejamentosInformadosECalcularOcupacaoPorCargosDetalhados()
    {
        var ano = DateTime.Today.Year;
        var planejamentos = new[]
        {
            new PlanejamentoCusto
            {
                Codigo = "PLC002",
                Descricao = "Planejamento B",
                Ano = ano,
                DepartamentoId = 20,
                DepartamentoCodigo = "DPB",
                Departamento = new Departamento { Id = 20, Codigo = "DPB", Descricao = "Departamento B" },
                ValorMinimo = 200,
                ValorTeto = 1000,
                ApenasDepartamento = false,
                DetalhesCargo =
                [
                    new PlanejamentoCustoCargo
                    {
                        CargoId = 200,
                        CargoCodigo = "CRG2",
                        Cargo = new Cargo { Id = 200, Codigo = "CRG2", Descricao = "Cargo 2" },
                        Detalhado = true,
                        ValorMinimo = 150,
                        ValorTeto = 400
                    },
                    new PlanejamentoCustoCargo
                    {
                        CargoId = 201,
                        CargoCodigo = "CRG3",
                        Detalhado = false
                    }
                ]
            },
            new PlanejamentoCusto
            {
                Codigo = "PLC001",
                Descricao = "Planejamento A",
                Ano = ano,
                DepartamentoId = 10,
                DepartamentoCodigo = "DPA",
                Departamento = new Departamento { Id = 10, Codigo = "DPA", Descricao = "Departamento A" },
                ValorMinimo = 100,
                ValorTeto = 500,
                ApenasDepartamento = true
            }
        };

        var cargos = new[]
        {
            new Cargo { Id = 100, Codigo = "CRG1", Descricao = "Cargo 1", DepartamentoId = 10, DepartamentoCodigo = "DPA" },
            new Cargo { Id = 200, Codigo = "CRG2", Descricao = "Cargo 2", DepartamentoId = 20, DepartamentoCodigo = "DPB" },
            new Cargo { Id = 201, Codigo = "CRG3", Descricao = "Cargo 3", DepartamentoId = 20, DepartamentoCodigo = "DPB" },
            new Cargo { Id = 300, Codigo = "CRG4", Descricao = "Cargo 4", DepartamentoId = 30, DepartamentoCodigo = "DPC" }
        };

        var service = new PlanejamentoCustoRelatorioService(
            new PlanejamentoCustoRepositoryFake(planejamentos),
            new CargoRepositoryFake(cargos));

        var resultado = await service.ObterRelatorioGeralAsync(ano);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(ano, resultado.Dados.Ano);
        Assert.Collection(
            resultado.Dados.Departamentos,
            departamento =>
            {
                Assert.Equal("DPA", departamento.DepartamentoCodigo);
                Assert.True(departamento.ApenasDepartamento);
                Assert.Empty(departamento.CargosDetalhados);
                Assert.Empty(departamento.CargosPendentes);
                Assert.Contains(
                    departamento.Informacoes,
                    informacao => informacao == "Planejamento definido apenas por departamento.");
            },
            departamento =>
            {
                Assert.Equal("DPB", departamento.DepartamentoCodigo);
                Assert.False(departamento.ApenasDepartamento);
                Assert.Equal(150, departamento.SomaMinimosCargos);
                Assert.Equal(400, departamento.SomaTetosCargos);
                Assert.Equal(40, departamento.PercentualOcupacaoTeto);
                Assert.Equal(1, departamento.QuantidadeCargosNaoDetalhados);
                Assert.Contains(
                    departamento.Informacoes,
                    informacao => informacao == "1 cargo(s) não detalhado(s).");

                var cargo = Assert.Single(departamento.CargosDetalhados);
                Assert.Equal("CRG2", cargo.CargoCodigo);
                Assert.Equal("Cargo 2", cargo.CargoDescricao);
                Assert.Equal(150, cargo.ValorMinimo);
                Assert.Equal(400, cargo.ValorTeto);
                Assert.Equal(40, cargo.PercentualOcupacaoTetoDepartamento);
            });
    }

    [Fact]
    public async Task ObterRelatorioPorCodigoAsync_DeveSinalizarCargoPendenteEmPlanejamentoDetalhado()
    {
        var ano = DateTime.Today.Year;
        var planejamento = new PlanejamentoCusto
        {
            Codigo = "PLC001",
            Descricao = "Planejamento",
            Ano = ano,
            DepartamentoId = 10,
            DepartamentoCodigo = "DPA",
            Departamento = new Departamento { Id = 10, Codigo = "DPA", Descricao = "Departamento A" },
            ValorMinimo = 100,
            ValorTeto = 500,
            ApenasDepartamento = false,
            DetalhesCargo =
            [
                new PlanejamentoCustoCargo
                {
                    CargoId = 100,
                    CargoCodigo = "CRG1",
                    Detalhado = true,
                    ValorMinimo = 80,
                    ValorTeto = 300
                }
            ]
        };

        var cargos = new[]
        {
            new Cargo { Id = 100, Codigo = "CRG1", Descricao = "Cargo 1", DepartamentoId = 10, DepartamentoCodigo = "DPA" },
            new Cargo { Id = 101, Codigo = "CRG2", Descricao = "Cargo 2", DepartamentoId = 10, DepartamentoCodigo = "DPA" }
        };

        var service = new PlanejamentoCustoRelatorioService(
            new PlanejamentoCustoRepositoryFake([planejamento]),
            new CargoRepositoryFake(cargos));

        var resultado = await service.ObterRelatorioPorCodigoAsync("PLC001");

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);

        var departamento = Assert.Single(resultado.Dados.Departamentos);
        Assert.Equal("DPA", departamento.DepartamentoCodigo);
        Assert.Equal(["CRG2 - Cargo 2"], departamento.CargosPendentes);
    }

    private sealed class PlanejamentoCustoRepositoryFake(IReadOnlyCollection<PlanejamentoCusto> planejamentos) : IPlanejamentoCustoRepository
    {
        public Task<bool> AtualizarAsync(PlanejamentoCusto planejamentoCusto) => Task.FromResult(true);

        public Task<bool> CriarAsync(PlanejamentoCusto planejamentoCusto) => Task.FromResult(true);

        public Task<bool> ExisteCargoEmPlanejamentoAtualOuFuturoAsync(int cargoId, string cargoCodigo, int departamentoId, string departamentoCodigo, int anoMinimo)
            => Task.FromResult(false);

        public Task<bool> ExisteCodigoAsync(string codigo) => Task.FromResult(false);

        public Task<bool> ExisteDepartamentoEmPlanejamentoAtualOuFuturoAsync(int departamentoId, string departamentoCodigo, int anoMinimo)
            => Task.FromResult(false);

        public Task<PlanejamentoCusto> ObterPorCodigoAsync(string codigo)
            => Task.FromResult(planejamentos.SingleOrDefault(p => p.Codigo == codigo) ?? null!);

        public Task<PlanejamentoCusto> ObterPorCodigoAsNoTrackingAsync(string codigo)
            => Task.FromResult(planejamentos.SingleOrDefault(p => p.Codigo == codigo) ?? null!);

        public Task<PlanejamentoCusto> ObterPorDepartamentoEAnoAsync(int departamentoId, string departamentoCodigo, int ano)
            => Task.FromResult(
                planejamentos.SingleOrDefault(p =>
                    p.DepartamentoId == departamentoId &&
                    p.DepartamentoCodigo == departamentoCodigo &&
                    p.Ano == ano) ?? null!);

        public Task<IEnumerable<PlanejamentoCusto>> ObterPorAnoAsync(int ano)
            => Task.FromResult<IEnumerable<PlanejamentoCusto>>(planejamentos.Where(p => p.Ano == ano));

        public Task<IEnumerable<PlanejamentoCusto>> ObterTodosAsync()
            => Task.FromResult<IEnumerable<PlanejamentoCusto>>(planejamentos);

        public Task<bool> RemoverAsync(PlanejamentoCusto planejamentoCusto) => Task.FromResult(true);
    }

    private sealed class CargoRepositoryFake(IReadOnlyCollection<Cargo> cargos) : ICargoRepository
    {
        public Task<bool> AtualizarRepositoryAsync(Cargo entity) => Task.FromResult(true);

        public Task<bool> AtualizarRepositoryAsync(int id, Cargo entity) => Task.FromResult(true);

        public Task<bool> AtualizarCargoAsync(Cargo cargo) => Task.FromResult(true);

        public Task<bool> CriarCargoAsync(Cargo cargo) => Task.FromResult(true);

        public Task<bool> CriarRepositoryAsync(Cargo entity) => Task.FromResult(true);

        public Task<Cargo> ObterCargoPorCodigoAsync(string codigo)
            => Task.FromResult(cargos.SingleOrDefault(c => c.Codigo == codigo) ?? null!);

        public Task<Cargo> ObterCargoPorIdAsync(int? id)
            => Task.FromResult(cargos.SingleOrDefault(c => c.Id == id) ?? null!);

        public Task<IEnumerable<Cargo>> ObterCargosAsync()
            => Task.FromResult<IEnumerable<Cargo>>(cargos);

        public Task<IEnumerable<Cargo>> ObterCargosPorDepartamento(int departamentoId, string departamentoCodigo)
            => Task.FromResult<IEnumerable<Cargo>>(
                cargos.Where(c => c.DepartamentoId == departamentoId && c.DepartamentoCodigo == departamentoCodigo));

        public Task<Cargo> ObterPorCodigoRepositoryAsync(string codigo)
            => Task.FromResult(cargos.SingleOrDefault(c => c.Codigo == codigo) ?? null!);

        public Task<Cargo> ObterPorIdRepositoryAsync(int id)
            => Task.FromResult(cargos.SingleOrDefault(c => c.Id == id) ?? null!);

        public Task<IEnumerable<Cargo>> ObterTodosRepositoryAsync()
            => Task.FromResult<IEnumerable<Cargo>>(cargos);

        public Task<bool> RemoverCargoAsync(Cargo cargo) => Task.FromResult(true);

        public Task<bool> RemoverRepositoryAsync(Cargo entity) => Task.FromResult(true);
    }
}

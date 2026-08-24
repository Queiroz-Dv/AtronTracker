using Application.DTO;
using Application.Mapping;
using Application.Resources;
using Application.Services.EntitiesServices.PlanejamentoCustos;
using Application.Validador;
using Domain.Entities;
using Tracker.Tests.TestSupport.Fakes.PlanejamentoCustos;
using Xunit;

namespace Tracker.Tests.PlanejamentoCustos;

public class PlanejamentoCustoPreparacaoServiceTests
{
    [Fact]
    public async Task PrepararCriacaoAsync_DevePrepararPlanejamentoDetalhadoQuandoTodosOsCargosForamDecididos()
    {
        var departamento = new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" };
        var cargos = new[]
        {
            new Cargo { Id = 20, Codigo = "CRG1", Descricao = "Cargo 1", DepartamentoId = 10, DepartamentoCodigo = "DPT" },
            new Cargo { Id = 21, Codigo = "CRG2", Descricao = "Cargo 2", DepartamentoId = 10, DepartamentoCodigo = "DPT" }
        };

        var service = new PlanejamentoCustoPreparacaoService(
            new PlanejamentoCustoValidador(),
            new PlanejamentoCustoMapping(),
            new PlanejamentoCustoRepositoryFake(),
            new DepartamentoRepositoryFake(departamento),
            new CargoRepositoryFake(cargos));

        var dto = new PlanejamentoCustoDTO
        {
            Codigo = "PLC001",
            Descricao = "Planejamento validado",
            Ano = DateTime.Today.Year,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 100,
            ValorTeto = 500,
            ApenasDepartamento = false,
            DetalhesCargo =
            [
                new PlanejamentoCustoCargoDTO
                {
                    CargoCodigo = "CRG1",
                    Detalhado = true,
                    ValorMinimo = 80,
                    ValorTeto = 300
                },
                new PlanejamentoCustoCargoDTO
                {
                    CargoCodigo = "CRG2",
                    Detalhado = false
                }
            ]
        };

        var resultado = await service.PrepararCriacaoAsync(dto);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal("DPT", resultado.Dados.Entidade.DepartamentoCodigo);
        Assert.Equal(10, resultado.Dados.Entidade.DepartamentoId);
        Assert.Equal(2, resultado.Dados.Entidade.DetalhesCargo.Count);

        var cargoDetalhado = resultado.Dados.Entidade.DetalhesCargo.Single(d => d.CargoCodigo == "CRG1");
        Assert.True(cargoDetalhado.Detalhado);
        Assert.Equal(20, cargoDetalhado.CargoId);
        Assert.Equal(80, cargoDetalhado.ValorMinimo);
        Assert.Equal(300, cargoDetalhado.ValorTeto);

        var cargoNaoDetalhado = resultado.Dados.Entidade.DetalhesCargo.Single(d => d.CargoCodigo == "CRG2");
        Assert.False(cargoNaoDetalhado.Detalhado);
        Assert.Equal(21, cargoNaoDetalhado.CargoId);
        Assert.Null(cargoNaoDetalhado.ValorMinimo);
        Assert.Null(cargoNaoDetalhado.ValorTeto);

        Assert.Contains(
            resultado.Dados.ResultadoDetalhes.Messages,
            mensagem => mensagem.Descricao == PlanejamentoCustoResource.Aviso_SomaMinimosDivergente);
    }

    [Fact]
    public async Task PrepararAtualizacaoAsync_DevePreservarIdentidadeERemoverDetalhesQuandoPlanejamentoForApenasDepartamento()
    {
        var planejamentoExistente = new PlanejamentoCusto
        {
            Id = 99,
            Codigo = "PLC001",
            Descricao = "Planejamento antigo",
            Ano = DateTime.Today.Year,
            DepartamentoId = 10,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 100,
            ValorTeto = 500,
            ApenasDepartamento = false,
            DetalhesCargo =
            [
                new PlanejamentoCustoCargo
                {
                    CargoId = 20,
                    CargoCodigo = "CRG1",
                    Detalhado = true,
                    ValorMinimo = 80,
                    ValorTeto = 300
                }
            ]
        };

        var service = new PlanejamentoCustoPreparacaoService(
            new PlanejamentoCustoValidador(),
            new PlanejamentoCustoMapping(),
            new PlanejamentoCustoRepositoryFake(planejamentoExistente),
            new DepartamentoRepositoryFake(new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" }),
            new CargoRepositoryFake([]));

        var dto = new PlanejamentoCustoDTO
        {
            Codigo = "PLC001",
            Descricao = "Planejamento atualizado",
            Ano = DateTime.Today.Year,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 120,
            ValorTeto = 600,
            ApenasDepartamento = true,
            DetalhesCargo =
            [
                new PlanejamentoCustoCargoDTO
                {
                    CargoCodigo = "CRG1",
                    Detalhado = true,
                    ValorMinimo = 80,
                    ValorTeto = 300
                }
            ]
        };

        var resultado = await service.PrepararAtualizacaoAsync("PLC001", dto);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(99, resultado.Dados.Entidade.Id);
        Assert.Equal("PLC001", resultado.Dados.Entidade.Codigo);
        Assert.Equal(DateTime.Today.Year, resultado.Dados.Entidade.Ano);
        Assert.Equal(10, resultado.Dados.Entidade.DepartamentoId);
        Assert.Equal("DPT", resultado.Dados.Entidade.DepartamentoCodigo);
        Assert.Equal("Planejamento atualizado", resultado.Dados.Entidade.Descricao);
        Assert.Equal(120, resultado.Dados.Entidade.ValorMinimo);
        Assert.Equal(600, resultado.Dados.Entidade.ValorTeto);
        Assert.True(resultado.Dados.Entidade.ApenasDepartamento);
        Assert.Empty(resultado.Dados.Entidade.DetalhesCargo);
    }

    [Fact]
    public async Task PrepararCriacaoAsync_DeveBloquearPlanejamentoDuplicadoParaMesmoDepartamentoEAno()
    {
        var anoAtual = DateTime.Today.Year;
        var planejamentoExistente = new PlanejamentoCusto
        {
            Id = 99,
            Codigo = "PLC001",
            Descricao = "Planejamento existente",
            Ano = anoAtual,
            DepartamentoId = 10,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 100,
            ValorTeto = 500,
            ApenasDepartamento = true
        };

        var service = new PlanejamentoCustoPreparacaoService(
            new PlanejamentoCustoValidador(),
            new PlanejamentoCustoMapping(),
            new PlanejamentoCustoRepositoryFake(planejamentoExistente),
            new DepartamentoRepositoryFake(new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" }),
            new CargoRepositoryFake([]));

        var dto = new PlanejamentoCustoDTO
        {
            Codigo = "PLC002",
            Descricao = "Novo planejamento",
            Ano = anoAtual,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 120,
            ValorTeto = 600,
            ApenasDepartamento = true
        };

        var resultado = await service.PrepararCriacaoAsync(dto);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == PlanejamentoCustoResource.Erro_DepartamentoAnoExistente);
    }

    [Fact]
    public async Task PrepararCriacaoAsync_DeveBloquearPlanejamentoDeAnoPassado()
    {
        var service = new PlanejamentoCustoPreparacaoService(
            new PlanejamentoCustoValidador(),
            new PlanejamentoCustoMapping(),
            new PlanejamentoCustoRepositoryFake(),
            new DepartamentoRepositoryFake(new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" }),
            new CargoRepositoryFake([]));

        var dto = new PlanejamentoCustoDTO
        {
            Codigo = "PLC001",
            Descricao = "Planejamento antigo",
            Ano = DateTime.Today.Year - 1,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 100,
            ValorTeto = 500,
            ApenasDepartamento = true
        };

        var resultado = await service.PrepararCriacaoAsync(dto);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao.Contains("ano passado"));
    }

    [Fact]
    public async Task PrepararAtualizacaoAsync_DeveBloquearPlanejamentoDeAnoPassado()
    {
        var planejamentoExistente = new PlanejamentoCusto
        {
            Id = 99,
            Codigo = "PLC001",
            Descricao = "Planejamento antigo",
            Ano = DateTime.Today.Year - 1,
            DepartamentoId = 10,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 100,
            ValorTeto = 500,
            ApenasDepartamento = true
        };

        var service = new PlanejamentoCustoPreparacaoService(
            new PlanejamentoCustoValidador(),
            new PlanejamentoCustoMapping(),
            new PlanejamentoCustoRepositoryFake(planejamentoExistente),
            new DepartamentoRepositoryFake(new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" }),
            new CargoRepositoryFake([]));

        var dto = new PlanejamentoCustoDTO
        {
            Codigo = "PLC001",
            Descricao = "Tentativa de atualizacao",
            Ano = DateTime.Today.Year - 1,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 120,
            ValorTeto = 600,
            ApenasDepartamento = true
        };

        var resultado = await service.PrepararAtualizacaoAsync("PLC001", dto);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == PlanejamentoCustoResource.Erro_AnoPassadoNaoPodeSerEditado);
    }

    [Fact]
    public async Task PrepararRemocaoAsync_DeveBloquearPlanejamentoDeAnoPassado()
    {
        var planejamentoExistente = new PlanejamentoCusto
        {
            Id = 99,
            Codigo = "PLC001",
            Descricao = "Planejamento antigo",
            Ano = DateTime.Today.Year - 1,
            DepartamentoId = 10,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 100,
            ValorTeto = 500,
            ApenasDepartamento = true
        };

        var service = new PlanejamentoCustoPreparacaoService(
            new PlanejamentoCustoValidador(),
            new PlanejamentoCustoMapping(),
            new PlanejamentoCustoRepositoryFake(planejamentoExistente),
            new DepartamentoRepositoryFake(new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" }),
            new CargoRepositoryFake([]));

        var resultado = await service.PrepararRemocaoAsync("PLC001");

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == PlanejamentoCustoResource.Erro_AnoPassadoNaoPodeSerExcluido);
    }

    [Fact]
    public async Task PrepararCriacaoAsync_DeveBloquearValorMinimoMaiorOuIgualAoTetoDoDepartamento()
    {
        var service = new PlanejamentoCustoPreparacaoService(
            new PlanejamentoCustoValidador(),
            new PlanejamentoCustoMapping(),
            new PlanejamentoCustoRepositoryFake(),
            new DepartamentoRepositoryFake(new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" }),
            new CargoRepositoryFake([]));

        var dto = new PlanejamentoCustoDTO
        {
            Codigo = "PLC001",
            Descricao = "Planejamento invalido",
            Ano = DateTime.Today.Year,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 500,
            ValorTeto = 500,
            ApenasDepartamento = true
        };

        var resultado = await service.PrepararCriacaoAsync(dto);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao.Contains("valor teto do planejamento de custo"));
    }

    [Fact]
    public async Task PrepararCriacaoAsync_DeveBloquearSomaDosTetosDosCargosMaiorQueTetoDoDepartamento()
    {
        var departamento = new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" };
        var cargos = new[]
        {
            new Cargo { Id = 20, Codigo = "CRG1", Descricao = "Cargo 1", DepartamentoId = 10, DepartamentoCodigo = "DPT" },
            new Cargo { Id = 21, Codigo = "CRG2", Descricao = "Cargo 2", DepartamentoId = 10, DepartamentoCodigo = "DPT" }
        };

        var service = new PlanejamentoCustoPreparacaoService(
            new PlanejamentoCustoValidador(),
            new PlanejamentoCustoMapping(),
            new PlanejamentoCustoRepositoryFake(),
            new DepartamentoRepositoryFake(departamento),
            new CargoRepositoryFake(cargos));

        var dto = new PlanejamentoCustoDTO
        {
            Codigo = "PLC001",
            Descricao = "Planejamento invalido",
            Ano = DateTime.Today.Year,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 100,
            ValorTeto = 500,
            ApenasDepartamento = false,
            DetalhesCargo =
            [
                new PlanejamentoCustoCargoDTO
                {
                    CargoCodigo = "CRG1",
                    Detalhado = true,
                    ValorMinimo = 80,
                    ValorTeto = 300
                },
                new PlanejamentoCustoCargoDTO
                {
                    CargoCodigo = "CRG2",
                    Detalhado = true,
                    ValorMinimo = 90,
                    ValorTeto = 250
                }
            ]
        };

        var resultado = await service.PrepararCriacaoAsync(dto);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == PlanejamentoCustoResource.Erro_SomaTetosUltrapassaDepartamento);
    }

    [Fact]
    public async Task PrepararCriacaoAsync_DeveBloquearCargosPendentesDeDecisao()
    {
        var departamento = new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" };
        var cargos = new[]
        {
            new Cargo { Id = 20, Codigo = "CRG1", Descricao = "Cargo 1", DepartamentoId = 10, DepartamentoCodigo = "DPT" },
            new Cargo { Id = 21, Codigo = "CRG2", Descricao = "Cargo 2", DepartamentoId = 10, DepartamentoCodigo = "DPT" }
        };

        var service = new PlanejamentoCustoPreparacaoService(
            new PlanejamentoCustoValidador(),
            new PlanejamentoCustoMapping(),
            new PlanejamentoCustoRepositoryFake(),
            new DepartamentoRepositoryFake(departamento),
            new CargoRepositoryFake(cargos));

        var dto = new PlanejamentoCustoDTO
        {
            Codigo = "PLC001",
            Descricao = "Planejamento incompleto",
            Ano = DateTime.Today.Year,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 100,
            ValorTeto = 500,
            ApenasDepartamento = false,
            DetalhesCargo =
            [
                new PlanejamentoCustoCargoDTO
                {
                    CargoCodigo = "CRG1",
                    Detalhado = true,
                    ValorMinimo = 80,
                    ValorTeto = 300
                }
            ]
        };

        var resultado = await service.PrepararCriacaoAsync(dto);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == string.Format(PlanejamentoCustoResource.Erro_CargosPendentes, "CRG2"));
    }

    [Fact]
    public async Task PrepararCriacaoAsync_DeveBloquearValorMinimoMaiorOuIgualAoTetoDoCargo()
    {
        var departamento = new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" };
        var cargos = new[]
        {
            new Cargo { Id = 20, Codigo = "CRG1", Descricao = "Cargo 1", DepartamentoId = 10, DepartamentoCodigo = "DPT" }
        };

        var service = new PlanejamentoCustoPreparacaoService(
            new PlanejamentoCustoValidador(),
            new PlanejamentoCustoMapping(),
            new PlanejamentoCustoRepositoryFake(),
            new DepartamentoRepositoryFake(departamento),
            new CargoRepositoryFake(cargos));

        var dto = new PlanejamentoCustoDTO
        {
            Codigo = "PLC001",
            Descricao = "Planejamento invalido",
            Ano = DateTime.Today.Year,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 100,
            ValorTeto = 500,
            ApenasDepartamento = false,
            DetalhesCargo =
            [
                new PlanejamentoCustoCargoDTO
                {
                    CargoCodigo = "CRG1",
                    Detalhado = true,
                    ValorMinimo = 300,
                    ValorTeto = 300
                }
            ]
        };

        var resultado = await service.PrepararCriacaoAsync(dto);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == string.Format(PlanejamentoCustoResource.Erro_ValorMinimoCargoMaiorTeto, "CRG1"));
    }

    [Fact]
    public async Task PrepararAtualizacaoAsync_DeveBloquearAlteracaoDoCodigoDoPlanejamento()
    {
        var planejamentoExistente = new PlanejamentoCusto
        {
            Id = 99,
            Codigo = "PLC001",
            Descricao = "Planejamento antigo",
            Ano = DateTime.Today.Year,
            DepartamentoId = 10,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 100,
            ValorTeto = 500,
            ApenasDepartamento = true
        };

        var service = new PlanejamentoCustoPreparacaoService(
            new PlanejamentoCustoValidador(),
            new PlanejamentoCustoMapping(),
            new PlanejamentoCustoRepositoryFake(planejamentoExistente),
            new DepartamentoRepositoryFake(new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" }),
            new CargoRepositoryFake([]));

        var dto = new PlanejamentoCustoDTO
        {
            Codigo = "PLC999",
            Descricao = "Planejamento atualizado",
            Ano = DateTime.Today.Year,
            DepartamentoCodigo = "DPT",
            ValorMinimo = 120,
            ValorTeto = 600,
            ApenasDepartamento = true
        };

        var resultado = await service.PrepararAtualizacaoAsync("PLC001", dto);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == PlanejamentoCustoResource.Erro_CodigoNaoPodeSerAlterado);
    }

}

using Application.DTO;
using Application.Mapping;
using Application.Policies.PlanejamentoCustos;
using Application.Resources;
using Application.UseCases.CargoCases;
using Application.UseCases.DepartamentoCases;
using Application.Validador;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Tracker.Tests.TestSupport.Fakes.PlanejamentoCustos;
using Xunit;

namespace Tracker.Tests.PlanejamentoCustos;

public class EstruturaPlanejadaPolicyTests
{
    [Fact]
    public async Task RemoverDepartamentoAsync_DeveBloquearDepartamentoComPlanejamentoAtualOuFuturo()
    {
        var departamento = new Departamento { Id = 10, Codigo = "DPT", Descricao = "Departamento" };
        var useCase = new ExcluirDepartamentoCase(
            new DepartamentoRepositoryFake(departamentos: [departamento]),
            new CargoRepositoryFake(),
            new EstruturaPlanejadaPolicy(new PlanejamentoCustoRepositoryFake(possuiDepartamentoPlanejado: true)),
            new UsuarioCargoDepartamentoRepositoryFake());

        var resultado = await useCase.ExecutarAsync("DPT");

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == string.Format(PlanejamentoCustoResource.Erro_DepartamentoComPlanejamento, "DPT"));
    }

    [Fact]
    public async Task RemoverCargoAsync_DeveBloquearCargoComPlanejamentoAtualOuFuturo()
    {
        var cargo = new Cargo
        {
            Id = 20,
            Codigo = "CRG",
            Descricao = "Cargo",
            DepartamentoId = 10,
            DepartamentoCodigo = "DPT"
        };
        var useCase = new ExcluirCargoCase(
            new CargoRepositoryFake(cargos: [cargo]),
            new EstruturaPlanejadaPolicy(new PlanejamentoCustoRepositoryFake(possuiCargoPlanejado: true)),
            new UsuarioCargoDepartamentoRepositoryFake());

        var resultado = await useCase.ExecutarAsync("CRG");

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == string.Format(PlanejamentoCustoResource.Erro_CargoComPlanejamentoRemocao, "CRG"));
    }

    [Fact]
    public async Task AtualizarCargoAsync_DeveBloquearMoverCargoPlanejadoParaOutroDepartamento()
    {
        var departamentoAtual = new Departamento { Id = 10, Codigo = "DPA", Descricao = "Departamento A" };
        var novoDepartamento = new Departamento { Id = 11, Codigo = "DPB", Descricao = "Departamento B" };
        var cargo = new Cargo
        {
            Id = 20,
            Codigo = "CRG",
            Descricao = "Cargo",
            DepartamentoId = departamentoAtual.Id,
            DepartamentoCodigo = departamentoAtual.Codigo,
            Departamento = departamentoAtual
        };

        var useCase = new AtualizarCargoCase(
            new CargoValidador(),
            new CargoMapping(),
            new CargoRepositoryFake(cargos: [cargo]),
            new DepartamentoRepositoryFake(departamentos: [departamentoAtual, novoDepartamento]),
            new EstruturaPlanejadaPolicy(new PlanejamentoCustoRepositoryFake(possuiCargoPlanejado: true)));

        var dto = new CargoDTO
        {
            Codigo = "CRG",
            Descricao = "Cargo atualizado",
            DepartamentoCodigo = "DPB"
        };

        var resultado = await useCase.ExecutarAsync("CRG", dto);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == string.Format(PlanejamentoCustoResource.Erro_CargoComPlanejamentoMovimentacao, "CRG"));
    }

    private sealed class UsuarioCargoDepartamentoRepositoryFake : IUsuarioCargoDepartamentoRepository
    {
        public Task<bool> GravarAssociacaoUsuarioCargoDepartamento(Usuario usuario, Cargo cargo, Departamento departamento)
            => Task.FromResult(true);

        public Task<UsuarioCargoDepartamento> ObterPorChaveDoUsuario(int usuarioId, string usuarioCodigo)
            => Task.FromResult<UsuarioCargoDepartamento>(null!);

        public Task<IEnumerable<UsuarioCargoDepartamento>> ObterPorCargo(int id, string codigo)
            => Task.FromResult<IEnumerable<UsuarioCargoDepartamento>>([]);

        public Task<IEnumerable<UsuarioCargoDepartamento>> ObterPorDepartamento(int id, string codigo)
            => Task.FromResult<IEnumerable<UsuarioCargoDepartamento>>([]);

        public Task<bool> RemoverAssociacaoUsuarioCargoDepartamento(UsuarioCargoDepartamento associacao)
            => Task.FromResult(true);
    }
}

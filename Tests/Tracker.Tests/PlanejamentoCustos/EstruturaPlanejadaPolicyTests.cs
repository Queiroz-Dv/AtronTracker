using Application.DTO;
using Application.Mapping;
using Application.Policies.PlanejamentoCustos;
using Application.Resources;
using Application.UseCases.CargoCases;
using Application.UseCases.DepartamentoCases;
using Application.Validador;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
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

    private sealed class PlanejamentoCustoRepositoryFake(
        bool possuiDepartamentoPlanejado = false,
        bool possuiCargoPlanejado = false) : IPlanejamentoCustoRepository
    {
        public Task<bool> AtualizarAsync(PlanejamentoCusto planejamentoCusto) => Task.FromResult(true);

        public Task<bool> CriarAsync(PlanejamentoCusto planejamentoCusto) => Task.FromResult(true);

        public Task<bool> ExisteCargoEmPlanejamentoAtualOuFuturoAsync(int cargoId, string cargoCodigo, int departamentoId, string departamentoCodigo, int anoMinimo)
            => Task.FromResult(possuiCargoPlanejado);

        public Task<bool> ExisteCodigoAsync(string codigo) => Task.FromResult(false);

        public Task<bool> ExisteDepartamentoEmPlanejamentoAtualOuFuturoAsync(int departamentoId, string departamentoCodigo, int anoMinimo)
            => Task.FromResult(possuiDepartamentoPlanejado);

        public Task<PlanejamentoCusto> ObterPorCodigoAsync(string codigo)
            => Task.FromResult<PlanejamentoCusto>(null!);

        public Task<PlanejamentoCusto> ObterPorCodigoAsNoTrackingAsync(string codigo)
            => Task.FromResult<PlanejamentoCusto>(null!);

        public Task<PlanejamentoCusto> ObterPorDepartamentoEAnoAsync(int departamentoId, string departamentoCodigo, int ano)
            => Task.FromResult<PlanejamentoCusto>(null!);

        public Task<IEnumerable<PlanejamentoCusto>> ObterPorAnoAsync(int ano)
            => Task.FromResult<IEnumerable<PlanejamentoCusto>>([]);

        public Task<IEnumerable<PlanejamentoCusto>> ObterTodosAsync()
            => Task.FromResult<IEnumerable<PlanejamentoCusto>>([]);

        public Task<bool> RemoverAsync(PlanejamentoCusto planejamentoCusto) => Task.FromResult(true);
    }

    private sealed class DepartamentoRepositoryFake(IReadOnlyCollection<Departamento>? departamentos = null) : IDepartamentoRepository
    {
        private readonly IReadOnlyCollection<Departamento> _departamentos = departamentos ?? [];

        public Task<bool> AtualizarDepartamentoRepositoryAsync(Departamento departamento) => Task.FromResult(true);

        public Task<bool> CriarDepartamentoRepositoryAsync(Departamento departamento) => Task.FromResult(true);

        public Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsync(string codigo)
            => Task.FromResult(_departamentos.SingleOrDefault(d => d.Codigo == codigo) ?? null!);

        public Task<Departamento> ObterDepartamentoPorCodigoRepository(string codigo)
            => Task.FromResult(_departamentos.SingleOrDefault(d => d.Codigo == codigo) ?? null!);

        public Task<Departamento> ObterDepartamentoPorIdRepositoryAsync(int? id)
            => Task.FromResult(_departamentos.SingleOrDefault(d => d.Id == id) ?? null!);

        public Task<IEnumerable<Departamento>> ObterDepartmentosAsync()
            => Task.FromResult<IEnumerable<Departamento>>(_departamentos);

        public Task<IEnumerable<Departamento>> ObterDepartamentosPorCodigoGestorAsync(string usuarioCodigo)
            => Task.FromResult<IEnumerable<Departamento>>([]);

        public Task<bool> RemoverDepartmentoRepositoryAsync(Departamento departamento) => Task.FromResult(true);
    }

    private sealed class CargoRepositoryFake(IReadOnlyCollection<Cargo>? cargos = null) : ICargoRepository
    {
        private readonly IReadOnlyCollection<Cargo> _cargos = cargos ?? [];

        public Task<bool> AtualizarRepositoryAsync(Cargo entity) => Task.FromResult(true);

        public Task<bool> AtualizarRepositoryAsync(int id, Cargo entity) => Task.FromResult(true);

        public Task<bool> AtualizarCargoAsync(Cargo cargo) => Task.FromResult(true);

        public Task<bool> CriarCargoAsync(Cargo cargo) => Task.FromResult(true);

        public Task<bool> CriarRepositoryAsync(Cargo entity) => Task.FromResult(true);

        public Task<Cargo> ObterCargoPorCodigoAsync(string codigo)
            => Task.FromResult(_cargos.SingleOrDefault(c => c.Codigo == codigo) ?? null!);

        public Task<Cargo> ObterCargoPorIdAsync(int? id)
            => Task.FromResult(_cargos.SingleOrDefault(c => c.Id == id) ?? null!);

        public Task<IEnumerable<Cargo>> ObterCargosAsync()
            => Task.FromResult<IEnumerable<Cargo>>(_cargos);

        public Task<IEnumerable<Cargo>> ObterCargosPorDepartamento(int departamentoId, string departamentoCodigo)
            => Task.FromResult<IEnumerable<Cargo>>(
                _cargos.Where(c => c.DepartamentoId == departamentoId && c.DepartamentoCodigo == departamentoCodigo));

        public Task<Cargo> ObterPorCodigoRepositoryAsync(string codigo)
            => Task.FromResult(_cargos.SingleOrDefault(c => c.Codigo == codigo) ?? null!);

        public Task<Cargo> ObterPorIdRepositoryAsync(int id)
            => Task.FromResult(_cargos.SingleOrDefault(c => c.Id == id) ?? null!);

        public Task<IEnumerable<Cargo>> ObterTodosRepositoryAsync()
            => Task.FromResult<IEnumerable<Cargo>>(_cargos);

        public Task<bool> RemoverCargoAsync(Cargo cargo) => Task.FromResult(true);

        public Task<bool> RemoverRepositoryAsync(Cargo entity) => Task.FromResult(true);
    }

    private sealed class UsuarioRepositoryFake : IUsuarioRepository
    {
        public Task<bool> AtualizarPreferenciaNotificacaoTarefaPorEmailAsync(string codigo, bool receberNotificacao)
            => Task.FromResult(true);

        public Task<bool> AtualizarPreferenciasNotificacaoTarefaAsync(string codigo, bool receberNotificacaoInterna, bool receberNotificacaoPorEmail)
            => Task.FromResult(true);

        public Task<bool> AtualizarRepositoryAsync(Usuario entity) => Task.FromResult(true);

        public Task<bool> AtualizarRepositoryAsync(int id, Usuario entity) => Task.FromResult(true);

        public Task<bool> AtualizarUsuarioAsync(Usuario usuario) => Task.FromResult(true);

        public Task<bool> CriarRepositoryAsync(Usuario entity) => Task.FromResult(true);

        public Task<bool> CriarUsuarioAsync(Usuario usuario) => Task.FromResult(true);

        public Task<bool> ConfirmarEmailAsync(string codigo) => Task.FromResult(true);

        public Task<Usuario> ObterInativoPorEmailAsync(string email)
            => Task.FromResult<Usuario>(null!);

        public Task<Usuario> ObterPorCodigoRepositoryAsync(string codigo)
            => Task.FromResult<Usuario>(null!);

        public Task<Usuario> ObterPorIdRepositoryAsync(int id)
            => Task.FromResult<Usuario>(null!);

        public Task<IEnumerable<Usuario>> ObterTodosRepositoryAsync()
            => Task.FromResult<IEnumerable<Usuario>>([]);

        public Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity()
            => Task.FromResult<List<UsuarioIdentity>>([]);

        public Task<Usuario> ObterUsuarioGeralPorCodigoAsync(string codigo)
            => Task.FromResult<Usuario>(null!);

        public Task<Usuario> ObterUsuarioGeralPorEmailAsync(string email)
            => Task.FromResult<Usuario>(null!);

        public Task<Usuario> ObterUsuarioPorCodigoAsync(string codigo)
            => Task.FromResult<Usuario>(null!);

        public Task<Usuario> ObterUsuarioPorIdAsync(int? id)
            => Task.FromResult<Usuario>(null!);

        public Task<IEnumerable<Usuario>> ObterUsuariosAsync()
            => Task.FromResult<IEnumerable<Usuario>>([]);

        public Task<bool> RemoverRepositoryAsync(Usuario entity) => Task.FromResult(true);

        public Task<bool> RemoverUsuarioAsync(Usuario usuario) => Task.FromResult(true);

        public Task<bool> VerificarEmailExistenteAsync(string email) => Task.FromResult(false);
    }

    private sealed class UsuarioCargoDepartamentoRepositoryFake : IUsuarioCargoDepartamentoRepository
    {
        public Task<bool> AtualizarRepositoryAsync(UsuarioCargoDepartamento entity) => Task.FromResult(true);

        public Task<bool> AtualizarRepositoryAsync(int id, UsuarioCargoDepartamento entity) => Task.FromResult(true);

        public Task<bool> CriarRepositoryAsync(UsuarioCargoDepartamento entity) => Task.FromResult(true);

        public Task<bool> GravarAssociacaoUsuarioCargoDepartamento(Usuario usuario, Cargo cargo, Departamento departamento)
            => Task.FromResult(true);

        public Task<UsuarioCargoDepartamento> ObterPorChaveDoUsuario(int usuarioId, string usuarioCodigo)
            => Task.FromResult<UsuarioCargoDepartamento>(null!);

        public Task<IEnumerable<UsuarioCargoDepartamento>> ObterPorCargo(int id, string codigo)
            => Task.FromResult<IEnumerable<UsuarioCargoDepartamento>>([]);

        public Task<IEnumerable<UsuarioCargoDepartamento>> ObterPorDepartamento(int id, string codigo)
            => Task.FromResult<IEnumerable<UsuarioCargoDepartamento>>([]);

        public Task<UsuarioCargoDepartamento> ObterPorCodigoRepositoryAsync(string codigo)
            => Task.FromResult<UsuarioCargoDepartamento>(null!);

        public Task<UsuarioCargoDepartamento> ObterPorIdRepositoryAsync(int id)
            => Task.FromResult<UsuarioCargoDepartamento>(null!);

        public Task<IEnumerable<UsuarioCargoDepartamento>> ObterTodosRepositoryAsync()
            => Task.FromResult<IEnumerable<UsuarioCargoDepartamento>>([]);

        public Task<bool> RemoverRepositoryAsync(UsuarioCargoDepartamento entity) => Task.FromResult(true);
    }
}

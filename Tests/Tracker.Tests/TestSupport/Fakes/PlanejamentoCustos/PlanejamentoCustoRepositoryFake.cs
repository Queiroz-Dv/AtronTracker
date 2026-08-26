using Domain.Entities;
using Domain.Interfaces;

namespace Tracker.Tests.TestSupport.Fakes.PlanejamentoCustos;

internal sealed class PlanejamentoCustoRepositoryFake : IPlanejamentoCustoRepository
{
    private readonly IReadOnlyCollection<PlanejamentoCusto> _planejamentos;
    private readonly bool _possuiDepartamentoPlanejado;
    private readonly bool _possuiCargoPlanejado;

    public PlanejamentoCustoRepositoryFake(PlanejamentoCusto planejamento)
        : this([planejamento])
    {
    }

    public PlanejamentoCustoRepositoryFake(
        IReadOnlyCollection<PlanejamentoCusto>? planejamentos = null,
        bool possuiDepartamentoPlanejado = false,
        bool possuiCargoPlanejado = false)
    {
        _planejamentos = planejamentos ?? [];
        _possuiDepartamentoPlanejado = possuiDepartamentoPlanejado;
        _possuiCargoPlanejado = possuiCargoPlanejado;
    }

    public Task<bool> AtualizarAsync(PlanejamentoCusto planejamentoCusto) => Task.FromResult(true);

    public Task<bool> CriarAsync(PlanejamentoCusto planejamentoCusto) => Task.FromResult(true);

    public Task<bool> ExisteCargoEmPlanejamentoAtualOuFuturoAsync(
        int cargoId,
        string cargoCodigo,
        int departamentoId,
        string departamentoCodigo,
        int anoMinimo)
        => Task.FromResult(_possuiCargoPlanejado);

    public Task<bool> ExisteCodigoAsync(string codigo) => Task.FromResult(false);

    public Task<bool> ExisteDepartamentoEmPlanejamentoAtualOuFuturoAsync(
        int departamentoId,
        string departamentoCodigo,
        int anoMinimo)
        => Task.FromResult(_possuiDepartamentoPlanejado);

    public Task<PlanejamentoCusto> ObterPorCodigoAsync(string codigo)
        => Task.FromResult(_planejamentos.SingleOrDefault(planejamento => planejamento.Codigo == codigo) ?? null!);

    public Task<PlanejamentoCusto> ObterPorCodigoAsNoTrackingAsync(string codigo)
        => Task.FromResult(_planejamentos.SingleOrDefault(planejamento => planejamento.Codigo == codigo) ?? null!);

    public Task<PlanejamentoCusto> ObterPorDepartamentoEAnoAsync(
        int departamentoId,
        string departamentoCodigo,
        int ano)
        => Task.FromResult(_planejamentos.SingleOrDefault(planejamento =>
            planejamento.DepartamentoId == departamentoId &&
            planejamento.DepartamentoCodigo == departamentoCodigo &&
            planejamento.Ano == ano) ?? null!);

    public Task<IEnumerable<PlanejamentoCusto>> ObterPorAnoAsync(int ano)
        => Task.FromResult<IEnumerable<PlanejamentoCusto>>(
            _planejamentos.Where(planejamento => planejamento.Ano == ano));

    public Task<IEnumerable<PlanejamentoCusto>> ObterTodosAsync()
        => Task.FromResult<IEnumerable<PlanejamentoCusto>>(_planejamentos);

    public Task<bool> RemoverAsync(PlanejamentoCusto planejamentoCusto) => Task.FromResult(true);
}

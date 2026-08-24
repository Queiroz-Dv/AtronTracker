using Domain.Entities;
using Domain.Interfaces;

namespace Tracker.Tests.TestSupport.Fakes.PlanejamentoCustos;

internal sealed class CargoRepositoryFake(
    IReadOnlyCollection<Cargo>? cargos = null) : ICargoRepository
{
    private readonly IReadOnlyCollection<Cargo> _cargos = cargos ?? [];

    public Task<bool> AtualizarCargoAsync(Cargo cargo) => Task.FromResult(true);

    public Task<bool> CriarCargoAsync(Cargo cargo) => Task.FromResult(true);

    public Task<Cargo> ObterCargoPorCodigoAsync(string codigo)
        => Task.FromResult(_cargos.SingleOrDefault(cargo => cargo.Codigo == codigo) ?? null!);

    public Task<Cargo> ObterCargoPorIdAsync(int? id)
        => Task.FromResult(_cargos.SingleOrDefault(cargo => cargo.Id == id) ?? null!);

    public Task<IEnumerable<Cargo>> ObterCargosAsync()
        => Task.FromResult<IEnumerable<Cargo>>(_cargos);

    public Task<IEnumerable<Cargo>> ObterCargosPorDepartamento(int departamentoId, string departamentoCodigo)
        => Task.FromResult<IEnumerable<Cargo>>(_cargos.Where(cargo =>
            cargo.DepartamentoId == departamentoId &&
            cargo.DepartamentoCodigo == departamentoCodigo));

    public Task<bool> RemoverCargoAsync(Cargo cargo) => Task.FromResult(true);
}

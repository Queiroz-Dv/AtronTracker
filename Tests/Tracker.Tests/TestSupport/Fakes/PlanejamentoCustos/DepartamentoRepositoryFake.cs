using Domain.Entities;
using Domain.Interfaces;
using Domain.Tenants;

namespace Tracker.Tests.TestSupport.Fakes.PlanejamentoCustos;

internal sealed class DepartamentoRepositoryFake : IDepartamentoRepository
{
    private readonly IReadOnlyCollection<Departamento> _departamentos;

    public DepartamentoRepositoryFake(Departamento departamento)
        : this([departamento])
    {
    }

    public DepartamentoRepositoryFake(IReadOnlyCollection<Departamento>? departamentos = null)
    {
        _departamentos = departamentos ?? [];
    }

    public Task<bool> AtualizarDepartamentoRepositoryAsync(Departamento departamento) => Task.FromResult(true);

    public Task<bool> CriarDepartamentoRepositoryAsync(Departamento departamento) => Task.FromResult(true);

    public Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsync(string codigo)
        => Task.FromResult(_departamentos.SingleOrDefault(departamento => departamento.Codigo == codigo) ?? null!);

    public Task<Departamento> ObterDepartamentoPorCodigoRepository(string codigo)
        => Task.FromResult(_departamentos.SingleOrDefault(departamento => departamento.Codigo == codigo) ?? null!);

    public Task<Departamento> ObterDepartamentoPorIdRepositoryAsync(int? id)
        => Task.FromResult(_departamentos.SingleOrDefault(departamento => departamento.Id == id) ?? null!);

    public Task<IEnumerable<Departamento>> ObterDepartmentosAsync()
        => Task.FromResult<IEnumerable<Departamento>>(_departamentos);

    public Task<IEnumerable<Departamento>> ObterDepartamentosPorCodigoGestorAsync(string usuarioCodigo)
        => Task.FromResult<IEnumerable<Departamento>>([]);

    public Task<bool> RemoverDepartmentoRepositoryAsync(Departamento departamento) => Task.FromResult(true);

    public Task<bool> CriarTenant(DepartamentoWorkspace departamentoWorkspace)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoverTenant(DepartamentoWorkspace departamentoWorkspace)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Departamento>> ObterDepartmentosAsync(string workspaceCodigo, int workspaceId)
    {
        throw new NotImplementedException();
    }

    public Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsync(string codigo, string worksapceCodigo, int workspaceId)
    {
        throw new NotImplementedException();
    }
}

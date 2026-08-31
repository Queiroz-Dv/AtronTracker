#nullable enable
using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class EmpresaRepository(AtronDbContext context) : IEmpresaRepository
    {
        public async Task<IReadOnlyList<Empresa>> ObterTodosAsync()
            => await context.Empresas
                .AsNoTracking()
                .OrderBy(empresa => empresa.NomeFantasia)
                .ToListAsync();

        public Task<Empresa?> ObterPorCodigoAsync(string codigo, bool rastrear = false)
        {
            var consulta = rastrear
                ? context.Empresas.AsQueryable()
                : context.Empresas.AsNoTracking();

            return consulta.SingleOrDefaultAsync(empresa => empresa.Codigo == codigo);
        }

        public Task<bool> CodigoExisteAsync(string codigo, int? empresaIdIgnorada = null)
            => context.Empresas.AnyAsync(empresa =>
                empresa.Codigo == codigo
                && (!empresaIdIgnorada.HasValue || empresa.Id != empresaIdIgnorada.Value));

        public async Task<bool> CriarAsync(Empresa empresa)
        {
            context.Empresas.Add(empresa);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AtualizarAsync(Empresa empresa)
            => await context.SaveChangesAsync() > 0;

        public async Task<bool> RemoverAsync(Empresa empresa)
        {
            context.Empresas.Remove(empresa);
            return await context.SaveChangesAsync() > 0;
        }
    }
}

using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PlanejamentoCustoRepository : IPlanejamentoCustoRepository
    {
        private readonly AtronDbContext _context;

        public PlanejamentoCustoRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AtualizarAsync(PlanejamentoCusto planejamentoCusto)
        {
            var atualizado = await _context.SaveChangesAsync();
            return atualizado > 0;
        }

        public async Task<bool> CriarAsync(PlanejamentoCusto planejamentoCusto)
        {
            await _context.PlanejamentosCusto.AddAsync(planejamentoCusto);
            var gravado = await _context.SaveChangesAsync();
            return gravado > 0;
        }

        public async Task<bool> ExisteCodigoAsync(string codigo)
        {
            return await _context.PlanejamentosCusto.AnyAsync(plc => plc.Codigo == codigo);
        }

        public async Task<bool> ExisteDepartamentoEmPlanejamentoAtualOuFuturoAsync(int departamentoId, string departamentoCodigo, int anoMinimo)
        {
            return await _context.PlanejamentosCusto
                .AsNoTracking()
                .AnyAsync(plc =>
                    plc.Ano >= anoMinimo &&
                    plc.DepartamentoId == departamentoId &&
                    plc.DepartamentoCodigo == departamentoCodigo);
        }

        public async Task<bool> ExisteCargoEmPlanejamentoAtualOuFuturoAsync(
            int cargoId,
            string cargoCodigo,
            int departamentoId,
            string departamentoCodigo,
            int anoMinimo)
        {
            return await _context.PlanejamentosCusto
                .AsNoTracking()
                .AnyAsync(plc =>
                    plc.Ano >= anoMinimo &&
                    (
                        plc.DetalhesCargo.Any(detalhe =>
                            detalhe.CargoId == cargoId &&
                            detalhe.CargoCodigo == cargoCodigo)
                        ||
                        (!plc.ApenasDepartamento &&
                         plc.DepartamentoId == departamentoId &&
                         plc.DepartamentoCodigo == departamentoCodigo)
                    ));
        }

        public async Task<PlanejamentoCusto> ObterPorCodigoAsync(string codigo)
        {
            return await _context.PlanejamentosCusto
                .Include(plc => plc.Departamento)
                .Include(plc => plc.DetalhesCargo)
                    .ThenInclude(detalhe => detalhe.Cargo)
                .FirstOrDefaultAsync(plc => plc.Codigo == codigo);
        }

        public async Task<PlanejamentoCusto> ObterPorCodigoAsNoTrackingAsync(string codigo)
        {
            return await _context.PlanejamentosCusto
                .Include(plc => plc.Departamento)
                .Include(plc => plc.DetalhesCargo)
                    .ThenInclude(detalhe => detalhe.Cargo)
                .AsNoTracking()
                .FirstOrDefaultAsync(plc => plc.Codigo == codigo);
        }

        public async Task<PlanejamentoCusto> ObterPorDepartamentoEAnoAsync(int departamentoId, string departamentoCodigo, int ano)
        {
            return await _context.PlanejamentosCusto
                .AsNoTracking()
                .FirstOrDefaultAsync(plc =>
                    plc.DepartamentoId == departamentoId &&
                    plc.DepartamentoCodigo == departamentoCodigo &&
                    plc.Ano == ano);
        }

        public async Task<IEnumerable<PlanejamentoCusto>> ObterPorAnoAsync(int ano)
        {
            return await _context.PlanejamentosCusto
                .Include(plc => plc.Departamento)
                .Include(plc => plc.DetalhesCargo)
                    .ThenInclude(detalhe => detalhe.Cargo)
                .AsNoTracking()
                .Where(plc => plc.Ano == ano)
                .OrderByDescending(plc => plc.Ano)
                .ThenBy(plc => plc.DepartamentoCodigo)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlanejamentoCusto>> ObterTodosAsync()
        {
            return await _context.PlanejamentosCusto
                .Include(plc => plc.Departamento)
                .Include(plc => plc.DetalhesCargo)
                    .ThenInclude(detalhe => detalhe.Cargo)
                .AsNoTracking()
                .OrderByDescending(plc => plc.Ano)
                .ThenBy(plc => plc.DepartamentoCodigo)
                .ToListAsync();
        }

        public async Task<bool> RemoverAsync(PlanejamentoCusto planejamentoCusto)
        {
            _context.PlanejamentosCusto.Remove(planejamentoCusto);
            var removido = await _context.SaveChangesAsync();
            return removido > 0;
        }
    }
}

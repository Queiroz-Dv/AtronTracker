using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private AtronDbContext _context;

        public DepartamentoRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AtualizarDepartamentoRepositoryAsync(Departamento departamento)
        {
            var atualizado = await _context.SaveChangesAsync();
            return atualizado > 0;
        }

        public async Task<bool> CriarDepartamentoRepositoryAsync(Departamento departamento)
        {
            await _context.AddAsync(departamento);
            var gravado = await _context.SaveChangesAsync();
            return gravado > 0;
        }

        public async Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsync(string codigo)
        {
            return await _context.Departamentos.FirstOrDefaultAsync(dpt => dpt.Codigo == codigo);
        }

        public async Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(string codigo)
        {
            return await _context.Departamentos.AsNoTracking().FirstOrDefaultAsync(dpt => dpt.Codigo == codigo);
        }

        public async Task<Departamento> ObterDepartamentoPorIdRepositoryAsync(int? id)
        {
            return await _context.Departamentos.AsNoTracking().FirstOrDefaultAsync(dpt => dpt.Id == id);
        }

        public async Task<IEnumerable<Departamento>> ObterDepartmentosAsync()
        {
            return await _context.Departamentos.OrderByDescending(order => order.Codigo).ToListAsync();
        }

        public async Task<bool> RemoverDepartmentoRepositoryAsync(Departamento departamento)
        {
            _context.Remove(departamento);
            var removido = await _context.SaveChangesAsync();
            return removido > 0;
        }
    }
}
using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TarefaEstadoRepository : ITarefaEstadoRepository
    {
        private readonly AtronDbContext _context;

        public TarefaEstadoRepository(AtronDbContext context)
        {
            _context = context;
        }

        public Task<List<TarefaEstado>> ObterTodosAsync()
        {
            return _context.Set<TarefaEstado>().AsNoTracking().ToListAsync();
        }

        public async Task<TarefaEstado> ObterPorIdAsync(int id)
        {
            return (await _context.Set<TarefaEstado>().AsNoTracking().FirstOrDefaultAsync(estado => estado.Id == id))!;
        }
    }
}

using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class TarefaEstadoRepository : ITarefaEstadoRepository
    {
        private readonly AtronDbContext _context;

        public TarefaEstadoRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<List<TarefaEstado>> ObterTodosAsync()
        {
            var tarefasEstado = await _context.TarefaEstados.AsNoTracking().ToListAsync();
            return tarefasEstado;
        }
    }
}

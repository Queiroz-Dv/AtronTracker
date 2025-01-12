using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class TarefaEstadoRepository : Repository<TarefaEstado>, ITarefaEstadoRepository
    {
        private readonly AtronDbContext _context;
        private readonly IRepository<TarefaEstado> _repository;

        public TarefaEstadoRepository(AtronDbContext context, IRepository<TarefaEstado> repository) : base(context)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<List<TarefaEstado>> ObterTodosAsync()
        {
            var tarefasEstado = await _context.TarefaEstados.AsNoTracking().ToListAsync();
            return tarefasEstado;
        }
    }
}

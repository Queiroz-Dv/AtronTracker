using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class TarefaRepository : Repository<Tarefa>, ITarefaRepository
    {
        private readonly AtronDbContext _context;

        public TarefaRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<Tarefa>> ObterTodasTarefasComEstado()
        {
            var entidades = _context.Tarefas.Include(tre => tre.TarefaEstado).AsNoTracking().ToListAsync();
            return entidades;
        }
    }
}
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;

namespace Atron.Infrastructure.Repositories
{
    public class TarefaRepository : Repository<Tarefa>, ITarefaRepository
    {
        AtronDbContext _context;

        public TarefaRepository(AtronDbContext context) : base(context) { }
    }
}
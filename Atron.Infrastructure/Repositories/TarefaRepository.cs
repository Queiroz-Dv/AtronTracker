using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class TarefaRepository : ITarefaRepository
    {
        AtronDbContext _context;

        public TarefaRepository(AtronDbContext context)
        {
            _context = context;
        }

        public Task<Tarefa> AtualizarTarefaAsync(Tarefa tarefa)
        {
            throw new NotImplementedException();
        }

        public async Task<Tarefa> CriarTarefaAsync(Tarefa tarefa)
        {
            await _context.Tarefas.AddAsync(tarefa);
            await _context.SaveChangesAsync();
            return new Tarefa();
        }

        public Task<Tarefa> ObterTarefaPorIdAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Tarefa>> ObterTarefasAsync()
        {
            var tarefas = await _context.Tarefas.AsNoTracking().OrderByDescending(trf => trf.DataInicial).ToListAsync();
            return tarefas;
        }

        public Task<Tarefa> RemoverTarefaAsync(Tarefa tarefa)
        {
            throw new NotImplementedException();
        }
    }
}

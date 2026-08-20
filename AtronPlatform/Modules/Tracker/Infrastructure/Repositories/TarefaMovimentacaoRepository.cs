using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TarefaMovimentacaoRepository(AtronDbContext context) : ITarefaMovimentacaoRepository
    {
        private readonly AtronDbContext _context = context;

        public async Task<bool> RegistrarAsync(TarefaMovimentacao movimentacao)
        {
            await _context.TarefaMovimentacoes.AddAsync(movimentacao);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<TarefaMovimentacao>> ObterMovimentacoesPorIdAsync(int tarefaId)
        {
            var query = _context.TarefaMovimentacoes
                .AsNoTracking()
                .Where(movimentacao => movimentacao.TarefaId == tarefaId);

            var itens = await query
                .OrderByDescending(movimentacao => movimentacao.DataOcorrencia)
                .ThenByDescending(movimentacao => movimentacao.Id)
                .ToListAsync();

            return itens;
        }
    }
}
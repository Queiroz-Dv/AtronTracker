using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Queries;
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

        public async Task<TarefaMovimentacaoPagina> ObterPaginaAsync(TarefaMovimentacaoConsulta consulta)
        {
            var query = _context.TarefaMovimentacoes
                .AsNoTracking()
                .Where(movimentacao => movimentacao.TarefaId == consulta.TarefaId);

            var totalItens = await query.CountAsync();
            var itens = await query
                .OrderByDescending(movimentacao => movimentacao.DataOcorrencia)
                .ThenByDescending(movimentacao => movimentacao.Id)
                .Skip((consulta.Pagina - 1) * consulta.TamanhoPagina)
                .Take(consulta.TamanhoPagina)
                .ToListAsync();

            return new TarefaMovimentacaoPagina(itens, totalItens);
        }
    }
}

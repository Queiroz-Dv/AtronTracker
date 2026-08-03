using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using AtronStock.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AtronStock.Infrastructure.Repositories
{
    public class EstoqueRepository : IEstoqueRepository
    {
        private readonly StockDbContext _context;

        public EstoqueRepository(StockDbContext context)
        {
            _context = context;
        }

        public async Task<Estoque?> ObterPorProdutoIdAsync(int produtoId)
        {
            return await _context.Estoques
                .FirstOrDefaultAsync(e => e.ProdutoId == produtoId);
        }

        public async Task AdicionarAsync(Estoque estoque)
        {
            await _context.Estoques.AddAsync(estoque);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Estoque estoque)
        {
            _context.Estoques.Update(estoque);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarMovimentacaoAsync(MovimentacaoEstoque movimentacao)
        {
            await _context.MovimentacoesEstoque.AddAsync(movimentacao);
            await _context.SaveChangesAsync();
        }

        public async Task RegistrarEntradaCompletaAsync(Entrada entrada)
        {
            await _context.Entradas.AddAsync(entrada);
            await _context.SaveChangesAsync();
        }

        public async Task RegistrarVendaCompletaAsync(Venda venda)
        {
            await _context.Vendas.AddAsync(venda);
            await _context.SaveChangesAsync();
        }
    }
}

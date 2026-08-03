using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using AtronStock.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AtronStock.Infrastructure.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly StockDbContext _context;

        public ProdutoRepository(StockDbContext context)
        {
            _context = context;
        }

        public async Task<Produto?> ObterPorIdAsync(int id)
        {
            return await _context.Produtos.FindAsync(id);
        }

        public async Task<bool> AdicionarAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            var salvo = await _context.SaveChangesAsync();
            return salvo > 0;
        }

        public async Task AtualizarAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<Produto>> ObterTodos()
        {
            return await _context.Produtos.ToListAsync();
        }

        public Task<Produto> ObterPorCodigoAsync(string codigo)
        {
            return _context.Produtos.FirstOrDefaultAsync(p => p.Codigo == codigo);
        }
    }
}

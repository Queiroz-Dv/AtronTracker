#nullable enable

using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using AtronStock.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AtronStock.Infrastructure.Repositories
{
    public sealed class ProdutoRepository(StockDbContext context) : IProdutoRepository
    {
        private readonly StockDbContext _context = context;

        public Task<Produto?> ObterPorIdAsync(int id)
            => ConsultaCompleta().FirstOrDefaultAsync(produto => produto.Id == id);

        public Task<Produto?> ObterPorCodigoAsync(string codigo)
            => ConsultaCompleta().FirstOrDefaultAsync(produto => produto.Codigo == codigo);

        public async Task<ICollection<Produto>> ObterTodosAsync()
            => await ConsultaCompleta()
                .AsNoTracking()
                .OrderBy(produto => produto.Codigo)
                .ToListAsync();

        public async Task<bool> AdicionarAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AtualizarAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            return await _context.SaveChangesAsync() > 0;
        }

        private IQueryable<Produto> ConsultaCompleta()
            => _context.Produtos
                .Include(produto => produto.Categorias)
                    .ThenInclude(relacionamento => relacionamento.Categoria)
                .Include(produto => produto.LoteProduto);
    }
}

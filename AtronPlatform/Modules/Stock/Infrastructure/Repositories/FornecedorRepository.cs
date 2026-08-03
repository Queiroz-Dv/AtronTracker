using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using AtronStock.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AtronStock.Infrastructure.Repositories
{
    public class FornecedorRepository : IFornecedorRepository
    {
        private readonly StockDbContext _context;

        public FornecedorRepository(StockDbContext context)
        {
            _context = context;
        }

        public async Task<Fornecedor?> ObterPorIdAsync(int id)
        {
            return await _context.Fornecedores.FindAsync(id);
        }

        public async Task<bool> AdicionarAsync(Fornecedor fornecedor)
        {
            await _context.Fornecedores.AddAsync(fornecedor);
            var salvo = await _context.SaveChangesAsync();
            return salvo > 0;
        }

        public async Task<ICollection<Fornecedor>> ListarTodosAsync()
        {
            return await _context.Fornecedores.ToListAsync();
        }

        public async Task<Fornecedor> ObterPorCodigoAsync(string codigo)
        {
            return await _context.Fornecedores
                .FirstOrDefaultAsync(f => f.Codigo == codigo);
        }
    }
}

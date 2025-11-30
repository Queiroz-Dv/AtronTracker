using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums; 
using AtronStock.Domain.Interfaces;
using AtronStock.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AtronStock.Infrastructure.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly StockDbContext _context;

        public CategoriaRepository(StockDbContext ctx)
        {
            _context = ctx;
        }

        public async Task<bool> AtualizarCategoriaAsync(Categoria categoria)
        {           
            var atualizado = await _context.SaveChangesAsync();
            return atualizado > 0;
        }

        public async Task<bool> CriarCategoriaAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            var salvo = await _context.SaveChangesAsync();
            return salvo > 0;
        }

        public async Task<Categoria> ObterCategoriaPorCodigoAsync(string codigo)
        {          
            return await _context.Categorias.FirstOrDefaultAsync(c => c.Codigo == codigo);
        }

        public async Task<ICollection<Categoria>> ObterTodasCategoriasAsync()
        {
            return await _context.Categorias.ToListAsync();
        }

        public async Task<ICollection<Categoria>> ObterTodasCategoriasInativasAsync()
        {            
            return await _context.Categorias
                                 .Where(c => c.Status == EStatus.Inativo)
                                 .ToListAsync();
        }
    }
}
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using AtronStock.Domain.Interfaces;
using AtronStock.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AtronStock.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly StockDbContext _context;

        public ClienteRepository(StockDbContext ctx)
        {
            _context = ctx;
        }

        public async Task<bool> AtualizarClienteAsync(Cliente cliente)
        {
            var atualizado = await _context.SaveChangesAsync();
            return atualizado > 0;
        }

        public async Task<bool> CriarClienteAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            var salvo = await _context.SaveChangesAsync();
            return salvo > 0;
        }

        public async Task<Cliente> ObterClientePorCodigoAsync(string codigo)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Codigo == codigo);
        }

        public async Task<ICollection<Cliente>> ObterTodoClientesAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<ICollection<Cliente>> ObterTodoClientesInativosAsync()
        {
            return await _context.Clientes.Where(c => c.Status == EStatus.Inativo).ToListAsync();
        }        
    }
}
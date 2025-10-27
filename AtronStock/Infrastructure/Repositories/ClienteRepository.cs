using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using AtronStock.Infrastructure.Context;

namespace AtronStock.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly StockDbContext _context;
        public ClienteRepository(StockDbContext ctx)
        {
            _context = ctx;
        }

        public async Task CriarCliente(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }
    }
}

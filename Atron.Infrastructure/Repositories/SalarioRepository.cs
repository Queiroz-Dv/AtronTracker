using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class SalarioRepository : ISalarioRepository
    {
        private AtronDbContext _context;

        public SalarioRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<Salario> CriarSalarioAsync(Salario salario)
        {
            await _context.Salarios.AddAsync(salario);
            await _context.SaveChangesAsync();
            return salario;
        }

        public async Task<IEnumerable<Salario>> ObterSalariosRepositoryAsync()
        {
            var salarios = await _context.Salarios.AsNoTracking().ToListAsync();
            return salarios;
        }
    }
}
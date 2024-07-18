using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class SalarioRepository : Repository<Salario>, ISalarioRepository
    {
        private AtronDbContext _context;

        public SalarioRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Salario> ObterSalarioPorCodigoUsuario(string codigoUsuario)
        {
            var salario = await _context.Salarios.FirstOrDefaultAsync(slr => slr.UsuarioCodigo == codigoUsuario);
            return salario;
        }
    }
}
using Domain.Componentes;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PropriedadeDeFluxoRepository : IPropriedadeDeFluxoRepository
    {
        private readonly AtronDbContext _context;

        public PropriedadeDeFluxoRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<PropriedadesDeFluxo> ObterPropriedadePorCodigo(string codigo)
        {
            return await _context.PropriedadesDeFluxo.FirstOrDefaultAsync(pdf => pdf.Codigo == codigo);
        }

        public async Task<PropriedadesDeFluxo> ObterPropriedadePorId(int id)
        {
            return await _context.PropriedadesDeFluxo.FirstOrDefaultAsync(pdf => pdf.Id == id);
        }

        public async Task<IEnumerable<PropriedadesDeFluxo>> ObterTodasPropriedades()
        {
            return await _context.PropriedadesDeFluxo.ToListAsync();
        }
    }
}

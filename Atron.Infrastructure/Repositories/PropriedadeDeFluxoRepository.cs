using Atron.Domain.Componentes;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Interfaces;
using Shared.Interfaces.Accessor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class PropriedadeDeFluxoRepository : Repository<PropriedadesDeFluxo>, IPropriedadeDeFluxoRepository
    {
        public PropriedadeDeFluxoRepository(ILiteFacade liteFacade, IServiceAccessor serviceAccessor) : base(liteFacade, serviceAccessor)
        {
        }

        public async Task<PropriedadesDeFluxo> ObterPropriedadePorCodigo(string codigo)
        {
            //return await _context.PropriedadesDeFluxo.FirstOrDefaultAsync(pdf => pdf.Codigo == codigo);
            throw new NotImplementedException();
        }

        public async Task<PropriedadesDeFluxo> ObterPropriedadePorId(int id)
        {
            //return await _context.PropriedadesDeFluxo.FirstOrDefaultAsync(pdf => pdf.Id == id);
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PropriedadesDeFluxo>> ObterTodasPropriedades()
        {
            //return await _context.PropriedadesDeFluxo.ToListAsync();
            throw new NotImplementedException();
        }
    }
}

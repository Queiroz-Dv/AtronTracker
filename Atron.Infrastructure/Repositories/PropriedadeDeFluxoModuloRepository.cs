using Atron.Domain.Componentes;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Interfaces;
using Shared.Interfaces.Accessor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class PropriedadeDeFluxoModuloRepository : Repository<PropriedadeDeFluxoModulo>, IPropriedadeDeFluxoModuloRepository
    {
        public PropriedadeDeFluxoModuloRepository(ILiteFacade liteFacade, IServiceAccessor serviceAccessor) : base(liteFacade, serviceAccessor)
        {
        }

        public async Task<bool> GravarAsync(PropriedadeDeFluxoModulo entidade)
        {
            throw new NotImplementedException();
            //try
            //{
            //    await _context.PropriedadeDeFluxoModulo.AddAsync(entidade);
            //    var result = await _context.SaveChangesAsync();
            //    return result > 0;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        public Task<List<PropriedadeDeFluxoModulo>> ObterRelacionamentosTodosAsync()
        {
            throw new NotImplementedException();
        }
    }
}

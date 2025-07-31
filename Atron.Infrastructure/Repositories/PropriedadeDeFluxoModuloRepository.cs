using Atron.Domain.Componentes;
using Atron.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class PropriedadeDeFluxoModuloRepository : IPropriedadeDeFluxoModuloRepository
    {

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

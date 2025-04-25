using Atron.Domain.Componentes;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class PropriedadeDeFluxoModuloRepository : IPropriedadeDeFluxoModuloRepository
    {
        private readonly AtronDbContext _context;

        public PropriedadeDeFluxoModuloRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<bool> GravarAsync(PropriedadeDeFluxoModulo entidade)
        {
            try
            {
                await _context.PropriedadeDeFluxoModulo.AddAsync(entidade);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<List<PropriedadeDeFluxoModulo>> ObterRelacionamentosTodosAsync()
        {
            throw new NotImplementedException();
        }
    }
}

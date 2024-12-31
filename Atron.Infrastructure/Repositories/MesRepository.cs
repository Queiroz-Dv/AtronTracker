using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class MesRepository : IMesRepository
    {
        private readonly AtronDbContext _context;

        public MesRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Mes>> ObterMesesRepositoryAsync()
        {

            try
            {
                var meses = await _context.Meses.ToListAsync();

                return meses;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
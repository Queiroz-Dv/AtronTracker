using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class ModuloRepository : IModuloRepository
    {
        private ILiteDbContext context;

        public ModuloRepository(ILiteDbContext context)
        {
            this.context = context;
        }

        public async Task<Modulo> ObterPorCodigoRepository(string codigo)
        {
            return await context.Modulos.FindOneAsync(mdl => mdl.Codigo == codigo);
        }

        public async Task<Modulo> ObterPorIdRepository(int id)
        {
            return await context.Modulos.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Modulo>> ObterTodosRepository()
        {
            return await context.Modulos.FindAllAsync();
        }
    }
}
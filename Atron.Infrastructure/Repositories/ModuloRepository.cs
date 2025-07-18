using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Shared.Interfaces.Accessor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class ModuloRepository : Repository<Modulo>, IModuloRepository
    {
        private IDataSet<Modulo> Modulos => _facade.LiteDbContext.Modulos;

        public ModuloRepository(ILiteFacade liteFacade, IServiceAccessor serviceAccessor) : base(liteFacade, serviceAccessor)
        { }

        public async Task<Modulo> ObterPorCodigoRepository(string codigo)
        {
            return await Modulos.FindOneAsync(mdl => mdl.Codigo == codigo);
        }

        public async Task<Modulo> ObterPorIdRepository(int id)
        {
            return await Modulos.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Modulo>> ObterTodosRepository()
        {
            return await Modulos.FindAllAsync();
        }
    }
}
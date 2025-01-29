using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;

namespace Atron.Infrastructure.Repositories
{
    public class MesRepository : Repository<Mes>, IMesRepository
    {
        public MesRepository(AtronDbContext context) : base(context)
        { }
    }
}
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class CargoRepository : ICargoRepository
    {
        AtronDbContext context;

        public CargoRepository(AtronDbContext context)
        {
            this.context = context;
        }

        public async Task<Cargo> CriarCargoAsync(Cargo cargo)
        {
            await context.Cargos.AddAsync(cargo);
            await context.SaveChangesAsync();
            return cargo;
        }

        public async Task<Cargo> ObterCargoPorIdAsync(int? id)
        {
            var cargo = await context.Cargos.FindAsync(id);
            return cargo;
        }

        public async Task<Cargo> ObterCargoComDepartamentoPorIdAsync(int? id)
        {
            var cargos = await (from pst in context.Cargos
                                join dept in context.Departamentos on pst.DepartmentoId equals dept.Id
                                where dept.Id == id
                                select new Cargo
                                {
                                    Descricao = pst.Descricao,
                                    Departmento = dept,
                                    DepartmentoId = dept.Id
                                }).FirstOrDefaultAsync();


            return cargos;
        }

        public async Task<IEnumerable<Cargo>> ObterCargosAsync()
        {
            var cargos = await context.Cargos.AsNoTracking().ToListAsync();
            return cargos;
        }

        public async Task<Cargo> RemoverCargoAsync(Cargo cargo)
        {
            context.Cargos.Remove(cargo);
            await context.SaveChangesAsync();
            return cargo;
        }

        public async Task<Cargo> AtualizarCargoAsync(Cargo cargo)
        {
            context.Cargos.Update(cargo);
            await context.SaveChangesAsync();
            return cargo;
        }

        public async Task<Cargo> ObterCargoPorCodigoAsync(string codigo)
        {
            var cargo = await context.Cargos.AsNoTracking().FirstOrDefaultAsync(crg => crg.Codigo == codigo);
            return cargo;
        }

        public bool CargoExiste(string codigo)
        {
            return context.Cargos.Any(crg => crg.Codigo == codigo);
        }
    }
}
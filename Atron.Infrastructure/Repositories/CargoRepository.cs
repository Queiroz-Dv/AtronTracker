using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class CargoRepository : Repository<Cargo>, ICargoRepository
    {
        //private AtronDbContext _context;

        public CargoRepository(AtronDbContext context, ILiteDbContext liteDbContext) : base(context, liteDbContext)
        {
            //_context = context;
        }

        public async Task<Cargo> CriarCargoAsync(Cargo cargo)
        {
            _context.Cargos.Add(cargo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return cargo;
        }

        public async Task<Cargo> ObterCargoPorIdAsync(int? id)
        {
            var cargo = await _context.Cargos.FindAsync(id);
            return cargo;
        }

        public async Task<Cargo> ObterCargoComDepartamentoPorIdAsync(int? id)
        {
            var cargos = await (from pst in _context.Cargos
                                join dept in _context.Departamentos on pst.DepartamentoId equals dept.Id
                                where dept.Id == id
                                select new Cargo
                                {
                                    Descricao = pst.Descricao,
                                    Departamento = dept,
                                    DepartamentoId = dept.Id
                                }).FirstOrDefaultAsync();


            return cargos;
        }

        public async Task<IEnumerable<Cargo>> ObterCargosAsync()
        {
            var cargos = await _context.Cargos.Include(dpt => dpt.Departamento).AsNoTracking().ToListAsync();
            return cargos;
        }

        public async Task<Cargo> RemoverCargoAsync(Cargo cargo)
        {
            _context.Cargos.Remove(cargo);
            await _context.SaveChangesAsync();
            return cargo;
        }

        public async Task<Cargo> AtualizarCargoAsync(Cargo cargo)
        {
            var cargoBd = await ObterCargoPorCodigoAsync(cargo.Codigo);
            cargoBd.Descricao = cargo.Descricao;
            cargoBd.DepartamentoId = cargo.DepartamentoId;
            cargoBd.DepartamentoCodigo = cargo.DepartamentoCodigo;

            try
            {
                _context.Cargos.Update(cargoBd);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return cargo;
        }

        public async Task<Cargo> ObterCargoPorCodigoAsync(string codigo)
        {
            return await _context.Cargos.Include(dpt => dpt.Departamento).FirstOrDefaultAsync(crg => crg.Codigo == codigo);
        }

        public async Task<Cargo> ObterCargoPorCodigoAsyncAsNoTracking(string codigo)
        {
            return await _context.Cargos.Include(dpt => dpt.Departamento).AsNoTracking().FirstOrDefaultAsync(crg => crg.Codigo == codigo);
        }


        public bool CargoExiste(string codigo)
        {
            return _context.Cargos.Any(crg => crg.Codigo == codigo);
        }

        public async Task<IEnumerable<Cargo>> ObterCargosPorDepartamento(int departamentoId, string departamentoCodigo)
        {
            return await _context.Cargos.Where(crg => crg.DepartamentoId == departamentoId && crg.DepartamentoCodigo == departamentoCodigo).ToListAsync();
        }
    }
}
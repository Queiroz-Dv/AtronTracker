using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class CargoRepository : ICargoRepository
    {
        private AtronDbContext _context;

        public CargoRepository(AtronDbContext context)
        {
            _context = context;
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
                                join dept in _context.Departamentos on pst.DepartmentoId equals dept.Id
                                where dept.Id == id
                                select new Cargo
                                {
                                    Descricao = pst.Descricao,
                                    Departamento = dept,
                                    DepartmentoId = dept.Id
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
            cargoBd.DepartmentoId = cargo.DepartmentoId;
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

        public bool CargoExiste(string codigo)
        {
            return _context.Cargos.Any(crg => crg.Codigo == codigo);
        }
    }
}
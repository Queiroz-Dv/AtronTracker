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
    public class CargoRepository : Repository<Cargo>, ICargoRepository
    {
        private AtronDbContext _context;

        public CargoRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CriarCargoAsync(Cargo cargo)
        {

            try
            {
                await _context.Cargos.AddAsync(cargo);

                var cargoGravado = await _context.SaveChangesAsync();
                return cargoGravado > 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
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

        public async Task<bool> RemoverCargoAsync(Cargo cargo)
        {
            _context.Cargos.Remove(cargo);
            var cargoRemovido = await _context.SaveChangesAsync();
            return cargoRemovido > 0;
        }

        public async Task<bool> AtualizarCargoAsync(Cargo cargo)
        {
            var cargoBd = await ObterCargoPorCodigoAsync(cargo.Codigo);
            cargoBd.Descricao = cargo.Descricao;
            cargoBd.DepartamentoId = cargo.DepartamentoId;
            cargoBd.DepartamentoCodigo = cargo.DepartamentoCodigo;

            try
            {
                _context.Cargos.Update(cargoBd);
                var atualizado = await _context.SaveChangesAsync();
                return atualizado > 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<Cargo> ObterCargoPorCodigoAsync(string codigo)
        {
            return await _context.Cargos.Include(dpt => dpt.Departamento).FirstOrDefaultAsync(crg => crg.Codigo == codigo);
        }

        public async Task<Cargo> ObterCargoPorCodigoAsyncAsNoTracking(string codigo)
        {
            return await _context.Cargos.Include(dpt => dpt.Departamento).AsNoTracking().FirstOrDefaultAsync(crg => crg.Codigo == codigo);
        }

        public async Task<IEnumerable<Cargo>> ObterCargosPorDepartamento(int departamentoId, string departamentoCodigo)
        {
            return await _context.Cargos.Where(crg => crg.DepartamentoId == departamentoId && crg.DepartamentoCodigo == departamentoCodigo).ToListAsync();
        }
    }
}
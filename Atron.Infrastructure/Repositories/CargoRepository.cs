using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class CargoRepository : Repository<Cargo>, ICargoRepository
    {
        private readonly ILiteUnitOfWork _uow;

        public CargoRepository(AtronDbContext context,
                               ILiteDbContext liteDbContext,
                               ILiteUnitOfWork liteUnitOfWork) : base(context, liteDbContext)
        {
            _uow = liteUnitOfWork;
        }

        public async Task<Cargo> CriarCargoAsync(Cargo cargo)
        {
            _uow.BeginTransaction();
            try
            {
                await _liteContext.Cargos.InsertAsync(cargo);
                _uow.Commit();
            }
            catch
            {
                _uow.Rollback();
                throw;
            }

            return await _liteContext.Cargos.FindOneAsync(crg => crg.Codigo == cargo.Codigo);
        }

        public async Task<Cargo> ObterCargoPorIdAsync(int? id)
        {
            var departamentos = await _liteContext.Departamentos.FindAllAsync();
            var cargo = await _liteContext.Cargos.FindByIdAsync(id);

            var departamento = (from dpt in departamentos.ToList()
                                where dpt.Id == cargo.DepartamentoId
                                select new Departamento()
                                {
                                    Id = dpt.Id,
                                    Codigo = dpt.Codigo,
                                    Descricao = dpt.Descricao
                                }).FirstOrDefault();

            cargo.Departamento = departamento;

            return cargo;
        }

        public async Task<Cargo> ObterCargoComDepartamentoPorIdAsync(int? id)
        {
            var cargosDb = await _liteContext.Cargos.FindAllAsync();
            var departamentosDb = await _liteContext.Departamentos.FindAllAsync();

            var cargos = (from pst in cargosDb.ToList()
                          join dept in departamentosDb.ToList() on pst.DepartamentoId equals dept.Id
                          where dept.Id == id
                          select new Cargo
                          {
                              Descricao = pst.Descricao,
                              Departamento = dept,
                              DepartamentoId = dept.Id
                          }).FirstOrDefault();
            return cargos;
        }

        public async Task<IEnumerable<Cargo>> ObterCargosAsync()
        {
            var cargos = await _liteContext.Cargos.FindAllAsync();
            var departamentos = await _liteContext.Departamentos.FindAllAsync();

            cargos = (from crg in cargos.ToList()
                      join dpt in departamentos.ToList() on crg.DepartamentoId equals dpt.Id
                      select new Cargo
                      {
                          Codigo = crg.Codigo,
                          Descricao = crg.Descricao,
                          DepartamentoId = dpt.Id,
                          DepartamentoCodigo = dpt.Codigo,
                          Departamento = dpt
                      }).ToList();
            return cargos;
        }

        public async Task<bool> RemoverCargoAsync(Cargo cargo)
        {
            _uow.BeginTransaction();
            try
            {
                var cargoBd = await _liteContext.Cargos.FindOneAsync(crg => crg.Codigo == cargo.Codigo);
                return await _liteContext.Cargos.DeleteAsync(cargo.Id);
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
        }

        public async Task<bool> AtualizarCargoAsync(Cargo cargo)
        {
            var cargoBd = await ObterCargoPorCodigoAsync(cargo.Codigo);
            cargoBd.Descricao = cargo.Descricao;
            cargoBd.DepartamentoId = cargo.DepartamentoId;
            cargoBd.DepartamentoCodigo = cargo.DepartamentoCodigo;

            _uow.BeginTransaction();
            try
            {
                var atualizado = await _liteContext.Cargos.UpdateAsync(cargoBd);
                _uow.Commit();
                return atualizado;
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
        }

        public async Task<Cargo> ObterCargoPorCodigoAsync(string codigo)
        {
            var departamentos = await _liteContext.Departamentos.FindAllAsync();
            var cargo = await _liteContext.Cargos.FindOneAsync(crg => crg.Codigo == codigo);

            var departamento = (from dpt in departamentos.ToList()
                                where dpt.Id == cargo.DepartamentoId
                                select new Departamento()
                                {
                                    Id = dpt.Id,
                                    Codigo = dpt.Codigo,
                                    Descricao = dpt.Descricao
                                }).FirstOrDefault();
            return cargo;
        }

        public async Task<Cargo> ObterCargoPorCodigoAsyncAsNoTracking(string codigo)
        {
            return await _context.Cargos.Include(dpt => dpt.Departamento).AsNoTracking().FirstOrDefaultAsync(crg => crg.Codigo == codigo);
        }

        public bool CargoExiste(string codigo)
        {
            return _liteContext.Cargos.AnyAsync(crg => crg.Codigo == codigo).Result;
        }

        public async Task<IEnumerable<Cargo>> ObterCargosPorDepartamento(int departamentoId, string departamentoCodigo)
        {
            var cargos = await _liteContext.Cargos.FindAllAsync();
            var departamentos = await _liteContext.Departamentos.FindAllAsync();

            cargos = (from crg in cargos.ToList()
                      join dpt in departamentos.ToList() on crg.DepartamentoId equals dpt.Id
                      where crg.DepartamentoId == departamentoId && crg.DepartamentoCodigo == departamentoCodigo
                      select new Cargo
                      {
                          Codigo = crg.Codigo,
                          Descricao = crg.Descricao,
                          DepartamentoId = dpt.Id,
                          DepartamentoCodigo = dpt.Codigo,
                          Departamento = dpt
                      }).ToList();

            return cargos;
        }
    }
}
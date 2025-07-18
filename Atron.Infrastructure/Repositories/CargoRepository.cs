using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Shared.Interfaces.Accessor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class CargoRepository : Repository<Cargo>, ICargoRepository
    {
        private IDataSet<Cargo> Cargos => _facade.LiteDbContext.Cargos;
        private IDataSet<Departamento> Departamentos => _facade.LiteDbContext.Departamentos;

        public CargoRepository(ILiteFacade liteFacade, IServiceAccessor serviceAccessor)
            : base(liteFacade, serviceAccessor)
        { }

        public async Task<bool> CriarCargoAsync(Cargo cargo)
        {
            return await Cargos.InsertAsync(cargo);
        }

        public async Task<Cargo> ObterCargoPorIdAsync(int? id)
        {
            var departamentos = await Departamentos.FindAllAsync();
            var cargo = await Cargos.FindByIdAsync(id);

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
            var cargosDb = await Cargos.FindAllAsync();
            var departamentosDb = await Departamentos.FindAllAsync();

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
            var cargos = await Cargos.FindAllAsync();
            var departamentos = await Departamentos.FindAllAsync();

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
            var cargoBd = await Cargos.FindOneAsync(crg => crg.Codigo == cargo.Codigo);
            return await Cargos.DeleteAsync(cargo.Id);

        }

        public async Task<bool> AtualizarCargoAsync(Cargo cargo)
        {
            var cargoBd = await ObterCargoPorCodigoAsync(cargo.Codigo);
            cargoBd.Descricao = cargo.Descricao;
            cargoBd.DepartamentoId = cargo.DepartamentoId;
            cargoBd.DepartamentoCodigo = cargo.DepartamentoCodigo;

            return await Cargos.UpdateAsync(cargoBd);
        }

        public async Task<Cargo> ObterCargoPorCodigoAsync(string codigo)
        {
            var departamentos = await Departamentos.FindAllAsync();
            var cargo = await Cargos.FindOneAsync(crg => crg.Codigo == codigo);

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

        public bool CargoExiste(string codigo)
        {
            return Cargos.AnyAsync(crg => crg.Codigo == codigo).Result;
        }

        public async Task<IEnumerable<Cargo>> ObterCargosPorDepartamento(int departamentoId, string departamentoCodigo)
        {
            var cargos = await Cargos.FindAllAsync();
            var departamentos = await Departamentos.FindAllAsync();

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
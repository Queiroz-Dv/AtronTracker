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
    public class DepartamentoRepository : Repository<Departamento>, IDepartamentoRepository
    {
        private IDataSet<Departamento> Departamentos => _facade.LiteDbContext.Departamentos;

        public DepartamentoRepository(ILiteFacade liteFacade, IServiceAccessor serviceAccessor) : base(liteFacade, serviceAccessor)
        { }

        public async Task<bool> AtualizarDepartamentoRepositoryAsync(Departamento departamento)
        {
            var entidade = await Departamentos.FindOneAsync(dpt => dpt.Codigo == departamento.Codigo);

            entidade.Descricao = departamento.Descricao;
            return await Departamentos.UpdateAsync(entidade);
        }

        public async Task<bool> CriarDepartamentoRepositoryAsync(Departamento departamento)
        {
            return await Departamentos.InsertAsync(departamento);
        }

        public bool DepartamentoExiste(string codigo)
        {
            return Departamentos.AnyAsync(dpt => dpt.Codigo == codigo).Result;
        }

        public async Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsync(string codigo)
        {
            return await Departamentos.FindOneAsync(dpt => dpt.Codigo == codigo);
        }

        public async Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(string codigo)
        {
            return await Departamentos.FindOneAsync(dpt => dpt.Codigo == codigo);
        }

        public async Task<Departamento> ObterDepartamentoPorIdRepositoryAsync(int? id)
        {
            return await Departamentos.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Departamento>> ObterDepartmentosAsync()
        {
            var departamentos = await Departamentos.FindAllAsync();
            return departamentos.OrderBy(x => x.Codigo).ToList();
        }

        public async Task<bool> RemoverDepartmentoRepositoryAsync(Departamento departamento)
        {
            var departamentoExistente = await Departamentos.FindOneAsync(dpt => dpt.Codigo == departamento.Codigo);
            return await Departamentos.DeleteAsync(departamentoExistente.Id);
        }
    }
}
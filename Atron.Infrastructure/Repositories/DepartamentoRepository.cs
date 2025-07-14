using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class DepartamentoRepository : Repository<Departamento>, IDepartamentoRepository
    {
        public readonly ILiteUnitOfWork _uow;

        public DepartamentoRepository(AtronDbContext context, ILiteDbContext liteContext, ILiteUnitOfWork liteUnitOfWork)
            : base(context, liteContext)
        {
            _uow = liteUnitOfWork;
        }

        public async Task<Departamento> AtualizarDepartamentoRepositoryAsync(Departamento departamento)
        {
            _uow.BeginTransaction();
            try
            {
                var entidade = await _liteContext.Departamentos.FindOneAsync(dpt => dpt.Codigo == departamento.Codigo);

                if (entidade is not null)
                {
                    entidade.Descricao = departamento.Descricao;
                    await _liteContext.Departamentos.UpdateAsync(entidade);
                    _uow.Commit();
                }
            }
            catch
            {
                _uow.Rollback();
                throw;
            }

            return departamento;
        }

        public async Task<Departamento> CriarDepartamentoRepositoryAsync(Departamento departamento)
        {
            _uow.BeginTransaction();
            try
            {
                await _liteContext.Departamentos.InsertAsync(departamento);
                _uow.Commit();
            }
            catch
            {
                _uow.Rollback();
            }

            return await _liteContext.Departamentos.FindOneAsync(dpt => dpt.Codigo == departamento.Codigo);
        }

        public bool DepartamentoExiste(string codigo)
        {
            return _liteContext.Departamentos.AnyAsync(dpt => dpt.Codigo == codigo).Result;
        }

        public async Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsync(string codigo)
        {
            return await _liteContext.Departamentos.FindOneAsync(dpt => dpt.Codigo == codigo);
        }

        public async Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(string codigo)
        {
            return await _liteContext.Departamentos.FindOneAsync(dpt => dpt.Codigo == codigo);
        }

        public async Task<Departamento> ObterDepartamentoPorIdRepositoryAsync(int? id)
        {
            return await _liteContext.Departamentos.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Departamento>> ObterDepartmentosAsync()
        {
            var departamentos = await _liteContext.Departamentos.FindAllAsync();
            return departamentos.OrderBy(x => x.Codigo).ToList();
        }

        public async Task<bool> RemoverDepartmentoRepositoryAsync(Departamento departamento)
        {
            _uow.BeginTransaction();
            try
            {
                var departamentoExistente = await _liteContext.Departamentos.FindOneAsync(dpt => dpt.Codigo == departamento.Codigo);
                var deletado = await _liteContext.Departamentos.DeleteAsync(departamentoExistente.Id);
                if (deletado)
                {
                    _uow.Commit();
                    return true;
                }
            }
            catch
            {
                _uow.Rollback();
                throw;
            }

            return false;
        }
    }
}
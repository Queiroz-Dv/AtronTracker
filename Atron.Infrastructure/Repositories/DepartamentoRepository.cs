using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Interfaces;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private ILiteDbContext context;
        private ILiteUnitOfWork unitOfWork;
        private IServiceAccessor serviceAccessor;

        public DepartamentoRepository(ILiteDbContext context, ILiteUnitOfWork unitOfWork, IServiceAccessor serviceAccessor)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
            this.serviceAccessor = serviceAccessor;
        }

        public async Task<bool> AtualizarDepartamentoRepositoryAsync(Departamento departamento)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var entidade = await context.Departamentos.FindOneAsync(dpt => dpt.Codigo == departamento.Codigo);

                entidade.Descricao = departamento.Descricao;
                var atualizado = await context.Departamentos.UpdateAsync(entidade);

                unitOfWork.Commit();
                return atualizado;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<bool> CriarDepartamentoRepositoryAsync(Departamento departamento)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var gravado = await context.Departamentos.InsertAsync(departamento);
                unitOfWork.Commit();
                return gravado > 0;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<bool> DepartamentoExiste(string codigo)
        {
            return await context.Departamentos.AnyAsync(dpt => dpt.Codigo == codigo);
        }

        public async Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsync(string codigo)
        {
            var departamento = await context.Departamentos.FindOneAsync(dpt => dpt.Codigo == codigo);
            return departamento;
        }

        public async Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(string codigo)
        {
            return await context.Departamentos.FindOneAsync(dpt => dpt.Codigo == codigo);
        }

        public async Task<Departamento> ObterDepartamentoPorIdRepositoryAsync(int? id)
        {
            return await context.Departamentos.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Departamento>> ObterDepartmentosAsync()
        {
            var departamentos = await context.Departamentos.FindAllAsync();
            return departamentos.OrderBy(x => x.Codigo).ToList();
        }

        public async Task<bool> RemoverDepartmentoRepositoryAsync(Departamento departamento)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var departamentoExistente = await context.Departamentos.FindOneAsync(dpt => dpt.Codigo == departamento.Codigo);
                var deletado = await context.Departamentos.DeleteAsync(departamentoExistente.Id);
                unitOfWork.Commit();
                return deletado;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }
    }
}
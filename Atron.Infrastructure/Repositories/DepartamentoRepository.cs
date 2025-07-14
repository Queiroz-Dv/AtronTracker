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
    public class DepartamentoRepository : Repository<Departamento>, IDepartamentoRepository
    {
        public DepartamentoRepository(AtronDbContext context, ILiteDbContext liteContext)
            : base(context, liteContext) { }

        public async Task<Departamento> AtualizarDepartamentoRepositoryAsync(Departamento departamento)
        {
            var entidade = await _liteContext.Departamentos.FindOneAsync(dpt => dpt.Codigo == departamento.Codigo);
            RunInTransaction(async () =>
            {
                if (entidade is not null)
                {
                    entidade.Descricao = departamento.Descricao;
                    await _liteContext.Departamentos.UpdateAsync(entidade);
                }
            });

            return departamento;
        }

        public async Task<Departamento> CriarDepartamentoRepositoryAsync(Departamento departamento)
        {
            int id = 0;
            RunInTransaction(async () =>
            {
                var resultado = await _liteContext.Departamentos.InsertAsync(departamento);
                id = resultado.AsInt32;
            });

            var departamentos = await _liteContext.Departamentos.FindAllAsync();
            return departamentos.FirstOrDefault(dpt => dpt.Codigo == departamento.Codigo);
        }

        public bool DepartamentoExiste(string codigo)
        {
            return _liteContext.Departamentos.AnyAsync(dpt => dpt.Codigo == codigo).Result;
        }

        public async Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsync(string codigo)
        {
            var departamentos = await _liteContext.Departamentos.FindAllAsync();
            return departamentos.FirstOrDefault(dpt => dpt.Codigo == codigo);
        }

        public async Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(string codigo)
        {
            return await _context.Departamentos
                .AsNoTracking()
                .FirstOrDefaultAsync(dpt => dpt.Codigo == codigo);
            //return await _liteContext.Departamentos.FindOneAsync(dpt => dpt.Codigo == codigo);
        }

        public async Task<Departamento> ObterDepartamentoPorIdRepositoryAsync(int? id)
        {
            return await _liteContext.Departamentos.FindOneAsync(dpt => dpt.Id == id);
        }

        public async Task<IEnumerable<Departamento>> ObterDepartmentosAsync()
        {
            var departamentos = await _liteContext.Departamentos.FindAllAsync();
            return departamentos.OrderBy(x => x.Codigo).ToList();
        }

        public async Task<Departamento> RemoverDepartmentoRepositoryAsync(Departamento departamento)
        {
            int id = 0;
            RunInTransaction(async () =>
            {
                var departamentoExistente = await _liteContext.Departamentos.FindOneAsync(dpt => dpt.Codigo == departamento.Codigo);
                var resultado = await _liteContext.Departamentos.DeleteAsync(departamentoExistente.Id);
            });

            return await ObterDepartamentoPorIdRepositoryAsync(id);
        }
    }
}
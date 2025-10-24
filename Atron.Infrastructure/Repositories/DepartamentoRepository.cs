using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Tracker.Infrastructure.Repositories
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private AtronDbContext _context;

        public DepartamentoRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AtualizarDepartamentoRepositoryAsync(Departamento departamento)
        {
            var entidade = await _context.Departamentos.FirstOrDefaultAsync(dpt => dpt.Codigo == departamento.Codigo);

            try
            {
                if (entidade is not null)
                {
                    entidade.Descricao = departamento.Descricao;
                    var atualizado = await _context.SaveChangesAsync();
                    return atualizado > 0;
                }
            }
            catch (Exception ex)
            {
                var message = ex.ToString();
                throw;
            }

            return false;
        }

        public async Task<bool> CriarDepartamentoRepositoryAsync(Departamento departamento)
        {
            try
            {
                await _context.AddAsync(departamento);

                var gravado  = await _context.SaveChangesAsync();
                return gravado > 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }    

        public async Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsync(string codigo)
        {
            return await _context.Departamentos.FirstOrDefaultAsync(dpt => dpt.Codigo == codigo);
        }


        public async Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(string codigo)
        {
            return await _context.Departamentos.AsNoTracking().FirstOrDefaultAsync(dpt => dpt.Codigo == codigo);
        }

        public async Task<Departamento> ObterDepartamentoPorIdRepositoryAsync(int? id)
        {
            return  await _context.Departamentos.AsNoTracking().FirstOrDefaultAsync(dpt => dpt.Id == id);
        }

        public async Task<IEnumerable<Departamento>> ObterDepartmentosAsync()
        {
            return await _context.Departamentos.OrderByDescending(order => order.Codigo).ToListAsync();
        }

        public async Task<bool> RemoverDepartmentoRepositoryAsync(Departamento departamento)
        {
            _context.Remove(departamento);
            var removido  = await _context.SaveChangesAsync();
            return removido > 0;
        }
    }
}
﻿using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private AtronDbContext _context;

        public DepartamentoRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<Departamento> AtualizarDepartamentoRepositoryAsync(Departamento departamento)
        {
            var entidade = await _context.Departamentos.FirstOrDefaultAsync(dpt => dpt.Codigo == departamento.Codigo);

            try
            {
                if (entidade is not null)
                {
                    entidade.Descricao = departamento.Descricao;
                    await _context.SaveChangesAsync();
                    return entidade;
                }
            }
            catch (Exception ex)
            {
                var message = ex.ToString();
                throw;
            }

            return departamento;
        }

        public async Task<Departamento> CriarDepartamentoRepositoryAsync(Departamento departamento)
        {
            try
            {
                await _context.AddAsync(departamento);

                await _context.SaveChangesAsync();
                return departamento;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool DepartamentoExiste(string codigo)
        {
            return _context.Departamentos.Any(dpt => dpt.Codigo == codigo);
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

        public async Task<Departamento> RemoverDepartmentoRepositoryAsync(Departamento departamento)
        {
            _context.Remove(departamento);
            await _context.SaveChangesAsync();
            return departamento;
        }
    }
}
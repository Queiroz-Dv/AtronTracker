using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class ModuloRepository : IModuloRepository
    {
        private AtronDbContext _context;

        public ModuloRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CriarModuloRepository(Modulo modulo)
        {
            try
            {
                await _context.AddAsync(modulo);

                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Modulo> Atualizar(Modulo modulo)
        {
            try
            {
                var entidade = await ObterPorCodigoRepository(modulo.Codigo);
                if (entidade is not null)
                {
                    entidade.Descricao = modulo.Descricao;
                    await _context.SaveChangesAsync();
                    return entidade;
                }
            }
            catch (Exception ex)
            {
                var message = ex.ToString();
                throw;
            }

            return modulo;
        }

        public async Task<Modulo> ObterPorCodigoRepository(string codigo)
        {
            return await _context.Modulo.FirstOrDefaultAsync(mdl => mdl.Codigo == codigo);
        }

        public async Task<Modulo> ObterPorIdRepository(int id)
        {
            return await _context.Modulo.FirstOrDefaultAsync(mdl => mdl.Id == id);
        }

        public async Task<IEnumerable<Modulo>> ObterTodosRepository()
        {
            return await _context.Modulo.ToListAsync();
        }

        public async Task<bool> RemoverModuloRepository(Modulo modulo)
        {
            _context.Remove(modulo);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
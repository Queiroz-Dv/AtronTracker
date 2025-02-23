using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class PerfilDeAcessoRepository : IPerfilDeAcessoRepository
    {
        private readonly AtronDbContext _context;

        public PerfilDeAcessoRepository(AtronDbContext context)
        {
            _context = context;
        }

        public Task<bool> AtualizarPerfilRepositoryAsync(PerfilDeAcesso perfilDeAcesso)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CriarPerfilRepositoryAsync(PerfilDeAcesso perfilDeAcesso)
        {
            try
            {
                await _context.PerfisDeAcesso.AddAsync(perfilDeAcesso);
                var result =  await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<bool> DeletarPerfilRepositoryAsync(string codigo)
        {
            throw new NotImplementedException();
        }

        public Task<PerfilDeAcesso> ObterPerfilPorCodigoRepositoryAsync(string codigo)
        {
            throw new NotImplementedException();
        }

        public Task<PerfilDeAcesso> ObterPerfilPorIdRepositoryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<PerfilDeAcesso>> ObterTodosPerfisRepositoryAsync()
        {
            return await _context.PerfisDeAcesso.ToListAsync();
        }
    }
}
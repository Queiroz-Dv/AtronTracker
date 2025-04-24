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

        public async Task<bool> AtualizarPerfilRepositoryAsync(string codigo, PerfilDeAcesso perfilDeAcesso)
        {
            var perfilBd = await ObterPerfilPorCodigoRepositoryAsync(codigo);
            perfilBd.Codigo = perfilDeAcesso.Codigo;
            perfilBd.Descricao = perfilDeAcesso.Descricao;
            perfilBd.PerfilDeAcessoModulos = perfilDeAcesso.PerfilDeAcessoModulos;

            try
            {
                _context.PerfisDeAcesso.Update(perfilBd);
                var result  = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {

                throw;
            }
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

        public async Task<bool> DeletarPerfilRepositoryAsync(PerfilDeAcesso perfil)
        {
            _context.Remove(perfil);
            var result  =await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<PerfilDeAcesso> ObterPerfilPorCodigoRepositoryAsync(string codigo)
        {
            return await _context.PerfisDeAcesso
               .Include(pam => pam.PerfilDeAcessoModulos)
               .ThenInclude(mdl => mdl.Modulo).FirstOrDefaultAsync(pf => pf.Codigo == codigo);
        }

        public Task<PerfilDeAcesso> ObterPerfilPorIdRepositoryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<PerfilDeAcesso>> ObterTodosPerfisRepositoryAsync()
        {
            return await _context.PerfisDeAcesso
                .Include(pam => pam.PerfilDeAcessoModulos)
                .ThenInclude(mdl => mdl.Modulo)                
                .ToListAsync();
        }
    }
}
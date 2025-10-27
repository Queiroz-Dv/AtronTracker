using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PerfilDeAcessoUsuarioRepository : IPerfilDeAcessoUsuarioRepository
    {
        private readonly AtronDbContext _context;

        public PerfilDeAcessoUsuarioRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CriarPerfilRepositoryAsync(PerfilDeAcessoUsuario perfilDeAcesso)
        {
            try
            {
                await _context.PerfilDeAcessoUsuarios.AddAsync(perfilDeAcesso);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeletarRelacionamento(PerfilDeAcessoUsuario relacionamento)
        {
            try
            {
                _context.PerfilDeAcessoUsuarios.Remove(relacionamento);
                await _context.SaveChangesAsync();
                return; // Fixed: Added a valid return statement for the void method
            }
            catch (Exception ex)
            {
                throw; // No changes needed here
            }
        }

        public async Task<PerfilDeAcessoUsuario> ObterPerfilDeAcessoPorCodigoRepositoryAsync(string codigo)
        {
            return await _context.PerfilDeAcessoUsuarios
                                 .Include(p => p.PerfilDeAcesso)
                                 .ThenInclude(m => m.PerfilDeAcessoModulos)
                                 .Include(p => p.Usuario)
                                 .FirstOrDefaultAsync(pda => pda.PerfilDeAcessoCodigo == codigo);
        }
    }
}

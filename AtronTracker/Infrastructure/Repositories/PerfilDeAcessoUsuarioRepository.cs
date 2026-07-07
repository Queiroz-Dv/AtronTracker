using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PerfilDeAcessoUsuarioRepository(AtronDbContext context) : IPerfilDeAcessoUsuarioRepository
    {
        private readonly AtronDbContext _context = context;

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

        public async Task<bool> CriarRelacionamentoRepositoryAsync(PerfilDeAcessoUsuario perfilDeAcesso)
        {
            await _context.PerfilDeAcessoUsuarios.AddAsync(perfilDeAcesso);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task DeletarRelacionamento(PerfilDeAcessoUsuario relacionamento)
        {
            _context.PerfilDeAcessoUsuarios.Remove(relacionamento);
            await _context.SaveChangesAsync();
            return;
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

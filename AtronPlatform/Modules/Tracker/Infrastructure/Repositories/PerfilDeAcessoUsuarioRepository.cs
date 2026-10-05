using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
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
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CriarRelacionamentoRepositoryAsync(PerfilDeAcessoUsuario perfilDeAcesso)
        {
            await _context.PerfilDeAcessoUsuarios.AddAsync(perfilDeAcesso);
            return true;
        }

        public Task DeletarRelacionamento(PerfilDeAcessoUsuario relacionamento)
        {
            _context.PerfilDeAcessoUsuarios.Remove(relacionamento);
            return Task.CompletedTask;
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
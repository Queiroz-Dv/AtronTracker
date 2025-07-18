using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Accessor;
using System;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class PerfilDeAcessoUsuarioRepository : Repository<PerfilDeAcessoUsuario>,  IPerfilDeAcessoUsuarioRepository
    {
        public PerfilDeAcessoUsuarioRepository(ILiteFacade liteFacade, IServiceAccessor serviceAccessor) : base(liteFacade, serviceAccessor)
        {
        }

        public async Task<bool> CriarPerfilRepositoryAsync(PerfilDeAcessoUsuario perfilDeAcesso)
        {
            throw new NotImplementedException();
        }

        public async Task DeletarRelacionamento(PerfilDeAcessoUsuario relacionamento)
        {
            throw new NotImplementedException();
        }

        public async Task<PerfilDeAcessoUsuario> ObterPerfilDeAcessoPorCodigoRepositoryAsync(string codigo)
        {
            //return await _context.PerfilDeAcessoUsuarios
            //                     .Include(p => p.PerfilDeAcesso)
            //                     .ThenInclude(m => m.PerfilDeAcessoModulos)
            //                     .Include(p => p.Usuario)
            //                     .FirstOrDefaultAsync(pda => pda.PerfilDeAcessoCodigo == codigo);
            throw new NotImplementedException();
        }
    }
}

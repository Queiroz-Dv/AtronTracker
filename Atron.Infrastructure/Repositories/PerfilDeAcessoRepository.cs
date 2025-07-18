using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Accessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class PerfilDeAcessoRepository : Repository<PerfilDeAcesso>, IPerfilDeAcessoRepository
    {
        private IDataSet<PerfilDeAcesso> PerfisDeAcesso => _facade.LiteDbContext.PerfisDeAcesso;

        public PerfilDeAcessoRepository(ILiteFacade liteFacade, IServiceAccessor serviceAccessor) : base(liteFacade, serviceAccessor)
        {
        }

        public async Task<bool> AtualizarPerfilRepositoryAsync(string codigo, PerfilDeAcesso perfilDeAcesso)
        {
            var perfilBd = await ObterPerfilPorCodigoRepositoryAsync(codigo);
            perfilBd.Codigo = perfilDeAcesso.Codigo;
            perfilBd.Descricao = perfilDeAcesso.Descricao;
            perfilBd.PerfilDeAcessoModulos = perfilDeAcesso.PerfilDeAcessoModulos;

            return await PerfisDeAcesso.UpdateAsync(perfilBd);
        }

        public async Task<bool> CriarPerfilRepositoryAsync(PerfilDeAcesso perfilDeAcesso)
        {
            return await PerfisDeAcesso.InsertAsync(perfilDeAcesso);
        }

        public async Task<bool> DeletarPerfilRepositoryAsync(PerfilDeAcesso perfil)
        {
            var perfilBd = await ObterPerfilPorCodigoRepositoryAsync(perfil.Codigo);
            return await PerfisDeAcesso.DeleteAsync(perfilBd.Id);                      
        }

        public async Task<PerfilDeAcesso> ObterPerfilPorCodigoRepositoryAsync(string codigo)
        {
            //return await _context.PerfisDeAcesso
            //   .Include(pam => pam.PerfilDeAcessoModulos) // Relacionamento com módulos
            //   .ThenInclude(mdl => mdl.Modulo) // Dentro do relacionamento vai trazer os módulos
            //   .Include(pda => pda.PerfisDeAcessoUsuario) // Relacionamento com usuários
            //   .ThenInclude(usr => usr.Usuario)
            //   .FirstOrDefaultAsync(pf => pf.Codigo == codigo);
            return null;
        }

        public Task<PerfilDeAcesso> ObterPerfilPorIdRepositoryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PerfilDeAcesso>> ObterPerfisPorCodigoDeUsuarioRepositoryAsync(string usuarioCodigo)
        {
            //return await _context.PerfisDeAcesso
            //    .Include(pam => pam.PerfilDeAcessoModulos)
            //        .ThenInclude(pam => pam.Modulo)
            //    .Include(pda => pda.PerfisDeAcessoUsuario)
            //        .ThenInclude(pau => pau.Usuario)
            //    .Where(p => p.PerfisDeAcess
            //    oUsuario.Any(u => u.Usuario.Codigo == usuarioCodigo))
            //    .ToListAsync();
            throw new NotImplementedException();
        }

        public async Task<ICollection<PerfilDeAcesso>> ObterTodosPerfisRepositoryAsync()
        {
            //return await _context.PerfisDeAcesso
            //    .Include(pam => pam.PerfilDeAcessoModulos)
            //    .ThenInclude(mdl => mdl.Modulo)
            //    .ToListAsync();

            throw new NotImplementedException();
        }
    }
}
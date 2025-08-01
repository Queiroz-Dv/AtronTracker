using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Interfaces;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class PerfilDeAcessoUsuarioRepository : IPerfilDeAcessoUsuarioRepository
    {
        private ILiteDbContext context;
        private ILiteUnitOfWork unitOfWork;
        private IServiceAccessor serviceAccessor;

        public PerfilDeAcessoUsuarioRepository(ILiteDbContext context,
                                               ILiteUnitOfWork unitOfWork,
                                               IServiceAccessor serviceAccessor)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
            this.serviceAccessor = serviceAccessor;
        }

        public async Task<bool> CriarPerfilRepositoryAsync(PerfilDeAcessoUsuario perfilDeAcesso)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var gravado = await context.PerfisDeAcessoUsuario.InsertAsync(perfilDeAcesso);
                if (gravado > 0)
                {
                    unitOfWork.Commit();
                    return true;
                }
                else
                {
                    unitOfWork.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task DeletarRelacionamento(PerfilDeAcessoUsuario relacionamento)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var perfilDeAcesso = await context.PerfisDeAcessoUsuario.FindOneAsync(prf =>
                prf.PerfilDeAcessoCodigo == relacionamento.PerfilDeAcessoCodigo &&
                prf.PerfilDeAcessoId == relacionamento.PerfilDeAcessoId &&
                prf.UsuarioCodigo == relacionamento.UsuarioCodigo &&
                prf.UsuarioId == relacionamento.UsuarioId);

                if (perfilDeAcesso != null)
                {
                    await context.PerfisDeAcessoUsuario.DeleteAsync(perfilDeAcesso.Id);
                    unitOfWork.Commit();
                    return;
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return;
            }
        }

        public async Task<PerfilDeAcessoUsuario> ObterPerfilDeAcessoPorCodigoRepositoryAsync(string codigo)
        {
            var perfilDeAcesso = await context.PerfisDeAcessoUsuario.FindOneAsync(pda => pda.PerfilDeAcessoCodigo == codigo);
            return perfilDeAcesso;
        }
    }
}

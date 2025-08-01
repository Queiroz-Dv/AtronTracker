using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Interfaces;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class PerfilDeAcessoRepository : IPerfilDeAcessoRepository
    {
        private ILiteDbContext context;
        private ILiteUnitOfWork unitOfWork;
        private IServiceAccessor serviceAccessor;

        public PerfilDeAcessoRepository(ILiteDbContext context,
                                        ILiteUnitOfWork unitOfWork,
                                        IServiceAccessor serviceAccessor)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
            this.serviceAccessor = serviceAccessor;
        }

        public async Task<bool> AtualizarPerfilRepositoryAsync(string codigo, PerfilDeAcesso perfilDeAcesso)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var perfilBd = await ObterPerfilPorCodigoRepositoryAsync(codigo);
                perfilBd.Codigo = perfilDeAcesso.Codigo;
                perfilBd.Descricao = perfilDeAcesso.Descricao;
                perfilBd.PerfilDeAcessoModulos = perfilDeAcesso.PerfilDeAcessoModulos;

                var atualizado = await context.PerfisDeAcesso.UpdateAsync(perfilBd);
                unitOfWork.Commit();
                return atualizado;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<bool> CriarPerfilRepositoryAsync(PerfilDeAcesso perfilDeAcesso)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var gravado = await context.PerfisDeAcesso.InsertAsync(perfilDeAcesso);
                unitOfWork.Commit();
                return gravado > 0;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeletarPerfilRepositoryAsync(PerfilDeAcesso perfil)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var perfilBd = await ObterPerfilPorCodigoRepositoryAsync(perfil.Codigo);
                var deletado = await context.PerfisDeAcesso.DeleteAsync(perfilBd.Id);
                unitOfWork.Commit();
                return deletado;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<PerfilDeAcesso> ObterPerfilPorCodigoRepositoryAsync(string codigo)
        {
            var perfis = await ObterTodosPerfisRepositoryAsync();            
            return perfis.FirstOrDefault(prf => prf.Codigo == codigo);
        }

        public Task<PerfilDeAcesso> ObterPerfilPorIdRepositoryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PerfilDeAcesso>> ObterPerfisPorCodigoDeUsuarioRepositoryAsync(string usuarioCodigo)
        {
            try
            {
                var modulos = (await context.Modulos.FindAllAsync()).ToList();
                var perfisDeAcessoUsuario = (await context.PerfisDeAcessoUsuario.FindAllAsync()).ToList();
                var perfisDeAcessoModulo = (await context.PerfisDeAcessoModulo.FindAllAsync()).ToList();
                var perfis = (await context.PerfisDeAcesso.FindAllAsync()).ToList();

                foreach (var perfil in perfis)
                {
                    perfil.PerfilDeAcessoModulos = new List<PerfilDeAcessoModulo>();
                    perfil.PerfisDeAcessoUsuario = new List<PerfilDeAcessoUsuario>();

                    perfil.PerfilDeAcessoModulos = perfisDeAcessoModulo
                        .Where(perfisDeAcessoModulo => perfisDeAcessoModulo.PerfilDeAcessoCodigo == perfil.Codigo)
                        .Select(pam => new PerfilDeAcessoModulo
                        {
                            Modulo = new Modulo() { Codigo = pam.ModuloCodigo, Descricao = modulos.FirstOrDefault(md => md.Codigo == pam.ModuloCodigo).Descricao },
                            ModuloId = pam.ModuloId,
                            PerfilDeAcessoId = pam.PerfilDeAcessoId,
                            ModuloCodigo = pam.ModuloCodigo,
                        }).ToList();

                    perfil.PerfisDeAcessoUsuario = perfisDeAcessoUsuario.Where(p => p.PerfilDeAcessoCodigo == perfil.Codigo && p.UsuarioCodigo == usuarioCodigo)
                        .Select(p => new PerfilDeAcessoUsuario
                        {
                            UsuarioId = p.UsuarioId,
                            UsuarioCodigo = p.UsuarioCodigo,
                            PerfilDeAcessoId = p.PerfilDeAcessoId,
                            PerfilDeAcessoCodigo = p.PerfilDeAcessoCodigo
                        }).ToList();
                }

                return perfis.Where(p => p.PerfisDeAcessoUsuario.Any(usr => usr.UsuarioCodigo == usuarioCodigo)).ToList();
            }
            catch (Exception ex)
            {
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                throw;
            }
        }

        public async Task<ICollection<PerfilDeAcesso>> ObterTodosPerfisRepositoryAsync()
        {
            var perfis = await context.PerfisDeAcesso.FindAllAsync();
            var perfisDeAcessoUsuario = await context.PerfisDeAcessoUsuario.FindAllAsync();
            var perfilsDeAcessoModulo = await context.PerfisDeAcessoModulo.FindAllAsync();


            var perfisList = perfis.GroupJoin(perfisDeAcessoUsuario,
                                              perfil => perfil.Codigo,
                                              usuario => usuario.PerfilDeAcessoCodigo,
                                              (perfil, usuarios) =>
                                              {
                                                  perfil.PerfisDeAcessoUsuario = usuarios
                                                      .Select(u => new PerfilDeAcessoUsuario
                                                      {
                                                          UsuarioId = u.UsuarioId,
                                                          UsuarioCodigo = u.UsuarioCodigo,
                                                          PerfilDeAcessoId = perfil.Id,
                                                          PerfilDeAcessoCodigo = perfil.Codigo
                                                      }).ToList();

                                                  perfil.PerfilDeAcessoModulos = perfilsDeAcessoModulo
                                                      .Where(pam => pam.PerfilDeAcessoCodigo == perfil.Codigo)
                                                      .Select(pam => new PerfilDeAcessoModulo
                                                      {
                                                          ModuloCodigo = pam.ModuloCodigo,
                                                          ModuloId = pam.ModuloId,
                                                          Modulo = pam.Modulo,
                                                          PerfilDeAcessoId = pam.PerfilDeAcessoId,
                                                          PerfilDeAcessoCodigo = pam.PerfilDeAcessoCodigo
                                                      }).ToList();

                                                  return perfil;
                                              }).ToList();

            return perfisList;
        }
    }
}
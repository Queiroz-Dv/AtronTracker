using Atron.Domain.Entities;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Atron.Infrastructure.Interfaces;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private ILiteDbContext context;
        private ILiteUnitOfWork unitOfWork;
        private IServiceAccessor serviceAccessor;

        public UsuarioRepository(ILiteDbContext context,
                                 ILiteUnitOfWork unitOfWork,
                                 IServiceAccessor serviceAccessor)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
            this.serviceAccessor = serviceAccessor;
        }

        public async Task<bool> AtualizarSalario(int usuarioId, int quantidadeTotal)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var usuario = await context.Usuarios.FindByIdAsync(usuarioId);
                if (usuario != null)
                    usuario.Salario = quantidadeTotal;

                var atualizado = await context.Usuarios.UpdateAsync(usuario);
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

        public async Task<bool> AtualizarUsuarioAsync(string codigo, Usuario usuario)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var usuarioBd = await context.Usuarios.FindOneAsync(usr => usr.Codigo == codigo);
                usuarioBd.Nome = usuario.Nome;
                usuarioBd.Sobrenome = usuario.Sobrenome;
                usuarioBd.DataNascimento = usuario.DataNascimento;
                usuarioBd.Salario = usuario.Salario;
                usuarioBd.Email = usuario.Email;
                usuarioBd.UsuarioCargoDepartamentos = usuario.UsuarioCargoDepartamentos;

                var atualizado = await context.Usuarios.UpdateAsync(usuarioBd);
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

        public async Task<bool> CriarUsuarioAsync(Usuario usuario)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var gravado = await context.Usuarios.InsertAsync(usuario);
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

        public async Task<UsuarioIdentity> ObterUsuarioPorCodigoAsync(string codigo)
        {
            var relacionamentos = await context.UsuarioCargoDepartamentos.FindAllAsync();
            var applicationUser = await context.UsuarioIdentity.FindOneAsync(usr => usr.Codigo == codigo);
            var usuario = await context.Usuarios.FindOneAsync(usr => usr.Codigo == codigo);

            foreach (var item in relacionamentos)
            {
                if (item.UsuarioCodigo == usuario.Codigo)
                {
                    if(usuario.UsuarioCargoDepartamentos == null)
                    {
                        usuario.UsuarioCargoDepartamentos = new List<UsuarioCargoDepartamento>();
                    }

                    usuario.UsuarioCargoDepartamentos.Add(item);
                }
            }

            var usuarioIdentity = new UsuarioIdentity
            {
                Id = usuario.Id,
                Codigo = usuario.Codigo,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                Email = usuario.Email,
                SalarioAtual = usuario.Salario,
                DataNascimento = usuario.DataNascimento,
                UsuarioCargoDepartamentos = usuario.UsuarioCargoDepartamentos,
                RefreshToken = applicationUser.RefreshToken,
                RefreshTokenExpireTime = applicationUser.RefreshTokenExpireTime
            };

            return usuarioIdentity;
        }

        public async Task<Usuario> ObterUsuarioPorIdAsync(int? id)
        {
            return await context.Usuarios.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Usuario>> ObterUsuariosAsync()
        {
            var relacionamentos = await context.UsuarioCargoDepartamentos.FindAllAsync();
            var usuarios = await context.Usuarios.FindAllAsync();

            // Monta lista de usuários com seus relacionamentos
            var usuariosComRelacionamentos = usuarios.Select(usr =>
            {
                usr.UsuarioCargoDepartamentos = relacionamentos
                    .Where(rel => rel.UsuarioCodigo == usr.Codigo)
                    .ToList();
                return usr;
            }).ToList();

            return usuariosComRelacionamentos;
        }

        public async Task<bool> RemoverUsuarioAsync(Usuario usuario)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var usuarioBd = await context.Usuarios.FindOneAsync(usr => usr.Codigo == usuario.Codigo);
                var deletado = await context.Usuarios.DeleteAsync(usuarioBd.Id);
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

        public async Task<bool> UsuarioExiste(string codigo)
        {
            return await context.Usuarios.AnyAsync(usr => usr.Codigo == codigo);
        }

        public async Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity()
        {    
            var applicationUsers = (await context.UsuarioIdentity.FindAllAsync()).ToList();
            var usuarios = (await context.Usuarios.FindAllAsync()).ToList();
            var relacionamentos = (await context.UsuarioCargoDepartamentos.FindAllAsync()).ToList();
            var perfisDeAcessoUsuario = (await context.PerfisDeAcessoUsuario.FindAllAsync()).ToList();
            
            var usuariosIdentity = applicationUsers.Select(identityUser =>
            {
                var usuarioPrincipal = usuarios.FirstOrDefault(u => u.Codigo == identityUser.Codigo);
            
                if (usuarioPrincipal == null)
                {
                    return null; 
                }
     
                return new UsuarioIdentity
                {
                    // Dados da tabela de autenticação (UsuarioIdentity)
                    Id = identityUser.Id,
                    Codigo = identityUser.Codigo,
                    RefreshToken = identityUser.RefreshToken,
                    RefreshTokenExpireTime = identityUser.RefreshTokenExpireTime,

                    // Dados da tabela de negócio (Usuario)
                    Nome = usuarioPrincipal.Nome,
                    Sobrenome = usuarioPrincipal.Sobrenome,
                    Email = usuarioPrincipal.Email,
                    SalarioAtual = usuarioPrincipal.Salario,
                    DataNascimento = usuarioPrincipal.DataNascimento,
                    
                    UsuarioCargoDepartamentos = relacionamentos
                        .Where(r => r.UsuarioCodigo == identityUser.Codigo)
                        .ToList(),
                   
                    PerfisDeAcessoUsuario = perfisDeAcessoUsuario
                        .Where(p => p.UsuarioCodigo == identityUser.Codigo)
                        .ToList()
                };
            }).Where(u => u != null)
            .ToList();


            return usuariosIdentity;
        }
    }
}
using Atron.Domain.Entities;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public readonly ILiteUnitOfWork _uow;

        public UsuarioRepository(AtronDbContext context,
                                 ILiteDbContext liteDbContext,
                                 ILiteUnitOfWork liteUnitOfWork) : base(context, liteDbContext)
        {
            _uow = liteUnitOfWork;
        }

        public async Task<bool> AtualizarSalario(int usuarioId, int quantidadeTotal)
        {
            _uow.BeginTransaction();
            try
            {
                var usuario = await _liteContext.Usuarios.FindByIdAsync(usuarioId);

                usuario.SalarioAtual = quantidadeTotal;

                var resultado = await _liteContext.Usuarios.UpdateAsync(usuario);

                _uow.Commit();
                return resultado;
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
        }

        public async Task<bool> AtualizarUsuarioAsync(string codigo, Usuario usuario)
        {
            var usuarioBd = await _liteContext.Usuarios.FindOneAsync(usr => usr.Codigo == codigo);
            usuarioBd.Nome = usuario.Nome;
            usuarioBd.Sobrenome = usuario.Sobrenome;
            usuarioBd.DataNascimento = usuario.DataNascimento;
            usuarioBd.SalarioAtual = usuario.SalarioAtual;
            usuarioBd.Email = usuario.Email;
            usuarioBd.UsuarioCargoDepartamentos = usuario.UsuarioCargoDepartamentos;

            _uow.BeginTransaction();
            try
            {
                var atualizado = await _liteContext.Usuarios.UpdateAsync(usuarioBd);
                _uow.Commit();
                return atualizado;
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
        }

        public async Task<bool> CriarUsuarioAsync(Usuario usuario)
        {
            _uow.BeginTransaction();
            try
            {
                int resultado = await _liteContext.Usuarios.InsertAsync(usuario);
                _uow.Commit();
                return resultado > 0;
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
        }

        public async Task<UsuarioIdentity> ObterUsuarioPorCodigoAsync(string codigo)
        {
            var relacionamentos = await _liteContext.UsuarioCargoDepartamentos.FindAllAsync();
            var applicationUser = await _liteContext.Users.FindOneAsync(usr => usr.UserName == codigo);
            var usuario = await _liteContext.Usuarios.FindOneAsync(usr => usr.Codigo == codigo);

            foreach (var item in relacionamentos)
            {
                if (item.Usuario.Codigo == usuario.Codigo)
                {
                    usuario.UsuarioCargoDepartamentos.Add(item);
                }
            }

            var usuarioIdentity = new UsuarioIdentity
            {
                Codigo = usuario.Codigo,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                Email = usuario.Email,
                SalarioAtual = usuario.SalarioAtual,
                DataNascimento = usuario.DataNascimento,
                UsuarioCargoDepartamentos = usuario.UsuarioCargoDepartamentos,
                RefreshToken = applicationUser.RefreshToken,
                RefreshTokenExpireTime = applicationUser.RefreshTokenExpireTime
            };

            return usuarioIdentity;
        }

        public async Task<Usuario> ObterUsuarioPorIdAsync(int? id)
        {
            return await _liteContext.Usuarios.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Usuario>> ObterUsuariosAsync()
        {
            var relacionamentos = await _liteContext.UsuarioCargoDepartamentos.FindAllAsync();
            var usuarios = await _liteContext.Usuarios.FindAllAsync();

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
            _uow.BeginTransaction();
            try
            {
                var usuarioBd = await _liteContext.Usuarios.FindOneAsync(usr => usr.Codigo == usuario.Codigo);
                return await _liteContext.Usuarios.DeleteAsync(usuarioBd.Id);
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
        }

        public bool UsuarioExiste(string codigo)
        {
            return _liteContext.Usuarios.AnyAsync(usr => usr.Codigo == codigo).Result;
        }

        public async Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity()
        {
            try
            {
                var applicationUsers = (await _liteContext.Users.FindAllAsync()).ToList();
                var relacionamentos = (await _liteContext.UsuarioCargoDepartamentos.FindAllAsync()).ToList();
                var usuarios = (await _liteContext.Usuarios.FindAllAsync()).ToList();

                var usuariosIdentity = new List<UsuarioIdentity>();

                foreach (var user in applicationUsers)
                {
                    var usuario = usuarios.FirstOrDefault(cdg => cdg.Codigo == user.UserName);

                    if (usuario is not null)
                    {
                        // Preenche os relacionamentos do usuário
                        usuario.UsuarioCargoDepartamentos = relacionamentos
                            .Where(rel => rel.UsuarioCodigo == usuario.Codigo)
                            .ToList();

                        var usuarioIdentity = new UsuarioIdentity
                        {
                            Codigo = usuario.Codigo,
                            Nome = usuario.Nome,
                            Sobrenome = usuario.Sobrenome,
                            Email = usuario.Email,
                            Salario = usuario.Salario,
                            SalarioAtual = usuario.SalarioAtual,
                            DataNascimento = usuario.DataNascimento,
                            UsuarioCargoDepartamentos = usuario.UsuarioCargoDepartamentos,
                            RefreshToken = user.RefreshToken,
                            RefreshTokenExpireTime = user.RefreshTokenExpireTime
                        };

                        usuariosIdentity.Add(usuarioIdentity);
                    }
                }

                return usuariosIdentity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
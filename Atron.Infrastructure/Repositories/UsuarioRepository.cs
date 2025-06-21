using Atron.Domain.Entities;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        private AtronDbContext _context;

        public UsuarioRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> AtualizarSalario(int usuarioId, int quantidadeTotal)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstAsync(usr => usr.Id == usuarioId);

                usuario.SalarioAtual = quantidadeTotal;

                _context.Update(usuario);

                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private static void AtualizarEntidadeParaPersistencia(Usuario usuario, Usuario usuarioBd)
        {
            usuarioBd.Nome = usuario.Nome;
            usuarioBd.Sobrenome = usuario.Sobrenome;
            usuarioBd.DataNascimento = usuario.DataNascimento;
            usuarioBd.SalarioAtual = usuario.SalarioAtual;
            usuarioBd.Email = usuario.Email;
            usuarioBd.UsuarioCargoDepartamentos = usuario.UsuarioCargoDepartamentos;
        }

        public async Task<bool> AtualizarUsuarioAsync(string codigo, Usuario usuario)
        {
            var usuarioBd = await ObterUsuarioPorCodigoAsync(codigo);
            AtualizarEntidadeParaPersistencia(usuario, usuarioBd);

            try
            {
                _context.Usuarios.Update(usuarioBd);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CriarUsuarioAsync(Usuario usuario)
        {
            try
            {
                await _context.Usuarios.AddAsync(usuario);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UsuarioIdentity> ObterUsuarioPorCodigoAsync(string codigo)
        {
            var applicationUser = await _context.Users.FirstOrDefaultAsync(usr => usr.UserName == codigo);
            var usuario = await _context.Usuarios.Include(rel => rel.UsuarioCargoDepartamentos).FirstOrDefaultAsync(usr => usr.Codigo == codigo);

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
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(usr => usr.Id == id);
            return usuario;
        }

        public async Task<IEnumerable<Usuario>> ObterUsuariosAsync()
        {
            return await _context.Usuarios.Include(rel => rel.UsuarioCargoDepartamentos).ToListAsync();
        }

        public async Task<Usuario> RemoverUsuarioAsync(Usuario usuario)
        {
            try
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                return new Usuario();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool UsuarioExiste(string codigo)
        {
            return _context.Usuarios.Any(usr => usr.Codigo == codigo);
        }

        public async Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity()
        {
            try
            {
                var applicationUsers = await _context.Users.ToListAsync();
                var usuarios = await _context.Usuarios.Include(rel => rel.UsuarioCargoDepartamentos).ToListAsync();

                var usuariosIdentity = new List<UsuarioIdentity>();

                foreach (var user in applicationUsers)
                {
                    var usuario = usuarios.FirstOrDefault(cdg => cdg.Codigo == user.UserName);

                    if (usuario is not null)
                    {
                        var usuarioIdentity = new UsuarioIdentity
                        {
                            Codigo = usuario.Codigo,
                            Nome = usuario.Nome,
                            Sobrenome = usuario.Sobrenome,
                            Email = usuario.Email,
                            Salario = usuario.Salario,
                            DataNascimento = usuario.DataNascimento,
                            UsuarioCargoDepartamentos = usuario.UsuarioCargoDepartamentos,                            
                        };

                        usuariosIdentity.Add(usuarioIdentity);
                    }
                }

                return usuariosIdentity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> TokenDeUsuarioExpiradoRepositoryAsync(string codigoUsuario, string refreshToken)
        {
            return await _context.Users.AnyAsync(usr => usr.UserName == codigoUsuario && usr.RefreshToken == refreshToken && usr.RefreshTokenExpireTime <= DateTime.Now);
        }
    }
}
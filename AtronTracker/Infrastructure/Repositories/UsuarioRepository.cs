using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
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
            var usuarioBd = await _context.Usuarios.FirstOrDefaultAsync(usr => usr.Codigo == codigo);
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
            if (usuario != null)
            {

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
                    RefreshTokenExpireTime = (DateTime)applicationUser.RefreshTokenExpireTime
                };

                return usuarioIdentity;
            }

            return null;
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

        public async Task<bool> RemoverUsuarioAsync(Usuario usuario)
        {
            try
            {
                _context.Usuarios.Remove(usuario);
                var removido = await _context.SaveChangesAsync();
                return removido > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity()
        {
            try
            {
                var applicationUsers = await _context.Users.ToListAsync();
                var usuarios = await _context.Usuarios.Include(rel => rel.UsuarioCargoDepartamentos).ToListAsync();

                var usuariosIdentity = await (from au in _context.Users
                                              join u in _context.Usuarios.Include(r => r.UsuarioCargoDepartamentos)
                                                on au.UserName equals u.Codigo
                                              select new UsuarioIdentity
                                              {
                                                  Codigo = u.Codigo,
                                                  Nome = u.Nome,
                                                  Sobrenome = u.Sobrenome,
                                                  Email = u.Email,
                                                  Salario = u.Salario,
                                                  DataNascimento = u.DataNascimento,
                                                  UsuarioCargoDepartamentos = u.UsuarioCargoDepartamentos
                                              }).ToListAsync();

                return usuariosIdentity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> VerificarEmailExistenteAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }
    }
}
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
    public class UsuarioRepository : IUsuarioRepository
    {
        private AtronDbContext _context;

        public UsuarioRepository(AtronDbContext context)
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
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Usuario> ObterUsuarioPorCodigoAsync(string codigo)
        {
            return await _context.Usuarios.Include(rel => rel.UsuarioCargoDepartamentos).FirstOrDefaultAsync(usr => usr.Codigo == codigo);
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
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return new Usuario();
        }

        public bool UsuarioExiste(string codigo)
        {
            return _context.Usuarios.Any(usr => usr.Codigo == codigo);
        }
    }
}
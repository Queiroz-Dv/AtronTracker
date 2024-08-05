using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
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
        AtronDbContext _context;

        public UsuarioRepository(AtronDbContext context)
        {
            _context = context;
        }

        public void AtualizarSalario(int usuarioId, int quantidadeTotal)
        {
            try
            {
                var usuario =  _context.Usuarios.First(usr => usr.Id == usuarioId);
                usuario.Salario = quantidadeTotal;
                 _context.Update(usuario);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public async Task<Usuario> AtualizarUsuarioAsync(Usuario usuario)
        {
            _context.Update(usuario);
            await _context.SaveChangesAsync();
            return new Usuario();
        }

        public async Task<Usuario> CriarUsuarioAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario> ObterUsuarioPorCodigoAsync(string codigo)
        {
            var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(usr => usr.Codigo == codigo);
            return usuario;
        }

        public async Task<Usuario> ObterUsuarioPorIdAsync(int? id)
        {
            var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(usr => usr.Id == id);
            return usuario;
        }

        public async Task<IEnumerable<Usuario>> ObterUsuariosAsync()
        {
            var usuarios = await _context.Usuarios.AsNoTracking().ToListAsync();
            return usuarios;
        }

        public async Task<Usuario> RemoverUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return new Usuario();
        }

        public bool UsuarioExiste(string codigo)
        {
            var usuarioExiste = _context.Usuarios.Where(usr => usr.Codigo == codigo).Any();
            return usuarioExiste;
        }
    }
}

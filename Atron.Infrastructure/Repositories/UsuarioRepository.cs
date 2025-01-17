﻿using Atron.Domain.Entities;
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

        public async Task AtualizarSalario(int usuarioId, int quantidadeTotal)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstAsync(usr => usr.Id == usuarioId);
                usuario.SalarioAtual = quantidadeTotal;
                _context.Update(usuario);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public async Task<Usuario> AtualizarUsuarioAsync(Usuario usuario)
        {
            var usuarioBd = await ObterUsuarioPorCodigoAsync(usuario.Codigo);
            AtualizarEntidadeParaPersistencia(usuario, usuarioBd);

            try
            {
                _context.Usuarios.Update(usuarioBd);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new Usuario();
        }

        private static void AtualizarEntidadeParaPersistencia(Usuario usuario, Usuario usuarioBd)
        {
            usuarioBd.Nome = usuario.Nome;
            usuarioBd.Sobrenome = usuario.Sobrenome;
            usuarioBd.DataNascimento = usuario.DataNascimento;
            usuarioBd.SalarioAtual = usuario.SalarioAtual;
            usuarioBd.Email = usuario.Email;
        }

        public async Task<bool> CriarUsuarioAsync(Usuario usuario)
        {            
            try
            {
                await _context.Usuarios.AddAsync(usuario);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Usuario> ObterUsuarioPorCodigoAsync(string codigo)
        {
            var usuario = await _context.Usuarios
                //.Include(crg => crg.Cargo)
                //.Include(dpt => dpt.Departamento)                
                .FirstOrDefaultAsync(usr => usr.Codigo == codigo);
            return usuario;
        }

        public async Task<Usuario> ObterUsuarioPorIdAsync(int? id)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(usr => usr.Id == id);
            return usuario;
        }

        public async Task<IEnumerable<Usuario>> ObterUsuariosAsync()
        {
            var usuarios = await _context.Usuarios
                                         //.Include(crg => crg.Cargo)
                                         //.Include(dpt => dpt.Departamento)
                                         .ToListAsync();
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
            return _context.Usuarios.Any(usr => usr.Codigo == codigo);
        }
    }
}

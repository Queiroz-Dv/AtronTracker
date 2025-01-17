﻿using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces.UsuarioInterfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> ObterUsuariosAsync();

        Task<Usuario> ObterUsuarioPorIdAsync(int? id);
        Task<Usuario> ObterUsuarioPorCodigoAsync(string codigo);

        Task<bool> CriarUsuarioAsync(Usuario usuario);

        Task<Usuario> AtualizarUsuarioAsync(Usuario usuario);

        Task<Usuario> RemoverUsuarioAsync(Usuario usuario);
        bool UsuarioExiste(string codigo);
        Task AtualizarSalario(int usuarioId, int quantidadeTotal);
    }
}
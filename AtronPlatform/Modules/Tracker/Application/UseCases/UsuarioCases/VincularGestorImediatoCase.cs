using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public sealed class VincularGestorImediatoCase(IUsuarioRepository usuarioRepository)
    {
        public IUsuarioRepository _usuarioRepository = usuarioRepository;

        public async Task<Resultado<Usuario>> ExecutarAsync(Usuario usuario, string gestorCodigo)
        {
            if (gestorCodigo.IsNullOrEmpty())
            {
                usuario.GestorImediatoId = null;
                usuario.GestorImediatoCodigo = null;
                return Resultado<Usuario>.Sucesso(usuario);
            }

            if (gestorCodigo == usuario.Codigo)
                return Resultado<Usuario>.Falha(UsuarioResource.ErroGestorProprio);

            var gestor = await _usuarioRepository.ObterUsuarioPorCodigoAsync(gestorCodigo);
            if (gestor is null)
                return Resultado<Usuario>.Falha(UsuarioResource.ErroGestorNaoEncontrado);

            usuario.GestorImediatoId = gestor.Id;
            usuario.GestorImediatoCodigo = gestor.Codigo;

            return Resultado<Usuario>.Sucesso(usuario);
        }
    }
}
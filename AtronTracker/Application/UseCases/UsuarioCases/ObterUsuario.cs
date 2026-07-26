using Application.DTO;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.UseCases.UsuarioCases
{
    public class ObterUsuario(IAsyncMap<UsuarioDTO, Usuario> asyncMap, IUsuarioRepository usuarioRepository)
    {
        private readonly IAsyncMap<UsuarioDTO, Usuario> _asyncMap = asyncMap;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        public async Task<Resultado<UsuarioDTO>> ExecutarAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<UsuarioDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var entidade = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);
            if (entidade is null)
                return Resultado<UsuarioDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dto = await _asyncMap.MapToDTOAsync(entidade);
            return Resultado<UsuarioDTO>.Sucesso(dto);
        }
    }
}

using System;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.UseCases.Usuario
{
    public class ObterUsuario
    {
        private readonly IAsyncMap<UsuarioDTO, Domain.Entities.Usuario> _asyncMap;
        private readonly IUsuarioRepository _usuarioRepository;

        public ObterUsuario(IAsyncMap<UsuarioDTO, Domain.Entities.Usuario> asyncMap, IUsuarioRepository usuarioRepository)
        {
            _asyncMap = asyncMap ?? throw new ArgumentNullException(nameof(asyncMap));
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

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

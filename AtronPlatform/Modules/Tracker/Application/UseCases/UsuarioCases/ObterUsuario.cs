using Application.DTO;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public class ObterUsuario(
        IToDtoMapper<Usuario, UsuarioDTO> usuarioMapper,
        IUsuarioRepository usuarioRepository,
        IUserAccessor userAccessor)
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUserAccessor _userAccessor = userAccessor;

        private readonly IToDtoMapper<Usuario, UsuarioDTO> _mapper = usuarioMapper;

        public async Task<Resultado<UsuarioDTO>> ExecutarAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<UsuarioDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var entidade = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);
            if (entidade is null)
                return Resultado<UsuarioDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dto = _mapper.MapToDto(entidade);
            return Resultado<UsuarioDTO>.Sucesso(dto);
        }

        public async Task<Resultado<Usuario>> ObterAsync()
        {
            var usuarioCodigo = _userAccessor.ObterCodigoUsuarioLogado();
            if (usuarioCodigo.IsNullOrEmpty())
                return Resultado<Usuario>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuarioCodigo);
            return usuario is null
                ? Resultado<Usuario>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado)
                : Resultado<Usuario>.Sucesso(usuario);
        }       
    }
}
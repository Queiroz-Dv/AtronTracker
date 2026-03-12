using Application.DTO;
using Application.DTO.Request;
using Application.Interfaces.Services;
using Application.UseCases.Usuario;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    /// <summary>
    /// Serviço de usuários — delega cada operação ao respectivo use case.
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly IAsyncMap<UsuarioDTO, Usuario> _asyncMap;
        private readonly IUsuarioRepository _usuarioRepository;

        // Use cases
        private readonly CriarUsuario _criarUsuario;
        private readonly AtualizarUsuario _atualizarUsuario;
        private readonly RemoverUsuario _removerUsuario;

        public UsuarioService(
            IAsyncMap<UsuarioDTO, Usuario> asyncMap,
            IUsuarioRepository usuarioRepository,
            CriarUsuario criarUsuario,
            AtualizarUsuario atualizarUsuario,
            RemoverUsuario removerUsuario)
        {
            _asyncMap = asyncMap;
            _usuarioRepository = usuarioRepository;
            _criarUsuario = criarUsuario;
            _atualizarUsuario = atualizarUsuario;
            _removerUsuario = removerUsuario;
        }

        #region Criação

        public async Task<Resultado<UsuarioRequest>> CriarAsync(UsuarioRequest request)
            => await _criarUsuario.ExecutarAsync(request);

        #endregion

        #region Atualização

        public async Task<Resultado<UsuarioRequest>> AtualizarAsync(UsuarioRequest request)
            => await _atualizarUsuario.ExecutarAsync(request);

        #endregion

        #region Consultas

        public async Task<Resultado<List<UsuarioDTO>>> ObterTodosAsync()
        {
            var entities = await _usuarioRepository.ObterUsuariosAsync();
            var dtos = await _asyncMap.MapToListDTOAsync([.. entities]);
            return Resultado<List<UsuarioDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<UsuarioDTO>> ObterPorCodigoAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<UsuarioDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var entidade = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);

            if (entidade == null)
                return Resultado<UsuarioDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dto = await _asyncMap.MapToDTOAsync(entidade);
            return Resultado<UsuarioDTO>.Sucesso(dto);
        }

        #endregion

        #region Remoção

        public async Task<Resultado> RemoverAsync(string codigo)
            => await _removerUsuario.ExecutarAsync(codigo);

        #endregion
    }
}
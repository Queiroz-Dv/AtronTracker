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
    public class UsuarioService : IUsuarioService
    {
        // Mapeamento
        private readonly IAsyncMap<UsuarioDTO, Usuario> _asyncMap;

        // Dados
        private readonly IUsuarioRepository _usuarioRepository;

        // Casos de Uso do módulo
        private readonly CriarUsuario _criarUsuario;
        private readonly AtualizarUsuario _atualizarUsuario;
        private readonly RemoverUsuario _removerUsuario;
        private readonly DesativarUsuario _desativarUsuario;
        private readonly ReativarUsuario _reativarUsuario;
        private readonly SolicitarReativacao _solicitarReativacao;

        // Casos de Uso - E-mail
        private readonly AlterarEmail _alterarEmail;
        private readonly ConfirmarAlteracaoEmail _confirmarAlteracaoEmail;
        private readonly ReenviarConfirmacaoEmail _reenviarConfirmacaoEmail;

        public UsuarioService(
            IAsyncMap<UsuarioDTO, Usuario> asyncMap,
            IUsuarioRepository usuarioRepository,
            CriarUsuario criarUsuario,
            AtualizarUsuario atualizarUsuario,
            RemoverUsuario removerUsuario,
            DesativarUsuario desativarUsuario,
            ReativarUsuario reativarUsuario,
            SolicitarReativacao solicitarReativacao,
            AlterarEmail alterarEmail,
            ConfirmarAlteracaoEmail confirmarAlteracaoEmail,
            ReenviarConfirmacaoEmail reenviarConfirmacaoEmail)
        {
            _asyncMap = asyncMap;
            _usuarioRepository = usuarioRepository;
            _criarUsuario = criarUsuario;
            _atualizarUsuario = atualizarUsuario;
            _removerUsuario = removerUsuario;
            _desativarUsuario = desativarUsuario;
            _reativarUsuario = reativarUsuario;
            _solicitarReativacao = solicitarReativacao;
            _alterarEmail = alterarEmail;
            _confirmarAlteracaoEmail = confirmarAlteracaoEmail;
            _reenviarConfirmacaoEmail = reenviarConfirmacaoEmail;
        }

        public async Task<Resultado<UsuarioRequest>> CriarAsync(UsuarioRequest request)
            => await _criarUsuario.ExecutarAsync(request);

        public async Task<Resultado<UsuarioRequest>> AtualizarAsync(UsuarioRequest request)
            => await _atualizarUsuario.ExecutarAsync(request);

        public async Task<Resultado> RemoverAsync(string codigo)
            => await _removerUsuario.ExecutarAsync(codigo);

        public async Task<Resultado> DesativarAsync(string codigo)
            => await _desativarUsuario.ExecutarAsync(codigo);

        public async Task<Resultado> ReativarAsync(string email, string codigoReativacao)
            => await _reativarUsuario.ExecutarAsync(email, codigoReativacao);

        public async Task<Resultado> SolicitarReativacaoAsync(string email)
            => await _solicitarReativacao.ExecutarAsync(email);

        public async Task<Resultado> AlterarEmailAsync(string codigo, string emailNovo, string clienteUri) 
            => await _alterarEmail.ExecutarAsync(codigo, emailNovo, clienteUri);

        public async Task<Resultado> ConfirmarAlteracaoEmailAsync(string usuarioCodigo, string emailNovo, string token) 
            => await _confirmarAlteracaoEmail.ExecutarAsync(usuarioCodigo, emailNovo, token);

        public async Task<Resultado> ReenviarConfirmacaoEmailAsync(string codigo, string clientUri)
            => await _reenviarConfirmacaoEmail.ExecutarAsync(codigo, clientUri);

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
            if (entidade is null)
                return Resultado<UsuarioDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dto = await _asyncMap.MapToDTOAsync(entidade);
            return Resultado<UsuarioDTO>.Sucesso(dto);
        }       
    }
}
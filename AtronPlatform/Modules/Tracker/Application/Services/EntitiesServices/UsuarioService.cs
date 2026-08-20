using Application.DTO;
using Application.DTO.Request;
using Application.Interfaces.Services;
using Application.UseCases.UsuarioCases;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class UsuarioService(
        IToDtoMapper<Usuario, UsuarioDTO> mapper,
        IUsuarioRepository usuarioRepository,
        ObterUsuario obterUsuario,
        CriarUsuario criarUsuario,
        AtualizarUsuario atualizarUsuario,
        RemoverUsuario removerUsuario,
        DesativarUsuario desativarUsuario,
        ReativarUsuario reativarUsuario,
        SolicitarReativacao solicitarReativacao,
        AlterarEmail alterarEmail,
        ConfirmarAlteracaoEmail confirmarAlteracaoEmail,
        ReenviarConfirmacaoEmail reenviarConfirmacaoEmail) : IUsuarioService
    {
        private readonly IToDtoMapper<Usuario, UsuarioDTO> _mapper = mapper;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly ObterUsuario _obterUsuario = obterUsuario;
        private readonly CriarUsuario _criarUsuario = criarUsuario;
        private readonly AtualizarUsuario _atualizarUsuario = atualizarUsuario;
        private readonly RemoverUsuario _removerUsuario = removerUsuario;
        private readonly DesativarUsuario _desativarUsuario = desativarUsuario;
        private readonly ReativarUsuario _reativarUsuario = reativarUsuario;
        private readonly SolicitarReativacao _solicitarReativacao = solicitarReativacao;
        private readonly AlterarEmail _alterarEmail = alterarEmail;
        private readonly ConfirmarAlteracaoEmail _confirmarAlteracaoEmail = confirmarAlteracaoEmail;
        private readonly ReenviarConfirmacaoEmail _reenviarConfirmacaoEmail = reenviarConfirmacaoEmail;

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

        public async Task<Resultado> AlterarEmailAsync(string codigo, string emailNovo)
            => await _alterarEmail.ExecutarAsync(codigo, emailNovo);

        public async Task<Resultado> ConfirmarAlteracaoEmailAsync(string usuarioCodigo, string emailNovo, string token)
            => await _confirmarAlteracaoEmail.ExecutarAsync(usuarioCodigo, emailNovo, token);

        public async Task<Resultado> ReenviarConfirmacaoEmailAsync(string codigo)
            => await _reenviarConfirmacaoEmail.ExecutarAsync(codigo);

        public async Task<Resultado<List<UsuarioDTO>>> ObterTodosAsync()
        {
            var entities = await _usuarioRepository.ObterUsuariosAsync();
            var dtos = _mapper.MapToDtos(entities).ToList();
            return Resultado<List<UsuarioDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<UsuarioDTO>> ObterPorCodigoAsync(string codigo)
            => await _obterUsuario.ExecutarAsync(codigo);

        public async Task<Resultado<Usuario>> ObterUsuarioAtual()
            => await _obterUsuario.ObterAsync();
    }
}

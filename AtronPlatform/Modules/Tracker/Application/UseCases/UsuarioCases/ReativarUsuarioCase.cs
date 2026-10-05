using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Messaging;
using Shared.Application.Resources;
using Shared.Domain.Events.Auditoria;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.UseCases.UsuarioCases
{
    public class ReativarUsuarioCase(
        IUsuarioRepository usuarioRepository,
        IUsuarioIdentityRepository usuarioIdentityRepository,
        IEventBus eventBus)
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository = usuarioIdentityRepository;
        private readonly IEventBus _eventBus = eventBus;

        public async Task<Resultado> ExecutarAsync(string email, string codigoReativacao)
        {
            if (email.IsNullOrEmpty() || codigoReativacao.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var usuario = await _usuarioRepository.ObterInativoPorEmailAsync(email);
            if (usuario is null)
                return Resultado.Falha(UsuarioResource.Erro_UsuarioNaoEncontrado);

            if (!usuario.CodigoReativacao.Equals(codigoReativacao))
                return Resultado.Falha(UsuarioResource.ErroCodigoReativacaoInvalido);

            usuario.Inativo = false;
            usuario.CodigoReativacao = null;

            await _usuarioRepository.AtualizarUsuarioAsync(usuario);
            await _usuarioIdentityRepository.ReativarContaAsync(usuario.Codigo);

            await _eventBus.PublicarAsync(new AuditoriaAtualizadaEvent(
                usuario.Codigo,
                nameof(Domain.Entities.Usuario),
                "Usuário $({usuario.Codigo}) reativado em $({DateTime.Now:dd/MM/yyyy HH:mm})."
            ));

            return Resultado.Sucesso().AdicionarMensagem(UsuarioResource.MensagemContaReativada);
        }
    }
}
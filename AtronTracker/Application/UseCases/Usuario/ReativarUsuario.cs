using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace Application.UseCases.Usuario
{
    public class ReativarUsuario
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository;
        private readonly IAuditoriaService _auditoriaService;

        private const string UsuarioContexto = "Usuario";

        public ReativarUsuario(
            IUsuarioRepository usuarioRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IAuditoriaService auditoriaService)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _auditoriaService = auditoriaService;
        }

        public async Task<Resultado> ExecutarAsync(string email, string codigoReativacao)
        {
            if (email.IsNullOrEmpty() || codigoReativacao.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var usuario = await _usuarioRepository.ObterInativoPorEmailAsync(email);
            if (usuario is null)
                return Resultado.Falha(UsuarioResource.Erro_UsuarioNaoEncontrado);

            if (usuario.CodigoReativacao != codigoReativacao.ToUpper())
                return Resultado.Falha(UsuarioResource.ErroCodigoReativacaoInvalido);

            usuario.Inativo = false;
            usuario.CodigoReativacao = null;

            await _usuarioRepository.AtualizarUsuarioAsync(usuario);
            await _usuarioIdentityRepository.ReativarContaAsync(usuario.Codigo);

            await _auditoriaService.AtualizarServiceAsync(new AuditoriaDTO
            {
                CodigoRegistro = usuario.Codigo,
                Contexto = UsuarioContexto,
                Historico = new HistoricoDTO
                {
                    CodigoRegistro = usuario.Codigo,
                    Contexto = UsuarioContexto,
                    Descricao = $"Usuário {usuario.Codigo} reativado em {DateTime.Now:dd/MM/yyyy HH:mm}."
                }
            });

            return Resultado.Sucesso().AdicionarMensagem(UsuarioResource.MensagemContaReativada);
        }
    }
}

using Application.Email.Compositores;
using Application.Email.Models;
using Application.Extensions;
using Application.Interfaces.Services;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System;
using System.Threading.Tasks;
using UsuarioEntity = Domain.Entities.Usuario;

namespace Application.UseCases.UsuarioCases
{
    public class ReenviarConfirmacaoEmail(
        IUsuarioRepository usuarioRepository,
        IConfirmacaoEmailRepository confirmacaoEmailRepository,
        IConfirmacaoEmailCodigoService confirmacaoEmailCodigoService,
        IEmailService emailService,
        IAcessoEmailCompositor emailCompositor,
        IEnderecoFrontendService enderecoFrontendService)
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IConfirmacaoEmailRepository _confirmacaoEmailRepository = confirmacaoEmailRepository;
        private readonly IConfirmacaoEmailCodigoService _confirmacaoEmailCodigoService = confirmacaoEmailCodigoService;
        private readonly IEmailService _emailService = emailService;
        private readonly IAcessoEmailCompositor _emailCompositor = emailCompositor;
        private readonly IEnderecoFrontendService _enderecoFrontendService = enderecoFrontendService;
        private const int ValidadeConfirmacaoEmailEmHoras = 24;
        private static readonly TimeSpan IntervaloMinimoReenvio = TimeSpan.FromMinutes(2);

        public async Task<Resultado> ExecutarAsync(string codigoUsuario)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigoUsuario);
            return await ReenviarAsync(usuario);
        }

        public async Task<Resultado> ExecutarPorIdentificadorAsync(string identificador)
        {
            if (string.IsNullOrWhiteSpace(identificador))
                return RespostaPublica();

            var usuario = identificador.IdentifierIsEmail()
                ? await _usuarioRepository.ObterUsuarioGeralPorEmailAsync(identificador)
                : await _usuarioRepository.ObterUsuarioGeralPorCodigoAsync(identificador);

            await ReenviarAsync(usuario);
            return RespostaPublica();
        }

        private async Task<Resultado> ReenviarAsync(UsuarioEntity usuario)
        {
            if (usuario == null)
                return Resultado.Falha(UsuarioResource.Erro_UsuarioNaoEncontrado);

            if (usuario.Inativo)
                return Resultado.Falha(UsuarioResource.Erro_UsuarioInativo);

            if (usuario.EmailConfirmado)
                return Resultado.Falha(EmailResource.Erro_EmailConfirmado);

            var confirmacaoAtiva = await _confirmacaoEmailRepository
                .ObterAtivaPorUsuarioAsync(usuario.Codigo);
            var agora = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            if (confirmacaoAtiva is not null &&
                agora - confirmacaoAtiva.CriadoEm < IntervaloMinimoReenvio)
            {
                return RespostaPublica();
            }

            var confirmacao = _confirmacaoEmailCodigoService.CriarDadosConfirmacao(usuario.Codigo, ValidadeConfirmacaoEmailEmHoras);
            var gravado = await _confirmacaoEmailRepository.GravarOuSubstituirAsync(confirmacao.ConfirmacaoEmail);
            if (!gravado)
                return Resultado.Falha(EmailResource.Erro_CriarCodigoDeConfirmacao);

            var baseUri = _enderecoFrontendService.ObterUriBase();
            var link = $"{baseUri}/confirmar-email?usuarioCodigo={usuario.Codigo}";

            try
            {
                var email = _emailCompositor.ComporConfirmacaoCadastro(new ConfirmacaoCadastroEmailParametros(
                    usuario.Email,
                    usuario.Nome,
                    confirmacao.Identificador,
                    link,
                    ValidadeConfirmacaoEmailEmHoras));

                if (email.TeveFalha)
                    return Resultado.Falha(email.Messages);

                var resultado = await _emailService.EnviarAsync(email.Dados);

                return resultado.TeveFalha
                    ? Resultado.Falha(resultado.Messages)
                    : Resultado.Sucesso(EmailResource.Mensagem_EnvioConfirmacaoEmail);
            }
            catch
            {
                return Resultado.Falha(AuthResource.Erro_EnvioEmailObrigatorio);
            }
        }

        private static Resultado RespostaPublica()
            => Resultado.Sucesso(EmailResource.Mensagem_EnvioConfirmacaoEmail);

    }
}

using Application.DTO;
using Application.EmailCompositor.Compositores;
using Application.Extensions;
using Application.Interfaces.Services;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public class ConfirmarEmailContaUsuarioCase(
        IConfirmacaoEmailRepository ConfirmacaoRepository,
        IConfirmacaoEmailCodigoService ConfirmacaoCodigoService,
        IUsuarioRepository UsuarioRepository,
        IAcessoEmailCompositor EmailCompositor,
        IEmailService EmailService
        )
    {
        private const int MaximoTentativasConfirmacao = 5;

        public async Task<Resultado> ExecutarAsync(string codigoUsuario, string identificador)
        {
            var codigo = codigoUsuario.NormalizeUserCodeIdentifier();
            var id = identificador.NormalizeIdentifier();

            if (codigo.IsNullOrEmpty() || id.IsNullOrEmpty())
                return Resultado.Falha(AuthResource.Erro_DadosConfirmacaoObrigatorios);

            var confirmacao = await ConfirmacaoRepository.ObterAtivaPorUsuarioAsync(codigo);
            if (confirmacao.IsNullable() || confirmacao.TentativasFalhas >= MaximoTentativasConfirmacao)
                return Resultado.Falha(AuthResource.Erro_FalhaConfirmarEmail);

            if (!ConfirmacaoCodigoService.ConfirmacaoValida(confirmacao, codigo, id))
            {
                await ConfirmacaoRepository.RegistrarTentativaFalhaAsync(confirmacao.Id);
                return Resultado.Falha(AuthResource.Erro_FalhaConfirmarEmail);
            }

            if (!await UsuarioRepository.ConfirmarEmailAsync(codigo))
                return Resultado.Falha(AuthResource.Erro_FalhaConfirmarEmail);

            await ConfirmacaoRepository.MarcarConfirmadaAsync(confirmacao.Id);

            var resultado = Resultado.Sucesso(AuthResource.Mensagem_EmailConfirmado);
            var usuario = await UsuarioRepository.ObterUsuarioPorCodigoAsync(codigo);

            if (!usuario.IsNullable() && !usuario.Email.IsNullOrEmpty())
            {
                try
                {
                    var parametrosEmailDto = new ParametrosEmailDTO()
                    {
                        Destinatario = usuario.Email,
                        Email = usuario.Email,
                        UsuarioNome = usuario.Nome,
                    };

                    var email = EmailCompositor.ComporConfirmacaoConcluida(parametrosEmailDto);
                    if (email.TeveFalha)
                        resultado.AdicionarAviso(string.Join(" | ", email.Messages.Select(m => m.Descricao)));

                    if ((await EmailService.EnviarAsync(email.Dados)).TeveFalha)
                        resultado.AdicionarAviso(AuthResource.Aviso_ConfirmacaoConcluidaEmailNaoEnviado);
                }
                catch
                {
                    resultado.AdicionarAviso(AuthResource.Aviso_ConfirmacaoConcluidaEmailNaoEnviado);
                }
            }
            return Resultado.Sucesso(resultado.Messages);
        }
    }
}
using Application.Email.Compositores;
using Application.Records.Usuario;
using Domain.Entities;
using Domain.Extensions;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases;

public sealed class EnviarEmailConfirmacaoCadastroCase(
    IAcessoEmailCompositor emailCompositor,
    IEmailService emailService)
{
    public async Task<Resultado> ExecutarAsync(
        Usuario usuario,
        ConfirmacaoCadastroCriadaRecord confirmacao)
    {
        try
        {
            var email = emailCompositor.ComporConfirmacaoCadastro(
                new ConfirmacaoCadastroEmailParametrosRecord(
                    usuario.Email,
                    usuario.ObterNome(),
                    confirmacao.Identificador,
                    confirmacao.Link,
                    confirmacao.ValidadeHoras));

            if (email.TeveFalha)
                return Resultado.Falha(email.Messages);

            var envio = await emailService.EnviarAsync(email.Dados);
            return envio.TeveFalha
                ? Resultado.Falha(AuthResource.Aviso_CadastroCriadoEmailNaoEnviado)
                : Resultado.Sucesso();
        }
        catch
        {
            return Resultado.Falha(AuthResource.Aviso_CadastroCriadoEmailNaoEnviado);
        }
    }
}
using Application.Email.Compositores;
using Domain.Entities;
using Domain.Extensions;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases;

public sealed class EnviarEmailConfirmacaoConcluidaCase(
    IAcessoEmailCompositor emailCompositor,
    IEmailService emailService)
{
    public async Task<Resultado> ExecutarAsync(Usuario usuario)
    {
        if (usuario.Email.IsNullOrEmpty())
            return Resultado.Sucesso();

        try
        {
            var email = emailCompositor.ComporConfirmacaoConcluida(
                usuario.Email,
                usuario.ObterNome());

            if (email.TeveFalha)
                return Resultado.Falha(email.Messages);

            var envio = await emailService.EnviarAsync(email.Dados);
            return envio.TeveFalha
                ? Resultado.Falha(AuthResource.Aviso_ConfirmacaoConcluidaEmailNaoEnviado)
                : Resultado.Sucesso();
        }
        catch
        {
            return Resultado.Falha(AuthResource.Aviso_ConfirmacaoConcluidaEmailNaoEnviado);
        }
    }
}

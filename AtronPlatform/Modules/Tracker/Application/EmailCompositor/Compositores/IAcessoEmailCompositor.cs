using Application.DTO;
using Shared.Application.DTOS.Requests;
using Shared.Domain.ValueObjects;

namespace Application.EmailCompositor.Compositores
{
    public interface IAcessoEmailCompositor
    {
        Resultado<EmailRequest> ComporConfirmacaoCadastro(ParametrosEmailDTO parametros);

        Resultado<EmailRequest> ComporRecuperacaoSenha(ParametrosEmailDTO parametros);

        Resultado<EmailRequest> ComporConfirmacaoConcluida(ParametrosEmailDTO parametros);

        Resultado<EmailRequest> ComporPrimeiroAcesso(ParametrosEmailDTO parametros);

        Resultado<EmailRequest> ComporAlteracaoEmail(ParametrosEmailDTO parametros);

        Resultado<EmailRequest> ComporReativacaoConta(ParametrosEmailDTO parametros);
    }
}
using Shared.Application.DTOS.Requests;
using Shared.Domain.ValueObjects;
using Application.Records.Usuario;

namespace Application.Email.Compositores;

public interface IAcessoEmailCompositor
{    
    Resultado<EmailRequest> ComporConfirmacaoCadastro(ConfirmacaoCadastroEmailParametrosRecord parametros);
    
    Resultado<EmailRequest> ComporRecuperacaoSenha(RecuperacaoSenhaEmailParametrosRecord parametros);

    Resultado<EmailRequest> ComporConfirmacaoConcluida(string destinatario, string nome);
    
    Resultado<EmailRequest> ComporPrimeiroAcesso(PrimeiroAcessoEmailParametrosRecord parametros);

    Resultado<EmailRequest> ComporAlteracaoEmail(string destinatario, string nome, string link);

    Resultado<EmailRequest> ComporReativacaoConta(string destinatario, string nome, string codigo);
}

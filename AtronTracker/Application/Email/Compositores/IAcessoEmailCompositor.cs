using Shared.Application.DTOS.Requests;
using Shared.Domain.ValueObjects;
using Application.Email.Models;

namespace Application.Email.Compositores;

public interface IAcessoEmailCompositor
{    
    Resultado<EmailRequest> ComporConfirmacaoCadastro(ConfirmacaoCadastroEmailParametros parametros);
    
    Resultado<EmailRequest> ComporRecuperacaoSenha(RecuperacaoSenhaEmailParametros parametros);

    Resultado<EmailRequest> ComporConfirmacaoConcluida(string destinatario, string nome);
    
    Resultado<EmailRequest> ComporPrimeiroAcesso(PrimeiroAcessoEmailParametros parametros);

    Resultado<EmailRequest> ComporAlteracaoEmail(string destinatario, string nome, string link);

    Resultado<EmailRequest> ComporReativacaoConta(string destinatario, string nome, string codigo);
}

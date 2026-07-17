using Shared.Application.DTOS.Requests;

namespace Application.Email.Compositores;

public interface IAcessoEmailCompositor
{
    EmailRequest ComporConfirmacaoCadastro(string destinatario, string nome, string codigo, string link, int validadeHoras);

    EmailRequest ComporRecuperacaoSenha(string destinatario, string nome, string link, int validadeHoras);

    EmailRequest ComporConfirmacaoConcluida(string destinatario, string nome);

    EmailRequest ComporPrimeiroAcesso(string destinatario, string nome, string link, int validadeHoras);

    EmailRequest ComporAlteracaoEmail(string destinatario, string nome, string link);

    EmailRequest ComporReativacaoConta(string destinatario, string nome, string codigo);
}

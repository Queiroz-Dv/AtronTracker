using Application.DTO;
using Application.EmailCompositor.UseCases;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Shared.Domain.ValueObjects;

namespace Application.EmailCompositor.Compositores
{
    public sealed class AcessoEmailCompositor(IEmailTemplateRenderer renderer) : IAcessoEmailCompositor
    {
        private readonly IEmailTemplateRenderer _renderer = renderer;

        public Resultado<EmailRequest> ComporConfirmacaoCadastro(ParametrosEmailDTO parametros)
        {
            return ComporrEmailConfirmacaoCadastroCase.Executar(parametros, _renderer);
        }

        public Resultado<EmailRequest> ComporRecuperacaoSenha(ParametrosEmailDTO parametros)
        {
            return ComporEmailRecuperacaoSenhaCase.Executar(parametros, _renderer);
        }

        public Resultado<EmailRequest> ComporConfirmacaoConcluida(ParametrosEmailDTO parametros)
        {
            return ComporEmailConfirmacaoConcluidaCase.Executar(parametros, _renderer);
        }

        public Resultado<EmailRequest> ComporPrimeiroAcesso(ParametrosEmailDTO parametros)
        {
            return ComporEmailPrimeiroAcessoCase.Executar(parametros, _renderer);
        }

        public Resultado<EmailRequest> ComporAlteracaoEmail(ParametrosEmailDTO parametrosEmailDTO)
        {
            return ComporAlteracaoEmailCase.Executar(parametrosEmailDTO, _renderer);
        }

        public Resultado<EmailRequest> ComporReativacaoConta(ParametrosEmailDTO parametros)
        {
            return ComporReativacaoContaCase.Executar(parametros, _renderer);
        }
    }
}
using Application.DTO;
using Shared.Application.DTOS.Email;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.EmailCompositor.Compositores
{
    public sealed class AcessoEmailCompositor(IEmailTemplateRenderer renderer) : IAcessoEmailCompositor
    {
        private readonly IEmailTemplateRenderer _renderer = renderer;

        public Resultado<EmailRequest> ComporConfirmacaoCadastro(ParametrosEmailDTO parametros)
        {
            var dto = new EmailTemplateDTO(
                EmailResource.Arquivo_ConfirmacaoCadastroHtml,
                EmailResource.Assunto_ConfirmeCadastro,
                EmailResource.Titulo_ConfirmacaoCadastro);

            var templateDefinition = dto.CriarTemplate<AcessoEmailCompositor>();
            return _renderer.Renderizar(templateDefinition, parametros, new[] { parametros.Destinatario });
        }

        public Resultado<EmailRequest> ComporRecuperacaoSenha(ParametrosEmailDTO parametros)
        {
            var dto = new EmailTemplateDTO()
            {
                Arquivo = EmailResource.Arquivo_RecuperacaoSenhaHtml,
                Assunto = EmailResource.Assunto_RecuperacaoSenha,
                Titulo = EmailResource.Titulo_RecuperacaoSenha
            };

            var templateDefinition = dto.CriarTemplate<AcessoEmailCompositor>();
            return _renderer.Renderizar(templateDefinition, parametros, new[] { parametros.Destinatario });
        }

        public Resultado<EmailRequest> ComporConfirmacaoConcluida(ParametrosEmailDTO parametros)
        {
            var dto = new EmailTemplateDTO()
            {
                Arquivo = EmailResource.Arquivo_ConfirmaaoConcluidaHtml,
                Assunto = EmailResource.Assunto_EmailConfirmado,
                Titulo = EmailResource.Titulo_EmailConfirmado
            };

            var templateDefinition = dto.CriarTemplate<AcessoEmailCompositor>();
            return _renderer.Renderizar(templateDefinition, parametros, new[] { parametros.Destinatario });
        }

        public Resultado<EmailRequest> ComporPrimeiroAcesso(ParametrosEmailDTO parametros)
        {
            var dto = new EmailTemplateDTO()
            {
                Arquivo = EmailResource.Arquivo_PrimeiroAcessoHtml,
                Assunto = EmailResource.Assunto_PrimeiroAcesso,
                Titulo = EmailResource.Titulo_PrimeiroAcesso
            };

            var templateDefinition = dto.CriarTemplate<AcessoEmailCompositor>();
            return _renderer.Renderizar(templateDefinition, parametros, new[] { parametros.Destinatario });
        }

        public Resultado<EmailRequest> ComporAlteracaoEmail(ParametrosEmailDTO parametros)
        {
            var dto = new EmailTemplateDTO()
            {
                Arquivo = EmailResource.Arquivo_AlteracaoEmailHtml,
                Assunto = EmailResource.Assunto_AlteracaoEmail,
                Titulo = EmailResource.Titulo_AlteracaoEmail
            };

            var templateDefinition = dto.CriarTemplate<AcessoEmailCompositor>();
            return _renderer.Renderizar(templateDefinition, parametros, new[] { parametros.Destinatario });
        }

        public Resultado<EmailRequest> ComporReativacaoConta(ParametrosEmailDTO parametros)
        {
            var dto = new EmailTemplateDTO()
            {
                Arquivo = EmailResource.Arquivo_ReativacaoContaHtml,
                Assunto = EmailResource.Assunto_ReativacaoConta,
                Titulo = EmailResource.Titulo_ReativacaoConta
            };

            var templateDefinition = dto.CriarTemplate<AcessoEmailCompositor>();
            return _renderer.Renderizar(templateDefinition, parametros, new[] { parametros.Destinatario });
        }
    }
}
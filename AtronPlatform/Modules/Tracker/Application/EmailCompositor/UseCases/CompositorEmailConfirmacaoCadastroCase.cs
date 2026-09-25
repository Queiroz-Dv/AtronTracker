using Application.DTO;
using Application.Statics;
using Shared.Application.DTOS.Email;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;

namespace Application.EmailCompositor.UseCases
{
    internal sealed class ComporrEmailConfirmacaoCadastroCase
    {
        internal static Resultado<EmailRequest> Executar(ParametrosEmailDTO parametros, IEmailTemplateRenderer renderer)
        {
            var dto = new EmailTemplateDTO(
                EmailResource.Arquivo_ConfirmacaoCadastroHtml,
                EmailResource.Assunto_ConfirmeCadastro,
                EmailResource.Titulo_ConfirmacaoCadastro);

            var templateDefinition = EmailTemplateBuilder.ObterTemplateDefinition(dto);
            return renderer.Renderizar(templateDefinition, parametros, new[] { parametros.Destinatario });
        }
    }
}
using Application.DTO;
using Application.Statics;
using Shared.Application.DTOS.Email;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;

namespace Application.EmailCompositor.UseCases
{
    internal sealed class ComporEmailRecuperacaoSenhaCase
    {
        internal static Resultado<EmailRequest> Executar(ParametrosEmailDTO parametros, IEmailTemplateRenderer renderer)
        {
            var dto = new EmailTemplateDTO()
            {
                Arquivo = EmailResource.Arquivo_RecuperacaoSenhaHtml,
                Assunto = EmailResource.Assunto_RecuperacaoSenha,
                Titulo = EmailResource.Titulo_RecuperacaoSenha
            };

            var templateDefinition = EmailTemplateBuilder.ObterTemplateDefinition(dto);
            return renderer.Renderizar(templateDefinition, parametros, new[] { parametros.Destinatario });
        }
    }
}
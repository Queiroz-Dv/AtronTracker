using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Validations;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Text.RegularExpressions;

namespace Shared.Application.Email.Rendering;

public sealed class EmailTemplateRenderer : IEmailTemplateRenderer
{
    private Regex TokenPattern = new(@"\{\{(?<nome>[A-Za-z][A-Za-z0-9]*)\}\}");

    public Resultado<EmailRequest> Renderizar<TModel>(
        EmailTemplateDefinition template,
        TModel model,
        IEnumerable<string> destinatarios)
        where TModel : class
    {
        try
        {
            var (Bag, EmailsDestino) = TemplateValidation.Validar(template, model, destinatarios);
            var messages = Bag.Messages;
            if (messages.TemErros())
                return Resultado<EmailRequest>.Falhas(messages);

            var templateConteudo = TemplateFactoryRenderer.Executar(template.TemplateAssembly, template.TemplateResourceName);
            if (templateConteudo.TeveFalha)
                return Resultado<EmailRequest>.Falhas(templateConteudo.Messages);

            var conteudoRenderizado = TemplateFactoryRenderer.ObterCamposRenderizados(templateConteudo.Dados, model, TokenPattern);
            if (conteudoRenderizado.TeveFalha)
                return Resultado<EmailRequest>.Falhas(conteudoRenderizado.Messages);

            var templateBase = TemplateFactoryRenderer.Executar(typeof(EmailTemplateRenderer).Assembly, EmailTemplateResourceNames.Base);
            if (templateBase.TeveFalha)
                return Resultado<EmailRequest>.Falhas(templateBase.Messages);

            var html = TemplateFactoryRenderer.ObterBaseRenderizada(templateBase.Dados, template.Titulo, conteudoRenderizado.Dados, TokenPattern);
            if (html.TeveFalha)
                return Resultado<EmailRequest>.Falhas(html.Messages);

            return Resultado<EmailRequest>.Sucesso(new EmailRequest
            {
                Assunto = template.Assunto,
                Mensagem = html.Dados,
                EmailsDestino = EmailsDestino
            });
        }
        catch
        {
            return Resultado<EmailRequest>.Falha(EmailResource.Erro_TemplateRenderizacao);
        }
    }       
}
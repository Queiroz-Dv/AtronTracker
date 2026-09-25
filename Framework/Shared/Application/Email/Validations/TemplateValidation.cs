using Shared.Application.Email.Rendering;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Shared.Application.Email.Validations
{
    public class TemplateValidation
    {
        public static (NotificationBag Bag, List<string> EmailsDestino) Validar<TModel>(
            EmailTemplateDefinition template,
            TModel model,
            IEnumerable<string> destinatarios)
        {
            var notificationBag = new NotificationBag();

            if (template.IsNullable())
                notificationBag.AdicionarErro(EmailResource.Erro_TemplateDefinicaoObrigatoria);

            if (model.IsNullable())
                notificationBag.AdicionarErro(EmailResource.Erro_TemplateModeloObrigatorio);

            if (destinatarios.IsNullable())
                notificationBag.AdicionarErro(EmailResource.Erro_TemplateDestinatarioObrigatorio);

            if (template.Assunto.IsNullOrEmpty())
                notificationBag.AdicionarErro(EmailResource.Erro_TemplateAssuntoObrigatorio);

            if (template.Titulo.IsNullOrEmpty())
                notificationBag.AdicionarErro(EmailResource.Erro_TemplateTituloObrigatorio);


            var emailsDestino = destinatarios
                .Where(destinatario => !destinatario.IsNullOrEmpty())
                .Select(destinatario => destinatario.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (emailsDestino.Count == 0)
                notificationBag.AdicionarErro(EmailResource.Erro_TemplateDestinatarioObrigatorio);


            return (notificationBag, emailsDestino);
        }
    }
}
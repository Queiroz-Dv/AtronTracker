using Shared.Application.Email.Rendering;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Shared.Application.Email.Validations
{
    public class TemplateValidation
    {
        public static (Resultado Bag, List<string> EmailsDestino) Validar<TModel>(
            EmailTemplateDefinition template,
            TModel model,
            IEnumerable<string> destinatarios)
        {
            var bag = new Resultado();

            if (template.IsNullable())
                bag.AdicionarErro(EmailResource.Erro_TemplateDefinicaoObrigatoria);

            if (model.IsNullable())
                bag.AdicionarErro(EmailResource.Erro_TemplateModeloObrigatorio);

            if (destinatarios.IsNullable())
                bag.AdicionarErro(EmailResource.Erro_TemplateDestinatarioObrigatorio);

            if (template.Assunto.IsNullOrEmpty())
                bag.AdicionarErro(EmailResource.Erro_TemplateAssuntoObrigatorio);

            if (template.Titulo.IsNullOrEmpty())
                bag.AdicionarErro(EmailResource.Erro_TemplateTituloObrigatorio);


            var emailsDestino = destinatarios
                .Where(destinatario => !destinatario.IsNullOrEmpty())
                .Select(destinatario => destinatario.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (emailsDestino.Count == 0)
                bag.AdicionarErro(EmailResource.Erro_TemplateDestinatarioObrigatorio);


            return (bag, emailsDestino);
        }
    }
}

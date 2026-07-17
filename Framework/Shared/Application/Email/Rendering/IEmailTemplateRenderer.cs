using Shared.Application.DTOS.Requests;

namespace Shared.Application.Email.Rendering;

public interface IEmailTemplateRenderer
{
    EmailRequest Renderizar<TModel>(
        EmailTemplateDefinition template,
        TModel model,
        IEnumerable<string> destinatarios)
        where TModel : class;
}

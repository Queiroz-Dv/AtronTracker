using Shared.Application.DTOS.Requests;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Email.Rendering;

public interface IEmailTemplateRenderer
{
    Resultado<EmailRequest> Renderizar<TModel>(
        EmailTemplateDefinition template,
        TModel model,
        IEnumerable<string> destinatarios)
        where TModel : class;
}

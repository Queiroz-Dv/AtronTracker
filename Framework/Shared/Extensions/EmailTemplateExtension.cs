using Shared.Application.DTOS.Email;
using Shared.Application.Email.Rendering;

namespace Shared.Extensions;

public static class EmailTemplateExtension
{
    public static EmailTemplateDefinition CriarTemplate<TCompositor>(this EmailTemplateDTO dto)
    {
        return new EmailTemplateDefinition(
               typeof(TCompositor).Assembly,
               $"{PrefixoTemplateDTO.Prefixo}{dto.Arquivo}",
               dto.Assunto,
               dto.Titulo);
    }
}
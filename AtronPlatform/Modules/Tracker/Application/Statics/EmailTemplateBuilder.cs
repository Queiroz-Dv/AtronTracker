using Application.EmailCompositor.Compositores;
using Shared.Application.DTOS.Email;
using Shared.Application.Email.Rendering;
using Shared.Extensions;

namespace Application.Statics
{
    public class EmailTemplateBuilder
    {
        public static EmailTemplateDefinition ObterTemplateDefinition(EmailTemplateDTO dto)
        {
            // Foi criado pra não precisar informar em cada classe
            return dto.CriarTemplate<AcessoEmailCompositor>();
        }
    }
}
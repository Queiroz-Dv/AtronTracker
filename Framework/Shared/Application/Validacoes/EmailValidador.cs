using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Validacoes
{
    public class EmailValidador : IValidador<EmailRequest>
    {
        public IList<NotificationMessage> Validar(EmailRequest entity)
        {
            var context = new NotificationBag();

            if (entity.EmailsDestino.Count < 0)
            {
                context.AdicionarErroCampoObrigatorio(nameof(entity.EmailsDestino));
            }

            if (string.IsNullOrWhiteSpace(entity.Assunto))
            {
                context.AdicionarErroCampoObrigatorio(nameof(entity.Assunto));
            }

            if (string.IsNullOrWhiteSpace(entity.Mensagem))
            {
                context.AdicionarErroCampoObrigatorio(nameof(entity.Assunto));
            }

            return context.Messages.ToList();
        }
    }
}
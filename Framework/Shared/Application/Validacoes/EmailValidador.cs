using Shared.Application.DTOS.Requests;
using Shared.Application.Services;
using Shared.Application.Resources;
using Shared.Extensions.RegraExtensions;

namespace Shared.Application.Validacoes
{
    public class EmailValidador : Validador<EmailRequest>
    {
        public EmailValidador()
        {
            RegraPara(x => x.EmailsDestino)
                .DeveSer(emails => emails != null && emails.Count > 0)
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, nameof(EmailRequest.EmailsDestino)));

            RegraPara(x => x.Assunto)
                .NaoVazio()
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, nameof(EmailRequest.Assunto)));

            RegraPara(x => x.Mensagem)
                .NaoVazio()
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, nameof(EmailRequest.Mensagem)));
        }
    }
}

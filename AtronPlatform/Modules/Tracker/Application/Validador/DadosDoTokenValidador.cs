using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;

namespace Application.Validador
{
    public class DadosDoTokenValidador : IValidador<DadosDoTokenDTO>
    {
        public IEnumerable<NotificationMessage> Validar(DadosDoTokenDTO entity)
        {
            var context = new NotificationBag();

            if (entity == null)
            {
                context.AdicionarErro(NotificacoesPadronizadas.ErroRegistroNulo);
            }
            else
            {
                if (entity.UsuarioCodigo.IsNullOrEmpty())
                {
                context.AdicionarErro(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, AuthResource.Campo_CodigoUsuario));
                    return [.. context.Messages];
                }

                if (entity.UsuarioCodigo.Length > 10)
                {
                    context.AdicionarErro(UsuarioResource.ErroCodigoLongo);
                }

                if (entity.UsuarioCodigo.Length < 3)
                {
                    context.AdicionarErro(UsuarioResource.ErroCodigoPequeno);
                }

            }

            return [.. context.Messages];
        }
    }
}

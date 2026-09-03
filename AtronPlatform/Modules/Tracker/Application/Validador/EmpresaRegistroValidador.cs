using Application.DTO.Request;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;

namespace Application.Validador
{
    public sealed class EmpresaRegistroValidador : IValidador<EmpresaRegistroRequest>
    {
        public IEnumerable<NotificationMessage> Validar(EmpresaRegistroRequest? request)
        {
            var notificacoes = new NotificationBag();
            if (request.IsNullable())
            {
                notificacoes.AdicionarErro(NotificacoesPadronizadas.ErroRegistroNulo);
                return notificacoes.Messages;
            }

            notificacoes.ValidarCampo(request.Codigo, 3, 25, nameof(request.Codigo));
            notificacoes.ValidarCampo(request.NomeFantasia, 3, 150, nameof(request.NomeFantasia));
            notificacoes.ValidarCampo(request.Endereco, 3, 200, nameof(request.Endereco));
            notificacoes.ValidarCampo(request.Numero, 1, 20, nameof(request.Numero));
            notificacoes.ValidarCampo(request.Email, 3, 254, nameof(request.Email));
            notificacoes.ValidarEmail(request.Email, nameof(request.Email));

            return notificacoes.Messages;
        }
    }
}
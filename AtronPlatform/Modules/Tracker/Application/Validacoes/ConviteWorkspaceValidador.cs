using Application.DTO.Request;
using Application.Resources;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Collections.Generic;

namespace Application.Validacoes
{
    public sealed class ConviteWorkspaceValidador
        : IValidador<CriarConviteWorkspaceRequest>
    {
        public IEnumerable<NotificationMessage> Validar(
            CriarConviteWorkspaceRequest? convite)
        {
            var notificacoes = new NotificationBag();
            if (convite.IsNullable())
            {
                notificacoes.AdicionarErro(WorkspaceResource.Erro_ConviteInvalido);
                return notificacoes.Messages;
            }

            if (convite.WorkspaceId <= 0
                || convite.RemetenteCodigo.IsNullOrEmpty()
                || convite.IdentificadorHash.IsNullOrEmpty()
                || convite.ExpiraEm <= DateTime.UtcNow.SemTimezone())
            {
                notificacoes.AdicionarErro(WorkspaceResource.Erro_ConviteInvalido);
            }

            return notificacoes.Messages;
        }
    }
}

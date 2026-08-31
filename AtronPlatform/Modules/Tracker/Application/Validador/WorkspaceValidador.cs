using Application.DTO.Request;
using Application.Resources;
using Domain.Enums;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Collections.Generic;

namespace Application.Validador;

public sealed class WorkspaceValidador : IValidador<CriarWorkspaceInicialRequest>
{
    public IEnumerable<NotificationMessage> Validar(CriarWorkspaceInicialRequest? workspace)
    {
        var notificacoes = new NotificationBag();

        if (workspace.IsNullable())
        {
            notificacoes.AdicionarErro(WorkspaceResource.Erro_RegistroNulo);
            return notificacoes.Messages;
        }

        if (workspace.Nome.IsNullOrEmpty())
            notificacoes.AdicionarErro(WorkspaceResource.Erro_NomeObrigatorio);

        if (workspace.Nome.Length > 150)
            notificacoes.AdicionarErro(WorkspaceResource.Erro_NomeLongo);

        if (!Enum.IsDefined(workspace.Tipo))
            notificacoes.AdicionarErro(WorkspaceResource.Erro_TipoInvalido);

        if (workspace.UsuarioCodigo.IsNullOrEmpty())
            notificacoes.AdicionarErro(WorkspaceResource.Erro_UsuarioCodigoObrigatorio);

        if (workspace.Tipo == TipoWorkspace.Empresa
            && workspace.EmpresaCodigo.IsNullOrEmpty())
            notificacoes.AdicionarErro(WorkspaceResource.Erro_EmpresaCodigoObrigatorio);

        if (workspace.Tipo != TipoWorkspace.Empresa
            && workspace.EmpresaCodigo is not null)
            notificacoes.AdicionarErro(WorkspaceResource.Erro_EmpresaNaoPermitida);

        return notificacoes.Messages;
    }
}
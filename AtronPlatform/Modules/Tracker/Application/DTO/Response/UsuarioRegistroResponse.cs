using Shared.Domain.ValueObjects;
using System.Collections.Generic;

namespace Application.DTO.Response;

public sealed record UsuarioRegistroResponse(
    string UsuarioCodigo,
    WorkspaceInicialResponse Workspace,
    IReadOnlyCollection<NotificationMessage> Mensagens);

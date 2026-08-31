using System;

namespace Application.DTO.Response;

public sealed record ConviteWorkspaceResponse(
    WorkspaceInicialResponse Workspace,
    string RemetenteCodigo,
    DateTime ExpiraEm);

public sealed record ConviteWorkspaceCriadoResponse(
    string Link,
    DateTime ExpiraEm);

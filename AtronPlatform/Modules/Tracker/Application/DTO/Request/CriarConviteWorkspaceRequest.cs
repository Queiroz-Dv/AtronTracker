using System;

namespace Application.DTO.Request;

public sealed record CriarConviteWorkspaceRequest(
    int WorkspaceId,
    string RemetenteCodigo,
    string IdentificadorHash,
    DateTime ExpiraEm);

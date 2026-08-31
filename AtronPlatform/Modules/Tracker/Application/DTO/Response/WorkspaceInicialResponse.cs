using Domain.Enums;
namespace Application.DTO.Response;

public sealed record WorkspaceInicialResponse(
    int Id,
    string Nome,
    TipoWorkspace Tipo,
    string? EmpresaCodigo);
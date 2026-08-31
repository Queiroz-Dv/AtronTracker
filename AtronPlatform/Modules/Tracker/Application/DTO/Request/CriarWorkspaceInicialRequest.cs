using Domain.Enums;
namespace Application.DTO.Request;

public sealed record CriarWorkspaceInicialRequest(
    string Nome,
    TipoWorkspace Tipo,
    string UsuarioCodigo,
    string? EmpresaCodigo = null);

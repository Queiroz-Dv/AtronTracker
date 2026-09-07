using Domain.Enums;

namespace Application.DTO.Request;

public sealed class WorkspaceRegistroRequest
{
    public string Nome { get; set; } = string.Empty;
    public TipoWorkspace Tipo { get; set; }
}

using Domain.Enums;
using System.Text.Json.Serialization;

namespace Application.DTO.Request;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class WorkspaceRegistroRequest
{
    public string Nome { get; set; } = string.Empty;
    public TipoWorkspace Tipo { get; set; }
    public EmpresaRegistroRequest? Empresa { get; set; }
}

using System.Text.Json.Serialization;

namespace Application.DTO.Request;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class WorkspaceRegistroRequest
{
    public string Codigo { get; set; }
    public string Descricao { get; set; }
}
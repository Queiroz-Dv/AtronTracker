using System.Text.Json.Serialization;

namespace Application.DTO.Request;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class EmpresaRegistroRequest
{
    public string Codigo { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

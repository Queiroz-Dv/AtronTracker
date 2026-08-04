namespace Application.DTO.Request
{
    [System.Text.Json.Serialization.JsonUnmappedMemberHandling(
        System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow)]
    public class ConfirmarEmailRequest
    {
        public string UsuarioCodigo { get; set; }
        public string Identificador { get; set; }
    }
}

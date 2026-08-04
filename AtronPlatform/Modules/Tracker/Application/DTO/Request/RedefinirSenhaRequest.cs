namespace Application.DTO.Request
{
    [System.Text.Json.Serialization.JsonUnmappedMemberHandling(
        System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow)]
    public class RedefinirSenhaRequest
    {
        public string IdentificadorTemporario { get; set; }
        public string NovaSenha { get; set; }
        public string RepetirSenha { get; set; }
    }
}

namespace Application.DTO.Request
{
    [System.Text.Json.Serialization.JsonUnmappedMemberHandling(
        System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow)]
    public class ReenviarConfirmacaoEmailRequest
    {
        public string Identificador { get; set; }
    }
}

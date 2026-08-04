namespace Application.DTO.Request
{
    [System.Text.Json.Serialization.JsonUnmappedMemberHandling(
        System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow)]
    public class ReativarContaRequest
    {
        public string Email { get; set; }
        public string CodigoReativacao { get; set; }
    }
}

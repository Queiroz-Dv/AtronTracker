namespace Application.DTO.Request
{
    public class RedefinirSenhaRequest
    {
        public string IdentificadorTemporario { get; set; }
        public string NovaSenha { get; set; }
        public string RepetirSenha { get; set; }
    }
}

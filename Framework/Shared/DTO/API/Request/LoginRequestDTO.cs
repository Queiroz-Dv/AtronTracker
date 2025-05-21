namespace Shared.DTO.API.Request
{
    public class LoginRequestDTO
    {
        public string CodigoDoUsuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public bool Lembrar { get; set; }
    }
}
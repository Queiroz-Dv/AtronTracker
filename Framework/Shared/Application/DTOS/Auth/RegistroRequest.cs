namespace Shared.Application.DTOS.Auth
{
    public class RegistroRequest
    {
        public string CodigoDeAcesso { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }
    }
}

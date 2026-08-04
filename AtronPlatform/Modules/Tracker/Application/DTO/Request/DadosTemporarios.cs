using System;

namespace Application.DTO.Request
{
    public class DadosTemporarios
    {
        public string UsuarioCodigo { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime DataAlteracaoSenha { get; set; }
    }
}

using System;

namespace Application.DTO.Request
{
    public class UsuarioRequest
    {
        public string Codigo { get; set; }

        public string Nome { get; set; }

        public string Sobrenome { get; set; }

        public DateTime? DataNascimento { get; set; }

        public string CargoCodigo { get; set; }

        public string DepartamentoCodigo { get; set; }

        public string CodigoPerfilDeAcesso { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public string ClientUri { get; set; }
    }
}

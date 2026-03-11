using System;
using System.ComponentModel;

namespace Application.DTO.ApiDTO
{
    public class UsuarioRegistroDTO
    {
        [DisplayName("Código")]
        public string Codigo { get; set; }

        public string Nome { get; set; }

        public string Sobrenome { get; set; }

        [DisplayName("Data de nascimento")]
        public DateOnly? DataNascimento { get; set; }

        [DisplayName("Código de perfil de acesso")]
        public string? CodigoPerfilDeAcesso { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public string ConfirmaSenha { get; set; }

        public string ClientUri { get; set; } = string.Empty;
    }
}
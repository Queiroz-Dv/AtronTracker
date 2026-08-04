using System;
using System.ComponentModel;

namespace Application.DTO.Request
{
    [System.Text.Json.Serialization.JsonUnmappedMemberHandling(
        System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow)]
    public class UsuarioRegistroRequest
    {
        [DisplayName("Código")]
        public string Codigo { get; set; }

        public string Nome { get; set; }

        public string Sobrenome { get; set; }

        [DisplayName("Data de nascimento")]
        public DateOnly? DataNascimento { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public string ConfirmaSenha { get; set; }
    }
}

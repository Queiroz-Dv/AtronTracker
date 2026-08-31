using System;

namespace Application.DTO.Request
{

    public class UsuarioRegistroRequest
    {
        public string Codigo { get; set; }

        public string Nome { get; set; }

        public string Sobrenome { get; set; }

        public DateOnly? DataNascimento { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public string ConfirmaSenha { get; set; }

        public WorkspaceRegistroRequest? Workspace { get; set; }

        public string? Convite { get; set; }
    }
}

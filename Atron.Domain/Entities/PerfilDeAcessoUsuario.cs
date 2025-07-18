﻿using Atron.Domain.Customs;

namespace Atron.Domain.Entities
{
    [ModuloInfo("RPERFUSR", "Relacionamento Perfil de Acesso X Usuário")]
    public class PerfilDeAcessoUsuario
    {
        public int PerfilDeAcessoId { get; set; }
        public string PerfilDeAcessoCodigo { get; set; }

        public int UsuarioId { get; set; }
        public string UsuarioCodigo { get; set; }

        // Navegação
        public Usuario Usuario { get; set; }
        public PerfilDeAcesso PerfilDeAcesso { get; set; }
    }
}
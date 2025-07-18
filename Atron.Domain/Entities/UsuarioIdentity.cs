using System;
using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    public class UsuarioIdentity : Usuario
    {
        public UsuarioIdentity()
        {
            UsuarioCargoDepartamentos = new List<UsuarioCargoDepartamento>();
            PerfisDeAcessoUsuario = new List<PerfilDeAcessoUsuario>();
        }

        // Campos de Identidade
        public string NomeDeUsuario { get; set; }
        public string NomeDeUsuarioNormalizado { get; set; }
        public string EmailNormalizado { get; set; }
        public bool EmailConfirmado { get; set; }

        public string SenhaHash { get; set; }
        public string StampSeguranca { get; set; }
        public string StampConcorrencia { get; set; }

        public string Telefone { get; set; }
        public bool TelefoneConfirmado { get; set; }
        public bool DoisFatoresHabilitado { get; set; }

        public DateTimeOffset? BloqueioAte { get; set; }
        public bool BloqueioHabilitado { get; set; }
        public int TentativasFalhas { get; set; }

        // Tokens de autenticação
        public string TokenAcesso { get; set; }        // opcional se gerar dinamicamente
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpireTime { get; set; }

        public string Senha { get; set; }
        public string Token { get; set; }        
    }
}
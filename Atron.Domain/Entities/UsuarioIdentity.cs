using System;
using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    public class UsuarioIdentity : EntityBase
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Email { get; set; }
        public int? SalarioAtual { get; set; }

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

        public ICollection<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }

        public ICollection<PerfilDeAcessoUsuario> PerfisDeAcessoUsuario { get; set; }
    }
}
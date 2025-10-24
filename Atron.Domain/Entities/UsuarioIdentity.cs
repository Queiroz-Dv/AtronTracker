using System;
using System.Collections.Generic;

namespace Atron.Tracker.Domain.Entities
{
    public class UsuarioIdentity : Usuario
    {
        public UsuarioIdentity()
        {
            UsuarioCargoDepartamentos = new List<UsuarioCargoDepartamento>();
        }

        public string Senha { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpireTime { get; set; }
    }
}
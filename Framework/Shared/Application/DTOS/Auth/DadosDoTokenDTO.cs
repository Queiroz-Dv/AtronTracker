using Shared.Extensions;

namespace Shared.Application.DTOS.Auth
{
    public abstract class TokenBaseDTO
    {
        /// <summary>
        /// Token de acesso do usuário.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Tempo de expiração do token de acesso.
        /// </summary>
        public DateTime Expires { get; set; }

        public abstract bool IsValid();
    }

    /// <summary>
    /// Classe representa as informações do token de acesso e, opcionalmente, do token de atualização.
    /// </summary>
    public class DadosDoTokenDTO : TokenBaseDTO
    {
        /// <summary>
        /// Classe representa as informações do token de acesso e, opcionalmente, do token de atualização.
        /// </summary>
        /// <param name="token">Token de acesso que é autenticado</param>
        /// <param name="expires">Tempo de vida do token</param>
        public DadosDoTokenDTO(string token, DateTime expires, string usuarioCodigo = null)
        {
            Value = token;
            Expires = expires;
            UsuarioCodigo = usuarioCodigo;
        }

        public string UsuarioCodigo { get; set; }

        public override bool IsValid()
        {
            return !Value.IsNullOrEmpty() && Expires > DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Classe representa as informações do token de acesso e, opcionalmente, do token de atualização.
    /// </summary>
    public class DadosDoRefrehTokenDTO : TokenBaseDTO
    {
        /// <summary>
        /// Classe representa as informações do token de acesso e, opcionalmente, do token de atualização.
        /// </summary>
        /// <param name="token">Token de acesso que é autenticado</param>
        /// <param name="expires">Tempo de vida do token</param>
        public DadosDoRefrehTokenDTO(string token, DateTime expires)
        {
            Value = token;
            Expires = expires;
        }

        public override bool IsValid()
        {
            return Expires > DateTime.UtcNow;
        }
    }

    public class DadosDoRefreshTokenCookieDTO
    {
        public string RefreshToken { get; set; }

        public bool IsValid()
        {
            return !RefreshToken.IsNullOrEmpty();
        }
    }

    public class DadosDeTokenComRefreshToken
    {
        public DadosDoTokenDTO TokenDTO { get; set; }

        public DadosDoRefrehTokenDTO RefrehTokenDTO { get; set; }
    }
}

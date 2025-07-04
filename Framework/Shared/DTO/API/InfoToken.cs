namespace Shared.DTO.API
{
    /// <summary>
    /// InfoToken representa as informações do token de acesso e, opcionalmente, do token de atualização.
    /// </summary>
    public class InfoToken
    {
        public InfoToken(string token, DateTime expires)
        {
            Token = token;
            Expires = expires;
        }

        /// <summary>
        /// Token de acesso do usuário.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Tempo de expiração do token de acesso.
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// RefreshToken contém o token de atualização e seu tempo de expiração, se disponível.
        /// </summary>
        public InfoRefreshToken? InfoRefreshToken { get; set; }
    }

    /// <summary>
    /// InfoRefreshToken representa as informações do token de atualização, incluindo o token e seu tempo de expiração.
    /// </summary>
    /// <param name="Token"> O token de atualização que pode ser usado para obter um novo token de acesso.</param>
    /// <param name="RefreshTokenExpireTime">Tempo de expiração do token de atualização.</param>
    public record InfoRefreshToken(string Token, DateTime RefreshTokenExpireTime);   
}
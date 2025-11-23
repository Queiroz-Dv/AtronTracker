namespace Shared.Application.DTOS.Auth
{
    /// <summary>
    /// Representa a requisição de login do usuário.
    /// </summary>
    public class LoginRequestDTO
    {
        /// <summary>
        /// Código do usuário que será autenticado.
        /// </summary>
        public string CodigoDoUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário que será autenticado.
        /// </summary>
        public string Senha { get; set; } = string.Empty;
    }
}
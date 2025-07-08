using System;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces.Identity
{
    /// <summary>
    /// Interface para o repositório de usuários da aplicação.
    /// </summary>
    public interface IUserIdentityRepository
    {
        /// <summary>
        /// Recupera o refresh token associado ao código do usuário.
        /// </summary>
        /// <param name="codigoUsuario"> Código do usuário para o qual o refresh token será recuperado. </param>
        /// <returns> Retorna o refresh token associado ao código do usuário. </returns>
        Task<string> ObterRefreshTokenPorCodigoUsuarioRepositoryAsync(string codigoUsuario);

        /// <summary>
        /// Obtém o refresh token associado ao token de refresh fornecido.
        /// </summary>
        /// <param name="refreshToken"> Refresh token para o qual o refresh token será obtido. </param>
        /// <returns> Retorna o refresh token associado ao token de refresh fornecido.</returns>
        Task<bool> RefreshTokenExisteRepositoryAsync(string refreshToken);

        Task<bool> RefreshTokenExpiradoRepositoryAsync(string codigoUsuario);

        /// <summary>
        /// Redefine o refresh token para o usuário especificado.
        /// </summary>
        /// <param name="codigoUsuario"></param>
        /// <returns></returns>
        Task RedefinirRefreshTokenRepositoryAsync(string codigoUsuario);

        /// <summary>
        /// Atualiza o refresh token do usuário com o código fornecido.
        /// </summary>
        /// <param name="codigoUsuario"> Código do usuário para o qual o refresh token será atualizado. </param>
        /// <param name="refreshToken"> Refresh token a ser atualizado para o usuário. </param>
        /// <param name="refreshTokenExpireTime"> Refresh token expire time a ser atualizado para o usuário. </param>
        /// <returns> Retorna true se o refresh token foi atualizado com sucesso, caso contrário, retorna false. </returns>
        Task<bool> AtualizarRefreshTokenUsuarioRepositoryAsync(string codigoUsuario, string refreshToken, DateTime refreshTokenExpireTime);
    }
}
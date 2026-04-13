using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Domain.Interfaces.Identity
{
    public interface IUsuarioIdentityRepository
    {
        Task<bool> ContaExisteRepositoryAsync(string codigoUsuario, string email);
        Task<bool> RegistrarContaDeUsuarioRepositoryAsync(string codigoUsuario, string email, string senha);
        Task<bool> AtualizarUserIdentityRepositoryAsync(string codigoUsuario, string email, string senha);
        Task<bool> DeletarContaUserRepositoryAsync(string codigoUsuario);
        Task<bool> DesativarContaAsync(string codigoUsuario);
        Task<bool> ReativarContaAsync(string codigoUsuario);
        Task<bool> AtualizarRefreshTokenUsuarioRepositoryAsync(string codigoUsuario, string refreshToken, DateTime refreshTokenExpireTime);
        Task<bool> RefreshTokenExisteRepositoryAsync(string refreshToken);
        Task<string> ObterRefreshTokenPorCodigoUsuarioRepositoryAsync(string codigoUsuario);
        Task<bool> RedefinirRefreshTokenRepositoryAsync(string codigoUsuario);
        Task<bool> RefreshTokenExpiradoRepositoryAsync(string codigoUsuario);
        Task<string> GerarTokenConfirmacaoEmailAsync(string codigoUsuario);
        Task<bool> ConfirmarEmailAsync(string codigoUsuario, string token);
        Task<UsuarioIdentity> ObterUsuarioIdentityPorCodigo(string codigoUsuario);
        Task<string> GerarTokenAlteracaoEmailAsync(string codigoUsuario, string emailNovo);
        Task<bool> ConfirmarAlteracaoEmailAsync(string codigoUsuario, string emailNovo, string token);
        /// <summary>
        /// Verifica se o e-mail do usuário já foi confirmado
        /// </summary>
        Task<bool> EmailConfirmadoAsync(string codigoUsuario);
    }
}
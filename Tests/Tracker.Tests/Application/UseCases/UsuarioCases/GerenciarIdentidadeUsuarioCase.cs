using Application.Records.Autenticacao;
using Domain.Entities;
using Domain.Interfaces.Identity;
using Shared.Application.Security;
using Shared.Extensions;

namespace Tracker.Tests.Application.UseCases.UsuarioCases
{
    public class GerenciarIdentidadeUsuarioCase(IUsuarioIdentityRepository repository)
    {
        public async Task<bool> GravarRefreshTokenAsync(
            string codigoUsuario,
            string refreshToken,
            DateTime refreshTokenExpireTime)
        {
            if (codigoUsuario.IsNullOrEmpty() ||
                refreshToken.IsNullOrEmpty() ||
                refreshTokenExpireTime <= DateTime.UtcNow)
            {
                return false;
            }

            return await repository.AtualizarRefreshTokenUsuarioRepositoryAsync(
                codigoUsuario,
                RefreshTokenHash.Obter(refreshToken),
                refreshTokenExpireTime);
        }

        public async Task<SessaoRefreshToken> ObterSessaoRefreshTokenAsync(string refreshToken)
        {
            if (refreshToken.IsNullOrEmpty())
                return null;

            return await repository.ObterSessaoRefreshTokenRepositoryAsync(
                RefreshTokenHash.Obter(refreshToken));
        }

        public async Task<bool> RotacionarRefreshTokenAsync(RotacaoRefreshTokenRecord rotacao)
        {
            if (rotacao is null ||
                rotacao.UsuarioCodigo.IsNullOrEmpty() ||
                rotacao.RefreshTokenAtual.IsNullOrEmpty() ||
                rotacao.NovoRefreshToken.IsNullOrEmpty() ||
                rotacao.NovaExpiracao <= DateTime.UtcNow)
            {
                return false;
            }

            return await repository.RotacionarRefreshTokenRepositoryAsync(new RotacaoRefreshTokenHash(
                rotacao.UsuarioCodigo,
                RefreshTokenHash.Obter(rotacao.RefreshTokenAtual),
                RefreshTokenHash.Obter(rotacao.NovoRefreshToken),
                rotacao.NovaExpiracao));
        }

        public async Task<bool> RevogarRefreshTokenAsync(string codigoUsuario)
        {
            if (codigoUsuario.IsNullOrEmpty())
                return false;

            return await repository.RedefinirRefreshTokenRepositoryAsync(codigoUsuario);
        }
    }
}

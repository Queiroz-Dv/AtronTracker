using Application.DTO.ApiDTO;
using Application.Interfaces.Services.Identity;
using Application.Records.Autenticacao;
using Domain.Entities;
using Domain.Interfaces.Identity;
using Shared.Application.Security;
using Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace Application.Services.Identity;

public class UserIdentityService(IUsuarioIdentityRepository repository) : IUserIdentityService
{
    public Task<bool> GravarRefreshTokenAsync(
        string codigoUsuario,
        string refreshToken,
        DateTime refreshTokenExpireTime)
    {
        if (codigoUsuario.IsNullOrEmpty() ||
            refreshToken.IsNullOrEmpty() ||
            refreshTokenExpireTime <= DateTime.UtcNow)
        {
            return Task.FromResult(false);
        }

        return repository.AtualizarRefreshTokenUsuarioRepositoryAsync(
            codigoUsuario,
            RefreshTokenHash.Obter(refreshToken),
            refreshTokenExpireTime);
    }

    public Task<SessaoRefreshToken> ObterSessaoRefreshTokenAsync(string refreshToken)
    {
        if (refreshToken.IsNullOrEmpty())
            return Task.FromResult<SessaoRefreshToken>(null);

        return repository.ObterSessaoRefreshTokenRepositoryAsync(
            RefreshTokenHash.Obter(refreshToken));
    }

    public Task<bool> RotacionarRefreshTokenAsync(RotacaoRefreshTokenRecord rotacao)
    {
        if (rotacao is null ||
            rotacao.UsuarioCodigo.IsNullOrEmpty() ||
            rotacao.RefreshTokenAtual.IsNullOrEmpty() ||
            rotacao.NovoRefreshToken.IsNullOrEmpty() ||
            rotacao.NovaExpiracao <= DateTime.UtcNow)
        {
            return Task.FromResult(false);
        }

        return repository.RotacionarRefreshTokenRepositoryAsync(new RotacaoRefreshTokenHash(
            rotacao.UsuarioCodigo,
            RefreshTokenHash.Obter(rotacao.RefreshTokenAtual),
            RefreshTokenHash.Obter(rotacao.NovoRefreshToken),
            rotacao.NovaExpiracao));
    }

    public Task<bool> RevogarRefreshTokenAsync(string codigoUsuario)
        => codigoUsuario.IsNullOrEmpty()
            ? Task.FromResult(false)
            : repository.RedefinirRefreshTokenRepositoryAsync(codigoUsuario);

    public Task<bool> AtualizarUserIdentityServiceAsync(UsuarioRegistroDTO usuarioRegistro)
    {
        if (usuarioRegistro == null || usuarioRegistro.Codigo.IsNullOrEmpty())
            return Task.FromResult(false);

        return repository.AtualizarUserIdentityRepositoryAsync(
            usuarioRegistro.Codigo,
            usuarioRegistro.Email,
            usuarioRegistro.Senha);
    }

    public Task<bool> ContaExisteServiceAsync(string codigoUsuario, string email)
    {
        var valido = !codigoUsuario.IsNullOrEmpty() && !email.IsNullOrEmpty();
        return valido
            ? repository.ContaExisteRepositoryAsync(codigoUsuario, email)
            : Task.FromResult(false);
    }

    public Task<bool> DeletarContaUserServiceAsync(string codigoUsuario)
        => codigoUsuario.IsNullOrEmpty()
            ? Task.FromResult(false)
            : repository.DeletarContaUserRepositoryAsync(codigoUsuario);

    public Task<bool> RegistrarContaDeUsuarioServiceAsync(UsuarioRegistroDTO usuarioRegistroDTO)
    {
        if (usuarioRegistroDTO == null)
            return Task.FromResult(false);

        return repository.RegistrarContaDeUsuarioRepositoryAsync(
            usuarioRegistroDTO.Codigo,
            usuarioRegistroDTO.Email,
            usuarioRegistroDTO.Senha);
    }
}

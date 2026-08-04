using Application.DTO.ApiDTO;
using Application.DTO.Request;
using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Identity;

public interface IUserIdentityService
{
    Task<bool> GravarRefreshTokenAsync(
        string codigoUsuario,
        string refreshToken,
        DateTime refreshTokenExpireTime);

    Task<SessaoRefreshToken> ObterSessaoRefreshTokenAsync(string refreshToken);

    Task<bool> RotacionarRefreshTokenAsync(RotacaoRefreshToken rotacao);

    Task<bool> RevogarRefreshTokenAsync(string codigoUsuario);

    Task<bool> AtualizarUserIdentityServiceAsync(UsuarioRegistroDTO usuarioRegistro);

    Task<bool> RegistrarContaDeUsuarioServiceAsync(UsuarioRegistroDTO usuarioRegistroDTO);

    Task<bool> DeletarContaUserServiceAsync(string codigoUsuario);

    Task<bool> ContaExisteServiceAsync(string codigoUsuario, string email);
}

using Atron.Application.DTO.ApiDTO;
using Atron.Application.Interfaces.Services.Identity;
using Atron.Domain.Interfaces.Identity;
using Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace Atron.Application.Services.Identity
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly IUsuarioIdentityRepository _repository;

        public UserIdentityService(IUsuarioIdentityRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AtualizarRefreshTokenUsuarioServiceAsync(string codigoUsuario, string refreshToken, DateTime refreshTokenExpireTime)
        {
            if (codigoUsuario.IsNullOrEmpty() ||
                refreshToken.IsNullOrEmpty() ||
                refreshTokenExpireTime > DateTime.Now)
            {
                return false;
            }

            return await _repository.AtualizarRefreshTokenUsuarioRepositoryAsync(codigoUsuario, refreshToken, refreshTokenExpireTime);
        }

        public async Task<bool> AtualizarUserIdentityServiceAsync(UsuarioRegistroDTO usuarioRegistro)
        {
            if (usuarioRegistro == null || usuarioRegistro.Codigo.IsNullOrEmpty())
            {
                return false;
            }

            return await _repository.AtualizarUserIdentityRepositoryAsync(usuarioRegistro.Codigo, usuarioRegistro.Email, usuarioRegistro.Senha);
        }

        public async Task<bool> ContaExisteServiceAsync(string codigoUsuario, string email)
        {
            var isValid = !codigoUsuario.IsNullOrEmpty() && !email.IsNullOrEmpty();
            return isValid ? await _repository.ContaExisteRepositoryAsync(codigoUsuario, email) : false;
        }

        public Task<bool> DeletarContaUserServiceAsync(string codigoUsuario)
        {
            var isValid = !codigoUsuario.IsNullOrEmpty();
            return isValid ? _repository.DeletarContaUserRepositoryAsync(codigoUsuario) : Task.FromResult(false);
        }

        public async Task<string> ObterRefreshTokenPorCodigoUsuarioServiceAsync(string codigoUsuario)
        {
            return codigoUsuario.IsNullOrEmpty() ? string.Empty : await _repository.ObterRefreshTokenPorCodigoUsuarioRepositoryAsync(codigoUsuario);
        }

        public async Task<bool> RedefinirRefreshTokenServiceAsync(string codigoUsuario)
        {
            if (!codigoUsuario.IsNullOrEmpty())
            {
                return await _repository.RedefinirRefreshTokenRepositoryAsync(codigoUsuario);
            }

            return false;
        }

        public async Task<bool> RefreshTokenExisteServiceAsync(string refreshToken)
        {
            return refreshToken.IsNullOrEmpty() ? false : await _repository.RefreshTokenExisteRepositoryAsync(refreshToken);
        }

        public async Task<bool> RefreshTokenExpiradoServiceAsync(string codigoUsuario)
        {
            return codigoUsuario.IsNullOrEmpty() ? false : await _repository.RefreshTokenExpiradoRepositoryAsync(codigoUsuario);
        }

        public async Task<bool> RegistrarContaDeUsuarioServiceAsync(UsuarioRegistroDTO usuarioRegistroDTO)
        {
            if (usuarioRegistroDTO == null)
                return false;

            return await _repository.RegistrarContaDeUsuarioRepositoryAsync(usuarioRegistroDTO.Codigo, usuarioRegistroDTO.Email, usuarioRegistroDTO.Senha);
        }
    }
}
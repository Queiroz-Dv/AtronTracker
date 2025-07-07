using Atron.Application.Interfaces.Services.Identity;
using Atron.Domain.Interfaces.Identity;
using Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace Atron.Application.Services.Identity
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly IUserIdentityRepository _repository;

        public UserIdentityService(IUserIdentityRepository repository)
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

        public async Task<string> ObterRefreshTokenPorCodigoUsuarioServiceAsync(string codigoUsuario)
        {
            return codigoUsuario.IsNullOrEmpty() ? string.Empty : await _repository.ObterRefreshTokenPorCodigoUsuarioRepositoryAsync(codigoUsuario);         
        }

        public async Task RedefinirRefreshTokenServiceAsync(string codigoUsuario)
        {
            if (!codigoUsuario.IsNullOrEmpty())
            {
                await _repository.RedefinirRefreshTokenRepositoryAsync(codigoUsuario);
            }
        }

        public async Task<bool> RefreshTokenExisteServiceAsync(string refreshToken)
        {
            return  refreshToken.IsNullOrEmpty() ? false : await  _repository.RefreshTokenExisteRepositoryAsync(refreshToken);           
        }

        public async Task<bool> RefreshTokenExpiradoServiceAsync(string codigoUsuario)
        {
            return codigoUsuario.IsNullOrEmpty() ? false : await _repository.RefreshTokenExpiradoRepositoryAsync(codigoUsuario);
        }
    }
}
using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Users;
using System;

namespace Application.Interfaces.Services
{
    public interface ICacheUsuarioService
    {
        void GravarCacheDeAcesso(DadosComplementaresDoUsuarioDTO dadosDoUsuario, DateTime expiracaoAccessToken);

        void RemoverCacheDeAcessoTokenInfo(string codigoUsuario);
    }
}

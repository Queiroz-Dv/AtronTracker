using Shared.Application.DTOS.Auth;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.ApplicationInterfaces
{
    /// <summary>
    /// Classe de autenticação para os usuários
    /// </summary>
    public interface ILoginService
    {
        Task<Resultado<DadosDoTokenDTO>> Autenticar(LoginRequestDTO login);

        Task<Resultado> Logout(string usuarioCodigo);

        Task<Resultado<DadosDoTokenDTO>> RefreshAcesso(DadosDoRefreshTokenCookieDTO infoToken);        
    }
}

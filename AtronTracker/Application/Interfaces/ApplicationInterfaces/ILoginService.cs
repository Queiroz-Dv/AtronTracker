using Shared.Application.DTOS.Auth;
using System.Threading.Tasks;

namespace Application.Interfaces.ApplicationInterfaces
{
    /// <summary>
    /// Classe de autenticação para os usuários
    /// </summary>
    public interface ILoginService
    {
        Task<DadosDoTokenDTO> Autenticar(LoginRequestDTO login);

        Task<bool> Logout(string usuarioCodigo);

        Task<DadosDoTokenDTO> RefreshAcesso(DadosDoTokenDTO infoToken);

        Task<bool> TrocarSenha(LoginRequestDTO dto);
    }
}
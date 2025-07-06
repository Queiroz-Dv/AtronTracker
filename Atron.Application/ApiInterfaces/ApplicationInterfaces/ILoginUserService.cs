using Shared.DTO.API;
using Shared.DTO.API.Request;
using System.Threading.Tasks;

namespace Atron.Application.ApiInterfaces.ApplicationInterfaces
{
    /// <summary>
    /// Classe de autenticação para os usuários
    /// </summary>
    public interface ILoginUserService
    {
        Task<DadosDoTokenDTO> Autenticar(LoginRequestDTO login);

        Task Logout(string usuarioCodigo);

        Task<DadosDoTokenDTO> RefreshAcesso(DadosDoTokenDTO infoToken);

        Task<bool> TrocarSenha(LoginRequestDTO dto);
    }
}
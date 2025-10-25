using Application.DTO;
using Shared.DTO.API;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IDadosComplementaresDoUsuarioService
    {
        Task<DadosComplementaresDoUsuarioDTO> ObterInformacoesComplementaresDoUsuario(UsuarioDTO usuarioDTO);
    }
}

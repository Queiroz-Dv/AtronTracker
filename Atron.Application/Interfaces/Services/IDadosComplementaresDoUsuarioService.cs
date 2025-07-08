using Atron.Application.DTO;
using Shared.DTO.API;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces.Services
{
    public interface IDadosComplementaresDoUsuarioService
    {
        Task<DadosComplementaresDoUsuarioDTO> ObterInformacoesComplementaresDoUsuario(UsuarioDTO usuarioDTO);
    }
}

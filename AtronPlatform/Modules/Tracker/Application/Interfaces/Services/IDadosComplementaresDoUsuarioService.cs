using Application.DTO;
using Shared.Application.DTOS.Users;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IDadosComplementaresDoUsuarioService
    {
        Task<DadosComplementaresDoUsuarioDTO> ObterInformacoesComplementaresDoUsuario(UsuarioDTO usuarioDTO);
    }
}

using Atron.Application.DTO;
using Atron.Application.DTO.ApiDTO;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces.Handlers
{
    public interface IUsuarioHandler
    {
        Task<LoginDTO> PreencherInformacoesDeUsuarioParaLoginAsync(UsuarioDTO usuarioDTO);
    }
}
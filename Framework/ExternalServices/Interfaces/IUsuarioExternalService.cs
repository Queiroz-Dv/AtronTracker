using Atron.Application.DTO;
using ExternalServices.Interfaces.ExternalMessage;

namespace ExternalServices.Interfaces
{
    public interface IUsuarioExternalService : IExternalMessageService
    {
        Task Criar(UsuarioDTO model);
        Task<List<UsuarioDTO>> ObterTodos();
    }
}

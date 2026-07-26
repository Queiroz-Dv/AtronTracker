using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Service;
using System.Threading.Tasks;

namespace Application.Interfaces.Mapping
{
    public interface IPerfilDeAcessoMapping : IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso>
    {
        Task<PerfilDeAcessoUsuarioDTO> MapToPerfilDeAcessoUsuarioDTOAsync(PerfilDeAcesso perfilDeAcesso);
    }
}

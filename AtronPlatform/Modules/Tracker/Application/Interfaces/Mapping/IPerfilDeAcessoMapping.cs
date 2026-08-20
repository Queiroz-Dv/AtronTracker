using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace Application.Interfaces.Mapping
{
    public interface IPerfilDeAcessoMapping : IMapper<PerfilDeAcesso, PerfilDeAcessoDTO>
    {
        PerfilDeAcessoUsuarioDTO MapToPerfilDeAcessoUsuarioDto(PerfilDeAcesso perfilDeAcesso);
    }
}

using Application.DTO;
using Domain.Entities;
using Shared.Application.Services.Mapper;
using System.Threading.Tasks;

namespace Application.Mapping
{
    public class UsuarioDoPerfilDeAcessoMapping : AsyncApplicationMapService<UsuarioDTO, PerfilDeAcessoUsuario>
    {
        public override Task<UsuarioDTO> MapToDTOAsync(PerfilDeAcessoUsuario entity)
        {
            var usuario = entity.Usuario;
            return Task.FromResult(new UsuarioDTO
            {
                Codigo = usuario.Codigo,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome
            });
        }

        public override Task<PerfilDeAcessoUsuario> MapToEntityAsync(UsuarioDTO dto)
        {
            return Task.FromResult(new PerfilDeAcessoUsuario
            {
                UsuarioCodigo = dto.Codigo,
                Usuario = new Usuario
                {
                    Codigo = dto.Codigo,
                    Nome = dto.Nome,
                    Sobrenome = dto.Sobrenome
                }
            });
        }
    }
}

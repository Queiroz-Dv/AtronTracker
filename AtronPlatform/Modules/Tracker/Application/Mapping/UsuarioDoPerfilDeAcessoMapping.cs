using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping
{
    public sealed class UsuarioDoPerfilDeAcessoMapping : Mapper<PerfilDeAcessoUsuario, UsuarioDTO>
    {
        public override UsuarioDTO MapToDto(PerfilDeAcessoUsuario entity)
        {
            var usuario = entity.Usuario;
            return new UsuarioDTO
            {
                Codigo = usuario.Codigo,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome
            };
        }

        public override PerfilDeAcessoUsuario MapToEntity(UsuarioDTO dto)
        {
            return new PerfilDeAcessoUsuario
            {
                UsuarioCodigo = dto.Codigo,
                Usuario = new Usuario
                {
                    Codigo = dto.Codigo,
                    Nome = dto.Nome,
                    Sobrenome = dto.Sobrenome
                }
            };
        }
    }
}

using Application.DTO.Request;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping
{
    /// <summary>
    /// Mapeamento para UsuarioRequest ↔ Usuario.    
    /// </summary>
    public class UsuarioRequestMapping : Mapper<Usuario, UsuarioRequest>
    {
        /// <summary>
        /// Mapeia uma entidade Usuario para um UsuarioRequest (DTO).
        /// </summary>
        public override UsuarioRequest MapToDto(Usuario entity)
        {
            return new UsuarioRequest
            {
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                Sobrenome = entity.Sobrenome,
                DataNascimento = entity.DataNascimento,
                Email = entity.Email,
                GestorImediatoCodigo = entity.GestorImediatoCodigo
            };

        }

        /// <summary>
        /// Mapeia um UsuarioRequest (DTO) para uma nova entidade Usuario.
        /// </summary>
        public override Usuario MapToEntity(UsuarioRequest dto)
        {
            return new Usuario
            {
                Codigo = dto.Codigo,
                Nome = dto.Nome,
                Sobrenome = dto.Sobrenome,
                DataNascimento = dto.DataNascimento,
                Email = dto.Email,
                GestorImediatoCodigo = dto.GestorImediatoCodigo,
                ReceberNotificacaoInternaTarefa = true,
                ReceberNotificacaoTarefaPorEmail = false
            };

        }

        /// <summary>
        /// Mapeia um UsuarioRequest (DTO) para uma entidade Usuario existente (atualização in-place).
        /// O código não é atualizado pois é a chave de identificação.
        /// </summary>
        public void MapToEntity(UsuarioRequest dto, Usuario entityToUpdate)
        {
            entityToUpdate.Nome = dto.Nome;
            entityToUpdate.Sobrenome = dto.Sobrenome;
            entityToUpdate.DataNascimento = dto.DataNascimento;
            entityToUpdate.Email = dto.Email;
            entityToUpdate.GestorImediatoCodigo = dto.GestorImediatoCodigo;

        }
    }
}

using Application.DTO.Request;
using Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;
using System.Threading.Tasks;

namespace Application.Mapping
{
    /// <summary>
    /// Mapeamento para UsuarioRequest ↔ Usuario seguindo o padrão CategoriaMapping.
    /// Focado em operações CRUD administrativas, separado do UsuarioIdentityMapping
    /// que é utilizado em cenários de autenticação.
    /// </summary>
    public class UsuarioRequestMapping : AsyncApplicationMapService<UsuarioRequest, Usuario>, IAsyncMap<UsuarioRequest, Usuario>
    {
        /// <summary>
        /// Mapeia uma entidade Usuario para um UsuarioRequest (DTO).
        /// </summary>
        public override Task<UsuarioRequest> MapToDTOAsync(Usuario entity)
        {
            var dto = new UsuarioRequest
            {
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                Sobrenome = entity.Sobrenome,
                DataNascimento = entity.DataNascimento,
                Email = entity.Email,
                SalarioMensal = entity.SalarioAtual
            };

            return Task.FromResult(dto);
        }

        /// <summary>
        /// Mapeia um UsuarioRequest (DTO) para uma nova entidade Usuario.
        /// </summary>
        public override Task<Usuario> MapToEntityAsync(UsuarioRequest dto)
        {
            var entity = new Usuario
            {
                Codigo = dto.Codigo?.ToUpper(),
                Nome = dto.Nome,
                Sobrenome = dto.Sobrenome,
                DataNascimento = dto.DataNascimento,
                Email = dto.Email,
                SalarioAtual = dto.SalarioMensal
            };

            return Task.FromResult(entity);
        }

        /// <summary>
        /// Mapeia um UsuarioRequest (DTO) para uma entidade Usuario existente (atualização in-place).
        /// O código não é atualizado pois é a chave de identificação.
        /// </summary>
        public Task MapToEntityAsync(UsuarioRequest dto, Usuario entityToUpdate)
        {
            entityToUpdate.Nome = dto.Nome;
            entityToUpdate.Sobrenome = dto.Sobrenome;
            entityToUpdate.DataNascimento = dto.DataNascimento;
            entityToUpdate.Email = dto.Email;
            entityToUpdate.SalarioAtual = dto.SalarioMensal;

            return Task.CompletedTask;
        }
    }
}

using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;
using System.Threading.Tasks;

namespace Application.Mapping
{
    /// <summary>
    /// Mapeamento assíncrono entre CargoDTO e Cargo
    /// </summary>
    public class CargoMapping : AsyncApplicationMapService<CargoDTO, Cargo>, IAsyncMap<CargoDTO, Cargo>
    {
        public override Task<CargoDTO> MapToDTOAsync(Cargo entity)
        {
            var cargo = new CargoDTO(entity.Codigo, entity.Descricao)
            {
                Id = entity.Id,
                DepartamentoCodigo = entity.DepartamentoCodigo,
                DepartamentoDescricao = entity.Departamento?.Descricao,
                DepartamentoId = entity.DepartamentoId,
                Departamento = entity.Departamento != null ? new DepartamentoDTO
                {
                    Id = entity.Departamento.Id,
                    Codigo = entity.Departamento.Codigo,
                    Descricao = entity.Departamento.Descricao
                } : null
            };

            return Task.FromResult(cargo);
        }

        public override Task<Cargo> MapToEntityAsync(CargoDTO dto)
        {
            var entity = new Cargo
            {
                Codigo = dto.Codigo.ToUpper(),
                Descricao = dto.Descricao.ToUpper(),
                DepartamentoCodigo = dto.DepartamentoCodigo.ToUpper()
            };

            return Task.FromResult(entity);
        }

        /// <summary>
        /// Atualiza uma entidade existente com os dados do DTO
        /// </summary>
        public Task MapToEntityAsync(CargoDTO dto, Cargo entityToUpdate)
        {
            entityToUpdate.Descricao = dto.Descricao.ToUpper();
            entityToUpdate.DepartamentoCodigo = dto.DepartamentoCodigo.ToUpper();
            return Task.CompletedTask;
        }
    }
}

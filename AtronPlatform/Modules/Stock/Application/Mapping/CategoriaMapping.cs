using AtronStock.Application.DTO.Request;
using AtronStock.Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;

namespace AtronStock.Application.Mapping
{
    public class CategoriaMapping : AsyncApplicationMapService<CategoriaRequest, Categoria>, IAsyncMap<CategoriaRequest, Categoria>
    {
        public override Task<CategoriaRequest> MapToDTOAsync(Categoria entity)
        {
            var dto = new CategoriaRequest
            {
                Codigo = entity.Codigo,
                Descricao = entity.Descricao,
                Status = entity.Status
            };

            return Task.FromResult(dto);
        }

        public override Task<Categoria> MapToEntityAsync(CategoriaRequest dto)
        {
            var entity = new Categoria
            {
                Codigo = dto.Codigo,
                Descricao = dto.Descricao,
                Status = dto.Status
            };

            return Task.FromResult(entity);
        }

        public Task MapToEntityAsync(CategoriaRequest dto, Categoria entityToUpdate)
        {
            entityToUpdate.Descricao = dto.Descricao;
            entityToUpdate.Status = dto.Status;

            return Task.CompletedTask;
        }
    }
}
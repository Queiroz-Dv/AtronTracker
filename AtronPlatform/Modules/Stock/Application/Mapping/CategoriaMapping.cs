using AtronStock.Application.DTO.Request;
using AtronStock.Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace AtronStock.Application.Mapping
{
    public sealed class CategoriaMapping
        : Mapper<Categoria, CategoriaRequest>,
          IUpdateMapper<Categoria, CategoriaRequest>
    {
        public override CategoriaRequest MapToDto(Categoria entity)
        {
            return new CategoriaRequest
            {
                Codigo = entity.Codigo,
                Descricao = entity.Descricao,
                Status = entity.Status
            };

        }

        public override Categoria MapToEntity(CategoriaRequest dto)
        {
            return new Categoria
            {
                Codigo = dto.Codigo,
                Descricao = dto.Descricao,
                Status = dto.Status
            };

        }

        public void MapToUpdate(CategoriaRequest dto, Categoria entityToUpdate)
        {
            entityToUpdate.Descricao = dto.Descricao;
            entityToUpdate.Status = dto.Status;

        }
    }
}

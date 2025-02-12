using Shared.Interfaces.Mapper;

namespace Shared.Services.Mapper
{
    public abstract class ApplicationMapService<DTO, Entity> : IApplicationMapService<DTO, Entity>
    {
        public abstract Entity MapToEntity(DTO dto);
        public abstract DTO MapToDTO(Entity entity);

        public virtual List<Entity> MapToListEntity(List<DTO> dtos)
        {
            if (dtos == null) return null;
            var entities = new List<Entity>();
            foreach (var dto in dtos)
            {
                entities.Add(MapToEntity(dto));
            }
            return entities;
        }

        public virtual List<DTO> MapToListDTO(List<Entity> entities)
        {
            if (entities == null) return null;
            var dtos = new List<DTO>();
            foreach (var entity in entities)
            {
                dtos.Add(MapToDTO(entity));
            }
            return dtos;
        }
    }
}
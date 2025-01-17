namespace Shared.Interfaces.Mapper
{
    public interface IApplicationMapService<DTO, Entity>
    {
        Entity MapToEntity(DTO dto);
        DTO MapToDTO(Entity entity);
        List<Entity> MapToListEntity(List<DTO> dtos);
        List<DTO> MapToListDTO(List<Entity> entities);
    }
}
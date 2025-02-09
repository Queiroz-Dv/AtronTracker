namespace Shared.Interfaces.Mapper
{
    /// <summary>
    /// Interface para mapeamento de DTOs para Entidades e vice-versa.
    /// </summary>
    /// <typeparam name="DTO">O tipo do Data Transfer Object (DTO).</typeparam>
    /// <typeparam name="Entity">O tipo da Entidade.</typeparam>
    public interface IApplicationMapService<DTO, Entity>
    {
        /// <summary>
        /// Mapeia um DTO para uma Entidade.
        /// </summary>
        /// <param name="dto">O DTO a ser mapeado.</param>
        /// <returns>A Entidade mapeada.</returns>
        Entity MapToEntity(DTO dto);

        /// <summary>
        /// Mapeia uma Entidade para um DTO.
        /// </summary>
        /// <param name="entity">A Entidade a ser mapeada.</param>
        /// <returns>O DTO mapeado.</returns>
        DTO MapToDTO(Entity entity);

        /// <summary>
        /// Mapeia uma lista de DTOs para uma lista de Entidades.
        /// </summary>
        /// <param name="dtos">A lista de DTOs a serem mapeados.</param>
        /// <returns>A lista de Entidades mapeadas.</returns>
        List<Entity> MapToListEntity(List<DTO> dtos);

        /// <summary>
        /// Mapeia uma lista de Entidades para uma lista de DTOs.
        /// </summary>
        /// <param name="entities">A lista de Entidades a serem mapeadas.</param>
        /// <returns>A lista de DTOs mapeados.</returns>
        List<DTO> MapToListDTO(List<Entity> entities);
    }
}
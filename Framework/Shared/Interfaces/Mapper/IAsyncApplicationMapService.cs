namespace Shared.Interfaces.Mapper
{
    /// <summary>
    /// Define um contrato para serviços de mapeamento assíncronos entre Entidades de Domínio e
    /// Data Transfer Objects (DTOs). Esta interface é projetada para cenários onde o processo
    /// de mapeamento pode envolver operações de I/O, como consultas a banco de dados para
    /// enriquecer o objeto mapeado.
    /// </summary>
    /// <typeparam name="DTO">O tipo do Data Transfer Object.</typeparam>
    /// <typeparam name="Entity">O tipo da Entidade de Domínio.</typeparam>
    public interface IAsyncApplicationMapService<DTO, Entity>
    {
        /// <summary>
        /// Mapeia de forma assíncrona um Data Transfer Object (DTO) para uma Entidade de Domínio.
        /// </summary>
        /// <param name="dto">O DTO a ser mapeado.</param>
        /// <returns>
        /// Uma Task que representa a operação assíncrona. O resultado da Task contém a Entidade mapeada.
        /// </returns>
        Task<Entity> MapToEntityAsync(DTO dto);

        /// <summary>
        /// Mapeia de forma assíncrona uma Entidade de Domínio para um Data Transfer Object (DTO).
        /// </summary>
        /// <param name="entity">A Entidade a ser mapeada.</param>
        /// <returns>
        /// Uma Task que representa a operação assíncrona. O resultado da Task contém o DTO mapeado.
        /// </returns>
        Task<DTO> MapToDTOAsync(Entity entity);

        /// <summary>
        /// Mapeia de forma assíncrona uma coleção de DTOs para uma lista de Entidades.
        /// A implementação deve otimizar a execução concorrente das operações de mapeamento.
        /// </summary>
        /// <param name="dtos">A coleção de DTOs a ser mapeada.</param>
        /// <returns>
        /// Uma Task que representa a operação assíncrona. O resultado da Task contém a lista de Entidades mapeadas.
        /// </returns>
        Task<List<Entity>> MapToListEntityAsync(IEnumerable<DTO> dtos);

        /// <summary>
        /// Mapeia de forma assíncrona uma coleção de Entidades para uma lista de DTOs.
        /// A implementação deve otimizar a execução concorrente das operações de mapeamento.
        /// </summary>
        /// <param name="entities">A coleção de Entidades a ser mapeada.</param>
        /// <returns>
        /// Uma Task que representa a operação assíncrona. O resultado da Task contém a lista de DTOs mapeados.
        /// </returns>
        Task<List<DTO>> MapToListDTOAsync(IEnumerable<Entity> entities);        
    }

    public interface IAsyncMap<DTO, Entity> : IAsyncApplicationMapService<DTO, Entity>
    {
        /// <summary>
        /// Mapeia de forma assíncrona um DTO para uma Entidade existente (Atualização).
        /// </summary>
        /// <param name="dto">O DTO com os dados de origem.</param>
        /// <param name="entityToUpdate">A Entidade existente a ser atualizada.</param>
        Task MapToEntityAsync(DTO dto, Entity entityToUpdate);
    }
}
using Shared.Interfaces.Mapper;

namespace Shared.Services.Mapper
{
    /// <summary>
    /// Fornece uma implementação base para a interface <see cref="IAsyncApplicationMapService{DTO, Entity}"/>.
    /// Esta classe abstrata gerencia o mapeamento de coleções de forma otimizada e concorrente,
    /// exigindo que as classes derivadas implementem apenas a lógica de mapeamento para itens individuais.
    /// </summary>
    /// <typeparam name="DTO">O tipo do Data Transfer Object.</typeparam>
    /// <typeparam name="Entity">O tipo da Entidade de Domínio.</typeparam>
    public abstract class AsyncApplicationMapService<DTO, Entity> : IAsyncApplicationMapService<DTO, Entity>
    {
        /// <summary>
        /// Quando sobreposto em uma classe derivada, mapeia de forma assíncrona um único DTO para uma Entidade.
        /// </summary>
        /// <param name="dto">O DTO a ser mapeado.</param>
        /// <returns>Uma Task contendo a Entidade mapeada.</returns>
        public abstract Task<Entity> MapToEntityAsync(DTO dto);

        /// <summary>
        /// Quando sobreposto em uma classe derivada, mapeia de forma assíncrona uma única Entidade para um DTO.
        /// </summary>
        /// <param name="entity">A Entidade a ser mapeada.</param>
        /// <returns>Uma Task contendo o DTO mapeado.</returns>
        public abstract Task<DTO> MapToDTOAsync(Entity entity);

        /// <summary>
        /// Mapeia de forma assíncrona e concorrente uma coleção de DTOs para uma lista de Entidades.
        /// Este método executa todas as operações de mapeamento em paralelo e aguarda a conclusão de todas.
        /// </summary>
        /// <param name="dtos">A coleção de DTOs a serem mapeados.</param>
        /// <returns>Uma lista de Entidades mapeadas.</returns>
        public virtual async Task<List<Entity>> MapToListEntityAsync(IEnumerable<DTO> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return new List<Entity>();
            }

            // Inicia uma tarefa de mapeamento para cada DTO na coleção.
            var tasks = dtos.Select(dto => MapToEntityAsync(dto));

            // Aguarda de forma não bloqueante que todas as tarefas de mapeamento sejam concluídas.
            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        /// <summary>
        /// Mapeia de forma assíncrona e concorrente uma coleção de Entidades para uma lista de DTOs.
        /// Este método executa todas as operações de mapeamento em paralelo e aguarda a conclusão de todas.
        /// </summary>
        /// <param name="entities">A coleção de Entidades a serem mapeadas.</param>
        /// <returns>Uma lista de DTOs mapeados.</returns>
        public virtual async Task<List<DTO>> MapToListDTOAsync(IEnumerable<Entity> entities)
        {
            if (entities == null || !entities.Any())
            {
                return new List<DTO>();
            }

            // Inicia uma tarefa de mapeamento para cada Entidade na coleção.
            var tasks = entities.Select(entity => MapToDTOAsync(entity));

            // Aguarda de forma não bloqueante que todas as tarefas de mapeamento sejam concluídas.
            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }
    }
}
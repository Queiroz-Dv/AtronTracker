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
        protected readonly IMapperEngine _mapperEngine;

        protected AsyncApplicationMapService(IMapperEngine mapperEngine)
        {
            // Garante que a dependência não seja nula quando este construtor for usado.
            _mapperEngine = mapperEngine ?? throw new ArgumentNullException(nameof(mapperEngine));
        }

        protected AsyncApplicationMapService()
        {
            _mapperEngine = null;
        }

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
        /// Método protegido para mapear uma coleção de entidades filhas para uma coleção de DTOs filhos.
        /// Este método delega o trabalho pesado para o IMapperEngine.
        /// </summary>
        /// <typeparam name="TChildSource">O tipo da entidade filha na coleção de origem.</typeparam>
        /// <typeparam name="TChildDestination">O tipo do DTO filho na coleção de destino.</typeparam>
        /// <param name="sourceCollection">A coleção de entidades filhas a ser mapeada.</param>
        /// <param name="childMapper">A instância do mapeador especialista para o tipo filho.</param>
        /// <returns>Uma lista de DTOs filhos mapeados.</returns>
        protected async Task<List<TChildDestination>> MapChildrenAsync<TChildSource, TChildDestination>(
            IEnumerable<TChildSource> sourceCollection,
            IAsyncApplicationMapService<TChildDestination, TChildSource> childMapper)
        {
            // Verificação de segurança: garante que o motor foi injetado antes de usá-lo.
            if (_mapperEngine == null)
            {
                throw new InvalidOperationException(
                    $"Para usar o método 'MapChildrenAsync', o mapeador '{GetType().Name}' " +
                    "deve receber 'IMapperEngine' em seu construtor. " +
                    "Verifique a configuração de injeção de dependência e o construtor da classe de mapeamento.");
            }

            if (childMapper == null)
            {
                throw new ArgumentNullException(nameof(childMapper), "O mapeador especialista para o filho não pode ser nulo.");
            }

            // Simplesmente usa o motor para fazer o trabalho.
            return await _mapperEngine.MapCollectionAsync(sourceCollection, childMapper);
        }

        /// <summary>
        /// Mapeia uma única entidade filha para um DTO filho (Relacionamento 1-para-1).
        /// </summary>
        /// <typeparam name="TChildSource">O tipo da entidade filha de origem.</typeparam>
        /// <typeparam name="TChildDestination">O tipo do DTO filho de destino.</typeparam>
        /// <param name="sourceChild">A instância da entidade filha a ser mapeada.</param>
        /// <param name="childMapper">O mapeador especialista para o tipo filho.</param>
        /// <returns>O DTO filho mapeado, ou o valor padrão (null) se a origem for nula.</returns>
        protected async Task<TChildDestination> MapChildAsync<TChildSource, TChildDestination>(
            TChildSource sourceChild,
            IAsyncApplicationMapService<TChildDestination, TChildSource> childMapper)
        {
            if (sourceChild == null)
            {
                return default; // Retorna null se a entidade filha não existir.
            }

            if (childMapper == null)
            {
                throw new ArgumentNullException(nameof(childMapper), "O mapeador especialista para o filho não pode ser nulo.");
            }

            // Delega diretamente para o método de mapeamento do especialista.
            return await childMapper.MapToDTOAsync(sourceChild);
        }


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
            var tasks = dtos.Select(MapToEntityAsync);

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
            var tasks = entities.Select(MapToDTOAsync);

            // Aguarda de forma não bloqueante que todas as tarefas de mapeamento sejam concluídas.
            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }
    }
}
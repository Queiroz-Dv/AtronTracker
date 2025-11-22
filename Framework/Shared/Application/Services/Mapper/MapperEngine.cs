using Shared.Application.Interfaces.Service;

namespace Shared.Application.Services.Mapper
{
    /// <summary>
    /// Implementação padrão do motor de mapeamento. Esta classe deve ser registrada
    /// como Scoped ou Transient no contêiner de DI.
    /// </summary>
    public class MapperEngine : IMapperEngineService
    {
        /// <summary>
        /// Usa a implementação base do AsyncApplicationMapService para mapear a coleção de forma
        /// concorrente e performática.
        /// </summary>
        public async Task<List<TDestination>> MapCollectionAsync<TSource, TDestination>(
            IEnumerable<TSource> sourceCollection,
            IAsyncApplicationMapService<TDestination, TSource> childMapper)
        {
            if (sourceCollection == null || !sourceCollection.Any())
            {
                return new List<TDestination>();
            }

            // Delega a lógica de mapeamento da lista para o mapeador filho fornecido.
            // O childMapper, herdando de AsyncApplicationMapService, já sabe como usar Task.WhenAll
            // para fazer isso de forma eficiente.
            return await childMapper.MapToListDTOAsync(sourceCollection);
        }
    }
}

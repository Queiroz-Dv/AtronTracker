namespace Shared.Application.Interfaces.Service
{
    /// <summary>
    /// Define a interface para o motor de mapeamento, responsável por orquestrar
    /// o mapeamento de coleções usando mapeadores específicos.
    /// </summary>
    public interface IMapperEngineService
    {
        /// <summary>
        /// Mapeia uma coleção de entidades de origem para uma coleção de DTOs de destino
        /// usando um mapeador filho específico.
        /// </summary>
        /// <typeparam name="TSource">O tipo da entidade de origem.</typeparam>
        /// <typeparam name="TDestination">O tipo do DTO de destino.</typeparam>
        /// <param name="sourceCollection">A coleção de origem a ser mapeada.</param>
        /// <param name="childMapper">A instância do mapeador filho a ser usada para a conversão.</param>
        /// <returns>Uma lista de DTOs mapeados.</returns>
        Task<List<TDestination>> MapCollectionAsync<TSource, TDestination>(
            IEnumerable<TSource> sourceCollection,
            IAsyncApplicationMapService<TDestination, TSource> childMapper);
    }
}

using Atron.Domain.ApiEntities;
using ExternalServices.Interfaces.ExternalMessage;

namespace ExternalServices.Interfaces.ApiRoutesInterfaces
{
    /// <summary>
    /// Interface dos processos e fluxos do módulo de rotas da API
    /// </summary>
    public interface IApiRouteExternalService : IExternalMessageService
    {
        /// <summary>
        /// Método que cadastrar uma rota
        /// </summary>
        /// <param name="route">Entidade que será gravada</param>
        /// <param name="rotaDoConnect"> Rota de comunicação do connect</param>
        Task Cadastrar(ApiRoute route, string rotaDoConnect);

        /// <summary>
        /// Método que obtém todas as rotas
        /// </summary>
        /// <param name="rotaPrincipal">Rota de acesso principal</param>
        /// <returns>Retorna todas as rotas disponíveis</returns>
        Task<List<ApiRoute>> ObterRotas(string rotaPrincipal);
    }
}
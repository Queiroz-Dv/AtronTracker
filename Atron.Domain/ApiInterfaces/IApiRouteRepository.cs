using Atron.Domain.ApiEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.ApiInterfaces
{
    public interface IApiRouteRepository
    {
        Task CriarRotaRepositoryAsync(ApiRoute apiRoute);
        Task<IEnumerable<ApiRoute>> ObterTodasRotas();
    }
}
using Atron.Domain.ApiEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.ApiInterfaces
{
    public interface IApiRouteRepository
    {
        Task<IEnumerable<ApiRoute>> ObterTodasRotas();
    }
}
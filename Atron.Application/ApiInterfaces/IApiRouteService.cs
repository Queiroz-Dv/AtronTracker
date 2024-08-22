using Atron.Domain.ApiEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.ApiInterfaces
{
    public interface IApiRouteService
    {
        Task<IEnumerable<ApiRoute>> ObterTodasRotasServiceAsync();
    }
}
using Atron.Application.ApiInterfaces;
using Atron.Domain.ApiEntities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiRouteConnectController : Controller
    {
        private readonly IApiRouteService _apiRouteService;

        public ApiRouteConnectController(IApiRouteService apiRouteService)
        {
            _apiRouteService = apiRouteService;
        }

        [HttpGet("MontarRotas/{modulo}")]
        public async Task<ActionResult<IEnumerable<ApiRoute>>> Get(string modulo)
        {
            var rotas =  _apiRouteService.MontarRotasPorModuloService(modulo);
            return Ok(rotas);
        }

        [HttpGet("ObterRotaPadrao/{modulo}")]
        public async Task<ActionResult<ApiRoute>> GetRotaPadrao(string modulo)
        {
            var rotas = _apiRouteService.ObterRotaPorModulo(modulo);
            return Ok(rotas);
        }
    }
}
using Atron.Application.ApiInterfaces;
using Atron.Domain.ApiEntities;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiRouteConnectController(IApiRouteService apiRouteService, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _apiRouteService = apiRouteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiRoute>>> Get()
        {
            var rotas = await _apiRouteService.ObterTodasRotasServiceAsync();
            return rotas is not null ? Ok(rotas) : NoContent();
        }

        [HttpGet("GetBaseUrl")]
        public  async Task<ActionResult> GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host.Value}";
            return Ok(baseUrl);
        }
    }
}
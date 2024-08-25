using Atron.Application.ApiInterfaces;
using Atron.Domain.ApiEntities;
using Microsoft.AspNetCore.Mvc;
using Notification.Models;
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

        [HttpGet("ObterRotas")]
        public async Task<ActionResult<IEnumerable<ApiRoute>>> Get()
        {
            var rotas = await _apiRouteService.ObterTodasRotasServiceAsync();
            return rotas is not null ? Ok(rotas) : NoContent();
        }

        [HttpPost("CriarRota")]
        public async Task<ActionResult> Post([FromBody] ApiRoute apiRoute)
        {
            await _apiRouteService.CriarRotaAsync(apiRoute);
            return !_apiRouteService.Messages.HasErrors() ?
                Ok(_apiRouteService.Messages) : BadRequest(_apiRouteService.Messages);
        }
    }
}
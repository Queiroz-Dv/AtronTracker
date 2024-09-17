using Atron.Domain.ApiEntities;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.DTO.API;
using Shared.Interfaces;

namespace Atron.WebViews.Controllers
{
    public class HomeController : Controller
    {
        private readonly RotaDeAcesso _appSettingsConfig;
        private IConfiguration _configuration;
        private IApiRouteExternalService _externalService;

        private string RotaFixas { get; set; }

        

        [HttpGet]
        public IActionResult Index(string searchString)
        {
            
            return View();
        }
    }
}

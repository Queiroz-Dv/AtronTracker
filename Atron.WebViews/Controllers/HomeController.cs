using Atron.Domain.ApiEntities;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.DTO.API;
using Shared.Interfaces;

namespace Atron.WebViews.Controllers
{
    public class HomeController : DefaultController<ApiRoute>
    {
        private readonly RotaDeAcesso _appSettingsConfig;
        private IConfiguration _configuration;
        private IApiRouteExternalService _externalService;

        private string RotaFixas { get; set; }

        public HomeController(
            IOptions<RotaDeAcesso> appSettingsConfig,
            IConfiguration configuration,
            IApiRouteExternalService externalService,
            IPaginationService<ApiRoute> paginationService,
            IResultResponseService responseService) :
            base(paginationService,
                responseService,
                externalService,
                configuration,
                appSettingsConfig)
        {
            _appSettingsConfig = appSettingsConfig.Value;
            _externalService = externalService;
            _configuration = configuration;
            CurrentController = nameof(HomeController).Replace(nameof(Controller), "");
        }

        [HttpGet]
        public IActionResult Index(string searchString)
        {
            ConfigureDataTitleForView("Atron Tracker");
            return View();
        }
    }
}

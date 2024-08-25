using Atron.Domain.ApiEntities;
using Communication.Extensions;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.DTO.API;
using Shared.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class HomeController : DefaultController<ApiRoute>
    {
        private readonly AppSettingsConfigShared _appSettingsConfig;
        private IConfiguration _configuration;
        private IApiRouteExternalService _externalService;
        private string RotaFixas { get; set; }

        public HomeController(
            IOptions<AppSettingsConfigShared> appSettingsConfig,
            IConfiguration configuration,
            IApiRouteExternalService externalService,
            IPaginationService<ApiRoute> paginationService,
            IResultResponseService responseService) : base(paginationService, responseService)
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

        [HttpGet]
        public async Task<IActionResult> PainelDeRotas(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de rotas de conexão com a API");
            var rotasFixas = _appSettingsConfig.RotasFixas;

            //var rotas = await _externalService.ObterRotas(RotaFixas);
            return View();

            //if (!rotas.Any())
            //{
            //}

            //Filter = filter;
            //ConfigurePaginationForView(rotas, itemPage, CurrentController, filter);

            //rotas = GetEntitiesPaginated();
            //ViewBag.PageInfo = PageInfo;

            //return View(rotas);
        }

        [HttpGet]
        public IActionResult CadastrarRota()
        {
            ConfigureDataTitleForView("Cadastro de rotas");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarRota(ApiRoute route)
        {
            if (ModelState.IsValid)
            {
                var rotaDoConnect = _configuration["RotaDoConnect"];

                await _externalService.Cadastrar(route, rotaDoConnect);
                var responses = _externalService.ResultResponses;
                CreateTempDataNotifications(responses);
                return !responses.HasErrors() ? RedirectToAction(nameof(CadastrarRota)) : View(nameof(CadastrarRota), route);
            }

            ConfigureDataTitleForView("Cadastro de rotas");
            return View(route);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Atron.WebViews.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //   private readonly IApiRouteService _apiRouteService;

        public HomeController(ILogger<HomeController> logger)
        {

            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string searchString)
        {
            ViewData["Title"] = "Atron Tracker";
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> CadastrarRotas()
        //{
        //    ViewData["Title"] = "Cadastro de Rotas";
        //   // var routes = await _apiRouteService.ObterTodasRotasServiceAsync();
        //    return View(routes); // Certifique-se de que a view espera uma lista de rotas
        //}

        //[HttpPost]
        //public async Task<IActionResult> CadastrarRotas(ApiRoute route)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //await _apiRouteService.(route);
        //        return RedirectToAction("CadastrarRotas");
        //    }
        //    return View(route);
        //}
    }
}

using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class CargoController : Controller
    {
        private IDepartamentoExternalService _departamentoService;
        private ICargoExternalService _cargoExternalService;
        private readonly PaginationService _paginationService;

        public CargoController(IDepartamentoExternalService departamentoService, ICargoExternalService cargoExternalService,PaginationService paginationService)
        {
            _departamentoService = departamentoService;
            _cargoExternalService = cargoExternalService;
            _paginationService = paginationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ViewData["Title"] = "Painel de cargos";
            var cargos = await _cargoExternalService.ObterTodos();

            if (!cargos.Any())
            {
                return View();
            }


            return View(cargos);
        }
    }
}

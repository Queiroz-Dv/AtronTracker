using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Enums;
using Shared.Extensions;
using Shared.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class CargoController : Controller
    {
        private IDepartamentoExternalService _departamentoService;
        private ICargoExternalService _cargoExternalService;
        private readonly PaginationService _paginationService;

        public List<ResultResponse> ResultResponses { get; set; }

        public CargoController(IDepartamentoExternalService departamentoService,
            ICargoExternalService cargoExternalService,
            PaginationService paginationService)
        {
            _departamentoService = departamentoService;
            _cargoExternalService = cargoExternalService;
            _paginationService = paginationService;
            ResultResponses = new List<ResultResponse>();
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

            var pageInfo = _paginationService.Paginate(cargos, itemPage, nameof(Cargo), filter);
            var model = new CargoModel()
            {
                Cargos = _paginationService.GetEntityPaginated(cargos, filter),
                PageInfo = pageInfo
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ViewData["Title"] = "Cadastro de cargos";

            var departamentos = await _departamentoService.ObterTodos();

            if (!departamentos.Any())
            {
                ResultResponses.Add(new ResultResponse() { Message = "Para criar um cargo é necessário ter um departamento.", Level = ResultResponseEnum.Error.GetEnumDescription() });
                var result = JsonConvert.SerializeObject(ResultResponses);
                TempData["Notifications"] = result;
                return RedirectToAction(nameof(Index));
            }

            var departamentosFiltrados = departamentos.Select(dpt =>
                new
                {
                    dpt.Codigo,
                    Descricao = $"{dpt.Codigo} - {dpt.Descricao}"
                }).ToList();

            ViewBag.Departamentos = new SelectList(departamentosFiltrados, "Codigo", "Descricao");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(CargoDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _cargoExternalService.Criar(model);
                ResultResponses.AddRange(response.responses);

                var responseSerialized = JsonConvert.SerializeObject(ResultResponses);
                TempData["Notifications"] = responseSerialized;
                return response.isSucess ? RedirectToAction(nameof(Cadastrar)) : View();
            }

            ResultResponses.Add(new ResultResponse()
            {
                Message = "Registro inválido para gravação. Tente novamente.",
                Level = ResultResponseEnum.Error.GetEnumDescription()
            });

            var result = JsonConvert.SerializeObject(ResultResponses);
            TempData["Notifications"] = result;
            return RedirectToAction(nameof(Cadastrar));
        }
    }
}

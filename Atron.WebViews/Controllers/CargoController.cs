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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class CargoController : DefaultController<CargoDTO>
    {
        private IDepartamentoExternalService _departamentoService;
        private ICargoExternalService _cargoExternalService;


        public List<ResultResponse> ResultResponses { get; set; }

        public CargoController(IDepartamentoExternalService departamentoService,
            ICargoExternalService cargoExternalService)
        {
            _departamentoService = departamentoService;
            _cargoExternalService = cargoExternalService;
            CurrentController = nameof(Cargo);
            ResultResponses = new List<ResultResponse>();
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ConfigureViewDataTitle("Painel de cargos");
            var cargos = await _cargoExternalService.ObterTodos();

            if (!cargos.Any())
            {
                return View();
            }

            Filter = filter;
            ConfigureEntitiesForView(cargos, itemPage);
            var model = new CargoModel()
            {
                Cargos = GetEntities(),
                PageInfo = PageInfo
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
                ResultResponses.Add(new ResultResponse() { Message = "Para criar um cargo é necessário ter um departamento.", Level = ResultResponseLevelEnum.Error.GetEnumDescription() });
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
                Level = ResultResponseLevelEnum.Error.GetEnumDescription()
            });

            var result = JsonConvert.SerializeObject(ResultResponses);
            TempData["Notifications"] = result;
            return RedirectToAction(nameof(Cadastrar));
        }
    }
}
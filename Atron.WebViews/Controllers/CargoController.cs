using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
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

            if (!string.IsNullOrEmpty(filter))
            {
                cargos = cargos.Where(dpt => dpt.Codigo.Contains(filter)).ToList();
            }

            var pageInfo = _paginationService.Paginate(cargos, itemPage, nameof(Cargo), filter);
            var model = new CargoModel()
            {
                Cargos = cargos,
                PageInfo = pageInfo
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar(string filter = "", int itemPage = 1)
        {
            ViewData["Title"] = "Cadastro de cargos";
            var departamentos = await _departamentoService.ObterTodos();

            if (departamentos is null || departamentos.Count() is 0)
            {
                //TempData["NotificacaoDeErro"] = JsonConvert.SerializeObject(("Para criar um cargo é necessário ter um departamento.", EGuardianMessageType.Error));
                return RedirectToAction(nameof(Index));
            }

            var pageInfo = _paginationService.Paginate(departamentos, itemPage, nameof(Cargo), filter, nameof(Cadastrar));
            var entityPaginated = _paginationService.GetEntityPaginated(departamentos);

            var model = new CargoModel();
            model.Departamentos.AddRange(entityPaginated);
            model.PageInfo = pageInfo;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(CargoModel model)
        {
            var cargoDTO = new CargoDTO();
            cargoDTO.Codigo = model.Cargo.Codigo;
            cargoDTO.Descricao = model.Cargo.Descricao;
            cargoDTO.DepartamentoCodigo = model.Cargo.DepartamentoCodigo;


            var response = await _cargoExternalService.Criar(cargoDTO);
            ResultResponses.AddRange(response.responses);

            var responseSerialized = JsonConvert.SerializeObject(ResultResponses);
            TempData["Notifications"] = responseSerialized;
            return response.isSucess ? RedirectToAction(nameof(Cadastrar)) : View(nameof(Cadastrar), cargoDTO);


            //ViewData["Title"] = "Cadastro de departamentos";
            //return View(new { filter = "", itemPage = 1 });
        }

        [HttpGet]
        public async Task<IActionResult> FiltrarDepartamentos(string filter = "", int itemPage = 1)
        {
            var departamentos = await _departamentoService.ObterTodos();

            var pageInfo = _paginationService.Paginate(departamentos, itemPage, nameof(Cargo), filter, nameof(Cadastrar));
            var departamentosModel = new DepartamentoModel()
            {
                Departamentos = _paginationService.GetEntityPaginated(departamentos, filter),
                PageInfo = pageInfo
            };

            return PartialView("Partials/Departamento/DepartamentosTablePartial", departamentosModel);
        }

    }
}

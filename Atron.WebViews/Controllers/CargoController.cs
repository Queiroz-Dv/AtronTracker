using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
using Communication.Extensions;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Models;
using Shared.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class CargoController : DefaultController<CargoDTO>
    {
        private IDepartamentoExternalService _departamentoService;
        private ICargoExternalService _cargoExternalService;

        public CargoController(
            PaginationService<CargoDTO> paginationService,
            ResultResponseModel resultResponseModel,
            IDepartamentoExternalService departamentoService,
            ICargoExternalService cargoExternalService) : base(paginationService, resultResponseModel)
        {
            _departamentoService = departamentoService;
            _cargoExternalService = cargoExternalService;
            CurrentController = nameof(Cargo);
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
                ResponseModel.AddError("Para criar um cargo é necessário ter um departamento.");
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
                await _cargoExternalService.Criar(model);

                var responses = _cargoExternalService.ResultResponses;
                CreateTempDataNotifications(responses);
                return !responses.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
            }

            ResponseModel.AddError("Registro inválido para gravação. Tente novamente.");
            CreateTempDataNotifications(ResponseModel.ResultMessages);

            return RedirectToAction(nameof(Cadastrar));
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
            if (codigo is null)
            {
                ResponseModel.AddError("O código informado não foi encontrado");
                return View(codigo);
            }

            var cargos = await _cargoExternalService.ObterTodos();
            var cargoDTO = cargos.FirstOrDefault(crg => crg.Codigo == codigo);

            var departamentos = await _departamentoService.ObterTodos();

            var departamentosFiltrados = departamentos.Select(dpt =>
                new
                {
                    dpt.Codigo,
                    Descricao = $"{dpt.Codigo} - {dpt.Descricao}"
                }).ToList();

            ViewBag.Departamentos = new SelectList(departamentosFiltrados, "Codigo", "Descricao");
            ViewBag.CodigoDoDepartamentoRelacionado = cargoDTO.DepartamentoCodigo;
            ConfigureViewDataTitle("Atualizar informação de cargo");
            return View(cargoDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string codigo, CargoDTO cargoDTO)
        {
            if (ModelState.IsValid)
            {
                await _cargoExternalService.Atualizar(codigo, cargoDTO);
            }
            else
            {
                ResponseModel.AddError("Registro inválido tente novamente");
                CreateTempDataNotifications(ResponseModel.ResultMessages);
                return RedirectToAction(nameof(Index));
            }

            CreateTempDataNotifications(_cargoExternalService.ResultResponses);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                ResponseModel.AddError("Código não informado, tente novamente.");
                CreateResultNotifications();
                return RedirectToAction(nameof(Index));
            }

            await _cargoExternalService.Remover(codigo);
            CreateTempDataNotifications(_cargoExternalService.ResultResponses);
            return RedirectToAction(nameof(Index));
        }
    }
}
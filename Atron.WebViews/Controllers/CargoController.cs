using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
using Communication.Extensions;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.DTO.API;
using Shared.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class CargoController : DefaultController<CargoDTO>
    {
        private IDepartamentoExternalService _departamentoService;
        private ICargoExternalService _cargoExternalService;

        public CargoController(
            IPaginationService<CargoDTO> paginationService,
            IResultResponseService responseModel,
            IApiUri apiUri,
            IApiRouteExternalService apiRouteExternalService,
            IDepartamentoExternalService departamentoService,            
            ICargoExternalService cargoExternalService,
            IConfiguration configuration,
            IOptions<RotaDeAcesso> appSettingsConfig)
            : base(paginationService,
                  responseModel, 
                  cargoExternalService, 
                  apiRouteExternalService, 
                  configuration, 
                  appSettingsConfig )
        {
            _departamentoService = departamentoService;
            _cargoExternalService = cargoExternalService;
            CurrentController = nameof(Cargo);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de cargos");
            var cargos = await _cargoExternalService.ObterTodos();

            if (!cargos.Any())
            {
                return View();
            }

            Filter = filter;
            ConfigurePaginationForView(cargos, itemPage, CurrentController, filter);
            var model = new CargoModel()
            {
                Cargos = GetEntitiesPaginated(),
                PageInfo = PageInfo
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de cargos");

            var departamentos = await _departamentoService.ObterTodos();

            if (!departamentos.Any())
            {
                _responseService.AddError("Para criar um cargo é necessário ter um departamento.");
                CreateTempDataNotifications();
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

            _responseService.AddError("Registro inválido para gravação. Tente novamente.");
            CreateTempDataNotifications();

            return RedirectToAction(nameof(Cadastrar));
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
            if (codigo is null)
            {
                _responseService.AddError("O código informado não foi encontrado");
                CreateTempDataNotifications();
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
            ConfigureDataTitleForView("Atualizar informação de cargo");
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
                _responseService.AddError("Registro inválido tente novamente");
                CreateTempDataNotifications();
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
                _responseService.AddError("Código não informado, tente novamente.");
                CreateTempDataNotifications();
                return RedirectToAction(nameof(Index));
            }

            await _cargoExternalService.Remover(codigo);
            CreateTempDataNotifications(_cargoExternalService.ResultResponses);
            return RedirectToAction(nameof(Index));
        }
    }
}
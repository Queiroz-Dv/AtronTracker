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
using Shared.Models;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class CargoController : DefaultController<CargoDTO, Cargo, ICargoExternalService>
    {
        private IDepartamentoExternalService _departamentoService;

        public CargoController(
            IUrlModuleFactory urlFactory,
            IPaginationService<CargoDTO> paginationService,
            ICargoExternalService cargoExternalService,
            IApiRouteExternalService apiRouteExternalService,
            IConfiguration configuration,
            IOptions<RotaDeAcesso> appSettingsConfig,
            MessageModel<Cargo> messageModel,
            IDepartamentoExternalService departamentoExternalService
            )
            : base(urlFactory,
                  paginationService,
                  cargoExternalService,
                  apiRouteExternalService,
                  configuration,
                  appSettingsConfig,
                  messageModel)
        {
            _departamentoService = departamentoExternalService;
            CurrentController = nameof(Cargo);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            await BuildRoute(nameof(Cargo));

            var cargos = await _service.ObterTodos();

            Filter = filter;
            ConfigurePaginationForView(cargos, itemPage, CurrentController, filter);
            var model = new CargoModel()
            {
                Cargos = GetEntitiesPaginated(),
                PageInfo = PageInfo
            };

            ConfigureDataTitleForView("Painel de cargos");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de cargos");
            await BuildRoute(nameof(Departamento));
            var departamentos = await _departamentoService.ObterTodos();

            if (!departamentos.Any())
            {
                _messageModel.AddError("Para criar um cargo é necessário ter um departamento.");
                CreateTempDataMessages();
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
                await BuildRoute(nameof(Cargo));

                await _service.Criar(model);

                CreateTempDataMessages();
                return !_messageModel.Messages.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
            }

            _messageModel.AddError("Registro inválido para gravação. Tente novamente.");
            CreateTempDataMessages();

            return RedirectToAction(nameof(Cadastrar));
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
            if (codigo is null)
            {
                _messageModel.AddError("O código informado não foi encontrado");
                CreateTempDataMessages();
                return View(nameof(Index));
            }

            await BuildRoute(nameof(Cargo), codigo);
            var cargoDTO = await _service.ObterPorCodigo(codigo);

            await BuildRoute(nameof(Departamento));
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
                await BuildRoute(nameof(Cargo), codigo);
                await _service.Atualizar(codigo, cargoDTO);
            }
            else
            {
                _messageModel.AddError("Registro inválido tente novamente");
                CreateTempDataMessages();
                return View(nameof(Index));
            }

            CreateTempDataMessages();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            await BuildRoute(nameof(Cargo), codigo);
            await _service.Remover(codigo);

            CreateTempDataMessages();
            return RedirectToAction(nameof(Index));
        }
    }
}
using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.DTO;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    [Authorize]
    public class CargoController : MainController<CargoDTO, Cargo>
    {
        private readonly IExternalService<CargoDTO> _service;
        private readonly IExternalService<DepartamentoDTO> _departamentoService;

        public CargoController(
            IExternalService<CargoDTO> service,
            IPaginationService<CargoDTO> paginationService,
            IExternalService<DepartamentoDTO> departamentoService,
            IRouterBuilderService router,
            MessageModel messageModel)
            : base(messageModel, paginationService)
        {
            _departamentoService = departamentoService;
            _service = service;
            _router = router;
            ApiControllerName = nameof(Cargo);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de cargos");

            BuildRoute();
            var cargos = await _service.ObterTodos();

            ConfigurePaginationForView(cargos, new PageInfoDTO()
            {
                CurrentPage = itemPage,
                PageRequestInfo = new PageRequestInfoDTO()
                {
                    CurrentViewController = ApiControllerName,
                    Action = nameof(Index),
                    Filter = filter,
                }
            });

            var model = new CargoModel()
            {
                Cargos = _paginationService.GetEntitiesFilled(),
                PageInfo = _paginationService.GetPageInfo()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de cargos");

            await FetchDepartamentosData();
            return _messageModel.Messages.HasErrors() ? RedirectToAction(nameof(Index)) : View();
        }

        private async Task FetchDepartamentosData()
        {
            ApiControllerName = nameof(Departamento);
            BuildRoute();
            var departamentos = await _departamentoService.ObterTodos();

            if (!departamentos.Any())
            {
                _messageModel.AddError("Para criar um cargo é necessário ter um departamento.");
                CreateTempDataMessages();
            }

            var departamentosFiltrados = departamentos.Select(dpt =>
                new
                {
                    dpt.Codigo,
                    Descricao = $"{dpt.Codigo} - {dpt.Descricao}"
                }).ToList();

            ViewBag.Departamentos = new SelectList(departamentosFiltrados, "Codigo", "Descricao");
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(CargoDTO model)
        {
            if (ModelState.IsValid)
            {
                BuildRoute();

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
            ConfigureDataTitleForView("Atualizar informação de cargo");
            if (codigo is null)
            {
                _messageModel.AddError("O código informado não foi encontrado");
                CreateTempDataMessages();
                return View(nameof(Index));
            }

            BuildRoute();
            var cargoDTO = await _service.ObterPorCodigo(codigo);

            await FetchDepartamentosData();

            ViewBag.CodigoDoDepartamentoRelacionado = cargoDTO.DepartamentoCodigo;
            return View(cargoDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string codigo, CargoDTO cargoDTO)
        {
            if (ModelState.IsValid)
            {
                BuildRoute();
                await _service.Atualizar(codigo, cargoDTO);
                CreateTempDataMessages();
                return RedirectToAction(nameof(Index));
            }

            _messageModel.AddError("Registro inválido tente novamente");
            CreateTempDataMessages();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            BuildRoute();
            await _service.Remover(codigo);

            CreateTempDataMessages();
            return RedirectToAction(nameof(Index));
        }
    }
}
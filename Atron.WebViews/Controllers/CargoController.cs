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
using System;
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

        [HttpGet]
        public async Task<IActionResult> MenuPrincipal(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de cargos");
            BuildRoute();
            var cargos = await _service.ObterTodos();

            ConfigurePaginationForView(cargos, nameof(MenuPrincipal), itemPage, filter);
            return View(GetModel<CargoModel>());            
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de cargos");

            await FetchDepartamentosData();
            return _messageModel.Notificacoes.HasErrors() ? RedirectToAction(nameof(MenuPrincipal)) : View();
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
                return;
            }

            var departamentosFiltrados = departamentos.Select(dpt =>
                new
                {
                    dpt.Codigo,
                    Descricao = $"{dpt.Codigo} - {dpt.Descricao}"
                }).ToList();

            ViewBag.Departamentos = new SelectList(departamentosFiltrados, nameof(Departamento.Codigo), nameof(Departamento.Descricao));
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(CargoDTO model)
        {
            ConfigureDataTitleForView("Cadastro de cargos");

            if (ModelState.IsValid)
            {
                BuildRoute();

                await _service.Criar(model);

                CreateTempDataMessages();
                return !_messageModel.Notificacoes.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
            }

            CreateTempDataMessages();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
            ConfigureDataTitleForView("Atualizar informação de cargo");
            if (codigo is null)
            {
                _messageModel.AddError("O código informado não foi encontrado");
                CreateTempDataMessages();
                return View(nameof(MenuPrincipal));
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
                return RedirectToAction(nameof(MenuPrincipal));
            }

            _messageModel.AddError("Registro inválido tente novamente");
            CreateTempDataMessages();
            return RedirectToAction(nameof(MenuPrincipal));
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            BuildRoute();
            await _service.Remover(codigo);

            CreateTempDataMessages();
            return RedirectToAction(nameof(MenuPrincipal));
        }

        public override Task<string> ObterMensagemExclusao()
        {
            return Task.FromResult("Ao excluir esse cargo os departamentos associados a ele serão excluídos e os usuários podem não ter acesso a alguns módulos." +
                                    " Deseja prosseguir ?");
        }
    }
}
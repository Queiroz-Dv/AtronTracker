using Application.DTO;
using Atron.WebViews.Models;
using Communication.Interfaces.Services;
using Domain.Entities;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    [Authorize]
    public class DepartamentoController : MainController<DepartamentoDTO, Departamento>
    {
        private readonly IExternalService<DepartamentoDTO> _service;

        public DepartamentoController(
            IExternalService<DepartamentoDTO> service,
            IPaginationService<DepartamentoDTO> paginationService,
            IRouterBuilderService router,
            MessageModel messageModel)
            : base(messageModel, paginationService)
        {
            _paginationService = paginationService;
            _service = service;
            _router = router;
            ApiControllerName = nameof(Departamento);
        }

        [HttpGet]
        public async Task<IActionResult> MenuPrincipal(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de departamentos");
            BuildRoute();
            var departamentos = await _service.ObterTodos();

            ConfigurePaginationForView(departamentos, nameof(MenuPrincipal), itemPage, filter);
            return View(GetModel<DepartamentoModel>());
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de departamentos");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(DepartamentoDTO departamento)
        {
            ConfigureDataTitleForView("Cadastro de departamentos");

            if (ModelState.IsValid)
            {
                BuildRoute();

                await _service.Criar(departamento);

                CreateTempDataMessages();

                return !_messageModel.Notificacoes.HasErrors() ?
                    RedirectToAction(nameof(Cadastrar)) :
                    View(nameof(Cadastrar), departamento);
            }

            return View(departamento);
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
            BuildRoute();
            var departamentoDTO = await _service.ObterPorCodigo(codigo);

            if (departamentoDTO is null || _messageModel.Notificacoes.HasErrors())
            {
                CreateTempDataMessages();
                return RedirectToAction(nameof(MenuPrincipal));
            }

            ConfigureDataTitleForView("Atualizar informação de departamento");
            return View(departamentoDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string codigo, DepartamentoDTO departamentoDTO)
        {
            BuildRoute();
            await _service.Atualizar(codigo, departamentoDTO);
            CreateTempDataMessages();

            ConfigureDataTitleForView("Atualizar informação de departamento");

            return !_messageModel.Notificacoes.HasErrors() ?
            RedirectToAction(nameof(MenuPrincipal)) :
            View(nameof(Atualizar), departamentoDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            BuildRoute();
            await _service.Remover(codigo);

            CreateTempDataMessages();
            return RedirectToAction(nameof(MenuPrincipal));
        }

        [HttpGet]
        public async Task<IActionResult> ObterDepartamento(string codigoDepartamento)
        {
            var departamento = await _service.ObterPorCodigo(codigoDepartamento);

            return Ok(departamento);
        }

        public override Task<string> ObterMensagemExclusao()
        {
            return Task.FromResult("Ao excluir esse departamento os cargos associados a ele serão excluídos e os usuários podem não ter acesso a alguns módulos." +
                                    " Deseja prosseguir ?");
        }
    }
}
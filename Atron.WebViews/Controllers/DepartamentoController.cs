using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
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

        public async override Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de departamentos");
            BuildRoute();
            var departamentos = await _service.ObterTodos();

            ConfigurePaginationForView(departamentos, nameof(Index), itemPage, filter);
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

                return !_messageModel.Messages.HasErrors() ?
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

            if (departamentoDTO is null || _messageModel.Messages.HasErrors())
            {
                CreateTempDataMessages();
                return RedirectToAction(nameof(Index));
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

            return !_messageModel.Messages.HasErrors() ?
            RedirectToAction(nameof(Index)) :
            View(nameof(Atualizar), departamentoDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            BuildRoute();
            await _service.Remover(codigo);

            CreateTempDataMessages();
            return RedirectToAction(nameof(Index));
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
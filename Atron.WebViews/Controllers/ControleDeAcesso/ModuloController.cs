using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models.ControleDeAcesso;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers.ControleDeAcesso
{
    [Authorize]
    public class ModuloController : MainController<ModuloDTO, Modulo>
    {
        private readonly IExternalService<ModuloDTO> _service;

        public ModuloController(
            IPaginationService<ModuloDTO> paginationService,
            IExternalService<ModuloDTO> service,
            IRouterBuilderService router,
            MessageModel messageModel) : base (messageModel, paginationService)
        {
            _service = service;
            _router = router;
            ApiControllerName = nameof(Modulo);
        }

        [HttpGet]
        public async Task<IActionResult> MenuPrincipal(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de módulos");

            BuildRoute();
            var modulos = await _service.ObterTodos();

            ConfigurePaginationForView(modulos, nameof(MenuPrincipal), itemPage, filter);

            return View(GetModel<ModuloModel>());
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de módulos");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(ModuloDTO modulo)
        {
            if (ModelState.IsValid)
            {
                BuildRoute();
                await _service.Criar(modulo);

                CreateTempDataMessages();
                return !_messageModel.Messages.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
            BuildRoute();
            var moduloDTO = await _service.ObterPorCodigo(codigo);
            ConfigureDataTitleForView("Atualizar informação de módulo");

            return moduloDTO is null ?
                    RedirectToAction(nameof(MenuPrincipal))
                    : View(moduloDTO);
          
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string codigo, ModuloDTO moduloDTO)
        {
            BuildRoute();
            await _service.Atualizar(codigo, moduloDTO);

            ConfigureDataTitleForView("Atualizar informação de módulo");
            CreateTempDataMessages();

            return !_messageModel.Messages.HasErrors() ?
            RedirectToAction(nameof(MenuPrincipal)) :
            View(nameof(Atualizar), moduloDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            BuildRoute();
            await _service.Remover(codigo);

            CreateTempDataMessages();
            return RedirectToAction(nameof(MenuPrincipal));
        }
    }
}
using Atron.Application.DTO;
using Atron.Domain.Componentes;
using Atron.Domain.Entities;
using Atron.WebViews.Models.ControleDeAcesso;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers.ControleDeAcesso
{
    [Authorize]
    public class PerfilDeAcessoController : MainController<PerfilDeAcessoDTO, PerfilDeAcesso>
    {
        private readonly IExternalService<PerfilDeAcessoDTO> _service;
        private readonly IExternalService<ModuloDTO> _moduloService;

        public PerfilDeAcessoController(
                        MessageModel messageModel,
                        IExternalService<PerfilDeAcessoDTO> service,
                        IExternalService<ModuloDTO> moduloService,
                        IRouterBuilderService router,
                        IPaginationService<PerfilDeAcessoDTO> paginationService) : base(messageModel, paginationService)
        {
            _service = service;
            _moduloService = moduloService;

            _router = router;

            ApiControllerName = nameof(PerfilDeAcesso);
        }

        [HttpGet]
        public Task<IActionResult> GerenciamentoDeAcesso()

        {
            ConfigureDataTitleForView("Gerenciamento de acesso");
            return Task.FromResult((IActionResult)View());
        }

        [HttpGet]
        public async Task<IActionResult> MenuPerfilDeAcesso(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Gerenciamento de perfis de acesso");

            BuildRoute();
            var perfis = await _service.ObterTodos();

            ConfigurePaginationForView(perfis, nameof(MenuPerfilDeAcesso), itemPage, filter);
            return View(GetModel<PerfilDeAcessoModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de perfil de acesso");
            BuildRoute("ObterModulos");
            var modulos = await _moduloService.ObterTodos();
            ViewBag.Modulos = modulos;
            ConfigurarGridTableModulos(modulos);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            if (ModelState.IsValid)
            {
                BuildRoute();
                await _service.Criar(perfilDeAcessoDTO);

                CreateTempDataMessages();
                return !_messageModel.Messages.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
            }

            return View();
        }

        private void ConfigurarGridTableModulos(List<ModuloDTO> modulos)
        {
            string legendTitle = "Módulos";
            string[] entityColumns = { "Codigo", "Descricao" };
            string[] childrenColumns = { "Codigo" };
            IEnumerable<object> entitiesObjects = modulos;

            IEnumerable<object> childrenObjects = new List<PropriedadesDeFluxo>()
            {
                new()
                {   Id = 1,
                    Codigo = "GRAVAR",
                },
                new()
                {
                    Id=2,
                    Codigo = "EXCLUIR",
                },
                new()
                {
                    Id=3,
                    Codigo = "ATUALIZAR",
                },
                new()
                {
                    Id=4,
                    Codigo = "CONSULTAR",
                },
            };

            ViewData["LegendTitle"] = legendTitle;
            ViewData["EntityColumns"] = entityColumns;
            ViewData["Entities"] = entitiesObjects;
            ViewData["IsMultiSelect"] = true;
            ViewData["HasChildren"] = true;
            ViewData["IsChildrenMultiSelect"] = true;
            ViewData["ChildrenColumns"] = childrenColumns;
            ViewData["ChildrenEntities"] = childrenObjects;
        }
    }
}
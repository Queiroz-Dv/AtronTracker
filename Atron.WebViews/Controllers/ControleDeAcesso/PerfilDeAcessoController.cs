using Application.DTO;
using Atron.WebViews.Models;
using Atron.WebViews.Models.ControleDeAcesso;
using Communication.Interfaces.Services;
using Domain.Entities;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers.ControleDeAcesso
{
    [Authorize]
    public class PerfilDeAcessoController : MainController<PerfilDeAcessoDTO, PerfilDeAcesso>
    {
        private readonly IExternalService<PerfilDeAcessoDTO> _service;
        private readonly IExternalService<ModuloDTO> _moduloService;

        public PerfilDeAcessoController(
                        Notifiable messageModel,
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

        private GenericGridViewModel ConfigurarGridTableModulos(List<ModuloDTO> modulos)
        {
            var gridViewModel = new GenericGridViewModel
            {
                LegendTitle = "Módulos",
                EntityColumns = new List<string> { "Codigo", "Descricao" },
                ChildrenColumns = new List<string> { "Codigo" },
                IsMultiSelect = true,
                HasChildren = true,
                IsChildrenMultiSelect = true,
                Entities = modulos.Select(m => new GridItem
                {
                    Id = m.Id,
                    Code = m.Codigo,
                    Values = new Dictionary<string, string>
                    {
                        { "Codigo", m.Codigo },
                        { "Descricao", m.Descricao }
                    },
                    Children = new List<GridItem>
                    {
                        new() { Id = 1, Values = new Dictionary<string, string> { { "GRV", "GRAVAR" } } },
                        new() { Id = 2, Values = new Dictionary<string, string> { { "EXEC", "EXCLUIR" } } },
                        new() { Id = 3, Values = new Dictionary<string, string> { { "ATL", "ATUALIZAR" } } },
                        new() { Id = 4, Values = new Dictionary<string, string> { { "CST", "CONSULTAR" } } }
                    }
                }).ToList()
            };

            return gridViewModel;
        }

        //public IActionResult Cadastrar()
        //{
        //    var modulos = _service.ObterTodosModulos(); // Método fictício para buscar os módulos
        //    var gridViewModel = ConfigurarGridTableModulos(modulos);
        //    return View(gridViewModel);
        //}


        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de perfil de acesso");
            BuildRoute("ObterModulos");
            var modulos = await _moduloService.ObterTodos();
            var gridViewModel = ConfigurarGridTableModulos(modulos);
            ViewBag.GridView = gridViewModel;
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Cadastrar(PerfilDeAcessoDTO perfilDeAcessoDTO)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        BuildRoute();
        //        await _service.Criar(perfilDeAcessoDTO);

        //        CreateTempDataMessages();
        //        return !_messageModel.Messages.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
        //    }

        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> Cadastrar(PerfilDeAcessoDTO perfilDeAcessoDTO, string selectedEntities, string selectedChildren)
        {
            if (ModelState.IsValid)
            {
                var modulos = selectedEntities?.Split(',').Select(int.Parse).ToList() ?? new List<int>();

                await _service.Criar(perfilDeAcessoDTO);

                CreateTempDataMessages();
                return !_messageModel.Notificacoes.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
            }

            return View();
        }


        //// Exemplo do grid
        //private void ConfigurarGridTableModulos(List<ModuloDTO> modulos)
        //{
        //    string legendTitle = "Módulos";
        //    string[] entityColumns = { "Codigo", "Descricao" };
        //    string[] childrenColumns = { "Codigo" };
        //    IEnumerable<object> entitiesObjects = modulos;

        //    IEnumerable<object> childrenObjects = new List<PropriedadesDeFluxo>()
        //    {
        //        new()
        //        {   Id = 1,
        //            Codigo = "GRAVAR",
        //        },
        //        new()
        //        {
        //            Id=2,
        //            Codigo = "EXCLUIR",
        //        },
        //        new()
        //        {
        //            Id=3,
        //            Codigo = "ATUALIZAR",
        //        },
        //        new()
        //        {
        //            Id=4,
        //            Codigo = "CONSULTAR",
        //        },
        //    };

        //    ViewData["LegendTitle"] = legendTitle;
        //    ViewData["EntityColumns"] = entityColumns;
        //    ViewData["Entities"] = entitiesObjects;
        //    ViewData["IsMultiSelect"] = true;
        //    ViewData["HasChildren"] = true;
        //    ViewData["IsChildrenMultiSelect"] = true;
        //    ViewData["ChildrenColumns"] = childrenColumns;
        //    ViewData["ChildrenEntities"] = childrenObjects;
        //}
    }
}
using Application.DTO;
using Atron.WebViews.Models;
using Communication.Interfaces.Services;
using Domain.Entities;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Interfaces.Services;
using Shared.Models;
using System.Linq;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    [Authorize]
    public class UsuarioController : MainController<UsuarioDTO, Usuario>
    {
        private readonly IExternalService<UsuarioDTO> _service;
        private readonly IExternalService<CargoDTO> _cargoService;
        private readonly IExternalService<DepartamentoDTO> _departamentoService;

        public UsuarioController(
            IExternalService<UsuarioDTO> service,
            IPaginationService<UsuarioDTO> paginationService,
            IExternalService<DepartamentoDTO> departamentoExternalService,
            IExternalService<CargoDTO> cargoExternalService,
            IRouterBuilderService router,
            MessageModel messageModel)
            : base(messageModel, paginationService)
        {
            _service = service;
            _departamentoService = departamentoExternalService;
            _cargoService = cargoExternalService;
            _router = router;
            ApiControllerName = nameof(Usuario);
        }

        [HttpGet]
        public async Task<IActionResult> MenuPrincipal(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de usuários");
            BuildRoute();
            var usuarios = await _service.ObterTodos();

            ConfigurePaginationForView(usuarios, nameof(MenuPrincipal), itemPage, filter);
            return View(GetModel<UsuarioModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de usuários");
            await FetchDepartamentosData();
            await FetchCargosData();
            return View(new UsuarioDTO());
        }

        private async Task FetchCargosData()
        {
            BuildCargoRoute();
            var cargos = await _cargoService.ObterTodos();
            var cargosFiltrados = cargos.Select(crg =>
                            new
                            {
                                crg.Codigo,
                                Descricao = crg.ObterCodigoComDescricao()
                            }).ToList();

            ViewBag.Cargos = new SelectList(cargosFiltrados, nameof(Cargo.Codigo), nameof(Cargo.Descricao));
        }

        private async Task FetchDepartamentosData()
        {
            BuildDepartamentoRoute();
            var departamentos = await _departamentoService.ObterTodos();

            var departamentosFiltrados = departamentos.Select(dpt =>
                new
                {
                    dpt.Codigo,
                    Descricao = dpt.ObterCodigoComDescricao()
                }).ToList();

            ViewBag.Departamentos = new SelectList(departamentosFiltrados, nameof(DepartamentoDTO.Codigo), nameof(DepartamentoDTO.Descricao));
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(UsuarioDTO model)
        {
            if (ModelState.IsValid)
            {
                BuildRoute();
                await _service.Criar(model);

                CreateTempDataMessages();
                return !_messageModel.Notificacoes.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObterCargosAssociados(string codigoDepartamento)
        {
            BuildCargoRoute();
            var cargos = await _cargoService.ObterTodos();

            if (!codigoDepartamento.IsNullOrEmpty())
            {
                cargos = cargos.Where(crg => crg.DepartamentoCodigo == codigoDepartamento).ToList();
            }

            var cargosFiltrados = cargos.Select(crg => new
            {
                crg.Codigo,
                Descricao = crg.ObterCodigoComDescricao()
            }).ToList();

            return Json(cargosFiltrados);
        }

        [HttpGet]
        public async Task<IActionResult> ObterDepartamentosAssociados(string codigoCargo)
        {
            BuildCargoRoute();
            var cargos = await _cargoService.ObterTodos();

            if (!codigoCargo.IsNullOrEmpty())
            {
                // Preciso obter os departamentos a partir do código do cargo;
                cargos = cargos.Where(crg => crg.Codigo == codigoCargo).ToList();
            }

            var departamentosFiltrados = cargos.Select(dpt => new
            {
                dpt.DepartamentoCodigo,
                Descricao = dpt.ObterCodigoComDescricao()
            }).ToList();

            return Json(departamentosFiltrados);
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
            ConfigureDataTitleForView("Atualizar informação de usuário");
            BuildRoute();
            var usuario = await _service.ObterPorCodigo(codigo);

            await FetchDepartamentosData();
            await FetchCargosData();

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string codigo, UsuarioDTO usuario)
        {
            BuildRoute();
            await _service.Atualizar(codigo, usuario);
            CreateTempDataMessages();

            return !_messageModel.Notificacoes.HasErrors() ?
            RedirectToAction(nameof(MenuPrincipal)) :
            View(nameof(Atualizar), usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            BuildRoute();
            await _service.Remover(codigo);
            CreateTempDataMessages();
            return RedirectToAction(nameof(MenuPrincipal));
        }

        private void BuildCargoRoute()
        {
            ApiControllerName = nameof(Cargo);
            BuildRoute();
        }

        private void BuildDepartamentoRoute()
        {
            ApiControllerName = nameof(Departamento);
            BuildRoute();
        }

        public override Task<string> ObterMensagemExclusao()
        {
            return Task.FromResult("Ao excluir o usuário todos os registros serão removidos também. Deseja prosseguir ?");
        }
    }
}
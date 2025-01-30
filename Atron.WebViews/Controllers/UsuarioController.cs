using Atron.Application.DTO;
using Atron.Application.DTO.ApiDTO;
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
    public class UsuarioController : MainController<UsuarioDTO, Usuario>
    {
        private readonly IExternalService<UsuarioDTO> _service;
        private readonly IExternalService<CargoDTO> _cargoService;
        private readonly IExternalService<DepartamentoDTO> _departamentoService;

        // Aqui eu preciso do Register para fazer o registro de usuários
        private readonly IExternalService<RegisterDTO> _registerService;

        public UsuarioController(
            IExternalService<UsuarioDTO> service,
            IPaginationService<UsuarioDTO> paginationService,
            IExternalService<DepartamentoDTO> departamentoExternalService,
            IExternalService<CargoDTO> cargoExternalService,
            IExternalService<RegisterDTO> registerService,
            IRouterBuilderService router,
            MessageModel messageModel)
            : base(messageModel, paginationService)
        {
            _service = service;
            _departamentoService = departamentoExternalService;
            _cargoService = cargoExternalService;
            _registerService = registerService;
            _router = router;
            ApiControllerName = nameof(Usuario);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de usuários");
            BuildRoute();
            var usuarios = await _service.ObterTodos();

            ConfigurePaginationForView(usuarios, new PageInfoDTO()
            {
                CurrentPage = itemPage,
                PageRequestInfo = new PageRequestInfoDTO()
                {
                    CurrentViewController = ApiControllerName,
                    Action = nameof(Index),
                    Filter = filter,
                }
            });

            var model = new UsuarioModel()
            {
                Usuarios = _paginationService.GetEntitiesFilled(),
                PageInfo = _paginationService.GetPageInfo()
            };

            return View(model);
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
                return !_messageModel.Messages.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
            }

            _messageModel.AddError("Registro inválido para gravação. Tente novamente.");
            CreateTempDataMessages();
            return RedirectToAction(nameof(Cadastrar));
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

            return !_messageModel.Messages.HasErrors() ?
            RedirectToAction(nameof(Index)) :
            View(nameof(Atualizar), usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            BuildRoute();
            await _service.Remover(codigo);
            CreateTempDataMessages();
            return RedirectToAction(nameof(Index));
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
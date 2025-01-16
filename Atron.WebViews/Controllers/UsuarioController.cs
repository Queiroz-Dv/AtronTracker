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
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class UsuarioController : MainController<UsuarioDTO, Usuario, IUsuarioExternalService>
    {
        IDepartamentoExternalService _departamentoService;
        ICargoExternalService _cargoService;

        public UsuarioController(
            IUrlModuleFactory urlModuleFactory,
            IPaginationService<UsuarioDTO> paginationService,
            IUsuarioExternalService externalService,
            IDepartamentoExternalService departamentoExternalService,
            IApiRouteExternalService apiRouteExternalService,
            IConfiguration configuration,
            IOptions<RotaDeAcesso> appSettingsConfig,
            ICargoExternalService cargoExternalService,
            MessageModel<Usuario> messageModel)
            : base(urlModuleFactory,
                   paginationService,
                   externalService,
                   apiRouteExternalService,
                   configuration,
                   appSettingsConfig,
                   messageModel)
        {
            _departamentoService = departamentoExternalService;
            _cargoService = cargoExternalService;
            ApiController = nameof(Usuario);
        }

        // Rever como automatizar o processo de montagem de rotas
        private void BuildCargoRoute()
        {
            BuildRoute(nameof(Cargo));
        }

        private void BuildDepartamentoRoute()
        {
            BuildRoute(nameof(Departamento));
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de usuários");

            BuildRoute(nameof(Usuario));
            var usuarios = await _service.ObterTodos();

            Filter = filter;
            ConfigurePaginationForView(usuarios, itemPage, ApiController, filter);
            var model = new UsuarioModel()
            {
                Usuarios = GetEntitiesPaginated(),
                PageInfo = PageInfo
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de usuários");

            BuildRoute(nameof(Departamento));
            var departamentos = await _departamentoService.ObterTodos();

            BuildRoute(nameof(Cargo));
            var cargos = await _cargoService.ObterTodos();

            MontarViewBagDepartamentos(departamentos);
            MontarViewBagCargos(cargos);
            return View();
        }

        private void MontarViewBagCargos(List<CargoDTO> cargos)
        {
            var cargosFiltrados = cargos.Select(crg =>
                            new
                            {
                                crg.Codigo,
                                Descricao =  crg.ObterCodigoComDescricao()
                            }).ToList();

            ViewBag.Cargos = new SelectList(cargosFiltrados, nameof(Cargo.Codigo), nameof(Cargo.Descricao));
        }

        private void MontarViewBagDepartamentos(List<DepartamentoDTO> departamentos)
        {
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
                BuildRoute(nameof(Usuario));
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

            BuildDepartamentoRoute();
            var departamentos = await _departamentoService.ObterTodos();

            BuildCargoRoute();
            var cargos = await _cargoService.ObterTodos();

            BuildRoute(nameof(Usuario), codigo);
            var usuario = await _service.ObterPorCodigo(codigo);

            MontarViewBagDepartamentos(departamentos);
            MontarViewBagCargos(cargos);

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string codigo, UsuarioDTO usuario)
        {
            BuildRoute(nameof(Usuario), codigo);
            await _service.Atualizar(codigo, usuario);
            CreateTempDataMessages();

            return !_messageModel.Messages.HasErrors() ?
            RedirectToAction(nameof(Index)) :
            View(nameof(Atualizar), usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            BuildRoute(nameof(Usuario), codigo);
            await _service.Remover(codigo);

            CreateTempDataMessages();
            return RedirectToAction(nameof(Index));
        }            
    }
}
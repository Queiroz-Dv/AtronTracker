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
using Newtonsoft.Json;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class UsuarioController : DefaultController<UsuarioDTO, Usuario, IUsuarioExternalService>
    {
        public DataTable DataTable { get; set; }
        IDepartamentoExternalService _departamentoService;
        ICargoExternalService _cargoService;
        IPaginationService<CargoDTO> _cargoPager;

        public UsuarioController(
            IUrlModuleFactory urlModuleFactory,
            IPaginationService<UsuarioDTO> paginationService,
            IPaginationService<CargoDTO> cargoPager,
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
            _cargoPager = cargoPager;
            DataTable = new();
            CurrentController = nameof(Usuario);
        }

        // Rever como automatizar o processo de montagem de rotas
        private async Task BuildCargoRoute()
        {
            await BuildRoute(nameof(Cargo));
        }

        private async Task BuildDepartamentoRoute()
        {
            await BuildRoute(nameof(Departamento));
        }


        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de usuários");

            await BuildRoute(nameof(Usuario));
            var usuarios = await _service.ObterTodos();

            Filter = filter;
            ConfigurePaginationForView(usuarios, itemPage, CurrentController, filter);
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

            await BuildRoute(nameof(Departamento));
            var departamentos = await _departamentoService.ObterTodos();

            await BuildRoute(nameof(Cargo));
            var cargos = await _cargoService.ObterTodos();


            if (!departamentos.Any())
            {
                _messageModel.AddWarning("Para criar um usuário é necessário cadastrar um departamento.");
                CreateTempDataMessages();
            }

            if (!cargos.Any())
            {
                _messageModel.AddWarning("Para criar um usuário é necessário cadastrar um cargo.");
                CreateTempDataMessages();
            }

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
                                Descricao = $"{crg.Codigo} - {crg.Descricao}"
                            }
                        ).ToList();

            ViewBag.Cargos = new SelectList(cargosFiltrados, "Codigo", "Descricao");
        }

        private void MontarViewBagDepartamentos(List<DepartamentoDTO> departamentos)
        {
            var departamentosFiltrados = departamentos.Select(dpt =>
                new
                {
                    dpt.Codigo,
                    Descricao = $"{dpt.Codigo} - {dpt.Descricao}"
                }).ToList();

            ViewBag.Departamentos = new SelectList(departamentosFiltrados, nameof(DepartamentoDTO.Codigo), nameof(DepartamentoDTO.Descricao));
        }

        private void PaginacaoDeCargos(int itemPage, List<CargoDTO> cargos)
        {
            _cargoPager.Paginate(cargos, itemPage, CurrentController, string.Empty, nameof(Cadastrar));
            _cargoPager.ConfigureEntityPaginated(cargos, string.Empty);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(UsuarioDTO model)
        {
            if (ModelState.IsValid)
            {
                await BuildRoute(nameof(Usuario));
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
            await BuildCargoRoute();
            var cargos = await _cargoService.ObterTodos();

            if (!codigoDepartamento.IsNullOrEmpty())
            {
                cargos = cargos.Where(crg => crg.DepartamentoCodigo == codigoDepartamento).ToList();
            }

            var cargosFiltrados = cargos.Select(crg => new
            {
                crg.Codigo,
                Descricao = $"{crg.Codigo} - {crg.Descricao}"
            }).ToList();

            return Json(cargosFiltrados);
        }

        [HttpGet]
        public async Task<IActionResult> ObterDepartamentosAssociados(string codigoCargo)
        {
            await BuildCargoRoute();
            var cargos = await _cargoService.ObterTodos();

            if (!codigoCargo.IsNullOrEmpty())
            {
                // Preciso obter os departamentos a partir do código do cargo;
                cargos = cargos.Where(crg => crg.Codigo == codigoCargo).ToList();
            }

            var departamentosFiltrados = cargos.Select(dpt => new
            {
                dpt.DepartamentoCodigo,
                Descricao = $"{dpt.Departamento.Codigo} - {dpt.Departamento.Descricao}"
            }).ToList();

            return Json(departamentosFiltrados);
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
            ConfigureDataTitleForView("Atualizar informação de usuário");

            if (codigo is null)
            {
                _messageModel.AddError("O código informado não foi encontrado");
                CreateTempDataMessages();
                return View(nameof(Index));
            }

            await BuildRoute(nameof(Departamento));
            var departamentos = await _departamentoService.ObterTodos();

            await BuildRoute(nameof(Cargo));
            var cargos = await _cargoService.ObterTodos();

            if (!departamentos.Any())
            {
                _messageModel.AddError("Para criar um usuário é necessário ter um departamento.");
                CreateTempDataMessages();
                return RedirectToAction(nameof(Index));
            }

            if (!cargos.Any())
            {
                _messageModel.AddError("Para criar um usuário é necessário ter um cargo.");
                CreateTempDataMessages();
                return RedirectToAction(nameof(Index));
            }

            cargos = cargos.Join(departamentos,
                                 crg => crg.DepartamentoCodigo,
                                 dpt => dpt.Codigo,
                                 (crg, dpt) => new CargoDTO
                                 {
                                     Codigo = crg.Codigo,
                                     Descricao = crg.Descricao,
                                     DepartamentoCodigo = crg.DepartamentoCodigo,
                                     DepartamentoDescricao = dpt.Descricao
                                 }).OrderBy(orderBy => orderBy.Codigo).ToList();


            ConfigureViewDataFilter();
            //PaginacaoDeCargos(itemPage, cargos);

            ViewBag.CargoComDepartamentos = _cargoPager.GetEntitiesFilled();
            ViewBag.CargoPageInfo = _cargoPager.PageInfo;
            return View();
        }
    }
}

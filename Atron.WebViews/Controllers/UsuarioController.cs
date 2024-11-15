using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
using Communication.Extensions;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class UsuarioController : DefaultController<UsuarioDTO, Usuario, IUsuarioExternalService>
    {
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
            CurrentController = nameof(Usuario);
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
        public async Task<IActionResult> Cadastrar(int itemPage = 1)
        {
            ConfigureDataTitleForView("Cadastro de usuários");            

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
            PaginacaoDeCargos(itemPage, cargos);

            ViewBag.CargoComDepartamentos = _cargoPager.GetEntitiesFilled();
            ViewBag.CargoPageInfo = _cargoPager.PageInfo;

            return View();
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
            PaginacaoDeCargos(itemPage, cargos);

            ViewBag.CargoComDepartamentos = _cargoPager.GetEntitiesFilled();
            ViewBag.CargoPageInfo = _cargoPager.PageInfo;
        }
    }
}

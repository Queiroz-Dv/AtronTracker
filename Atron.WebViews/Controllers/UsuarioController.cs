// using Atron.Application.DTO;
// using Atron.Domain.Entities;
// using Atron.WebViews.Models;
// using Communication.Extensions;
// using ExternalServices.Interfaces;
// using ExternalServices.Interfaces.ApiRoutesInterfaces;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Options;
// using Shared.DTO.API;
// using Shared.Interfaces;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// namespace Atron.WebViews.Controllers
// {
//     public class UsuarioController : Controller
//     {
//         IUsuarioExternalService _externalService;
//         IDepartamentoExternalService _departamentoExternalService;
//         ICargoExternalService _cargoExternalService;
//         IPaginationService<CargoDTO> _cargoPager;

//         public UsuarioController(
//             IPaginationService<UsuarioDTO> paginationService,
//             IPaginationService<CargoDTO> cargoPager,
//             IResultResponseService responseModel,
//             IUsuarioExternalService externalService,
//             IDepartamentoExternalService departamentoExternalService,
//             IApiRouteExternalService apiRouteExternalService,
//             IConfiguration configuration,
//             IOptions<RotaDeAcesso> appSettingsConfig,
//             ICargoExternalService cargoExternalService) 
//             : base(paginationService,
//                   responseModel,
//                   externalService,
//                   apiRouteExternalService,
//                   configuration,
//                   appSettingsConfig)
//         {
//             _departamentoExternalService = departamentoExternalService;
//             _cargoExternalService = cargoExternalService;
//             _externalService = externalService;
//             _cargoPager = cargoPager;
//             CurrentController = nameof(Usuario);
//         }

//         [HttpGet, HttpPost]
//         public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
//         {
//             ConfigureDataTitleForView("Painel de usuários");
//             var usuarios = await _externalService.ObterTodos();

//             if (!usuarios.Any())
//             {
//                 return View();
//             }

//             Filter = filter;
//             ConfigurePaginationForView(usuarios, itemPage);
//             var model = new UsuarioModel()
//             {
//                 Usuarios = GetEntitiesPaginated(),
//                 PageInfo = PageInfo
//             };

//             return View(model);
//         }


//         [HttpGet]
//         public async Task<IActionResult> Cadastrar(int itemPage = 1)
//         {
//             ConfigureDataTitleForView("Cadastro de usuários");
//             ViewBag.CurrentController = CurrentController;

//             var departamentos = await _departamentoExternalService.ObterTodos();
//             var cargos = await _cargoExternalService.ObterTodos();

//             if (!departamentos.Any())
//             {
//                 _responseService.AddError("Para criar um usuário é necessário ter um departamento.");
//                 CreateTempDataNotifications();
//                 return RedirectToAction(nameof(Index));
//             }

//             if (!cargos.Any())
//             {
//                 _responseService.AddError("Para criar um usuário é necessário ter um cargo.");
//                 CreateTempDataNotifications();
//                 return RedirectToAction(nameof(Index));
//             }

//             cargos = cargos.Join(departamentos,
//                                  crg => crg.DepartamentoCodigo,
//                                  dpt => dpt.Codigo,
//                                  (crg, dpt) => new CargoDTO
//                                  {
//                                      Codigo = crg.Codigo,
//                                      Descricao = crg.Descricao,
//                                      DepartamentoCodigo = crg.DepartamentoCodigo,
//                                      DepartamentoDescricao = dpt.Descricao
//                                  }).OrderBy(orderBy => orderBy.Codigo).ToList();


//             ConfigureViewDataFilter();
//             PaginacaoDeCargos(itemPage, cargos);

//             ViewBag.CargoComDepartamentos = _cargoPager.GetEntitiesFilled();
//             ViewBag.CargoPageInfo = _cargoPager.PageInfo;

//             return View();
//         }

//         private void PaginacaoDeCargos(int itemPage, List<CargoDTO> cargos)
//         {
//             _cargoPager.Paginate(cargos, itemPage, CurrentController, string.Empty, nameof(Cadastrar));
//             _cargoPager.ConfigureEntityPaginated(cargos, string.Empty);
//         }

//         [HttpPost]
//         public async Task<IActionResult> Cadastrar(UsuarioDTO model)
//         {
//             if (ModelState.IsValid)
//             {
//                 await _externalService.Criar(model);
//                 var responses = _externalService.ResultResponses;
//                 CreateTempDataNotifications(responses);
//                 return !responses.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
//             }

//             _responseService.AddError("Registro inválido para gravação. Tente novamente.");
//             CreateTempDataNotifications();

//             return RedirectToAction(nameof(Cadastrar));
//         }       
//     }
// }

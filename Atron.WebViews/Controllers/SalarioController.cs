//using Atron.Application.DTO;
//using Atron.Domain.Entities;
//using Atron.WebViews.Models;
//using ExternalServices.Interfaces;
//using ExternalServices.Interfaces.ApiRoutesInterfaces;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Options;
//using Shared.DTO.API;
//using Shared.Extensions;
//using Shared.Interfaces;
//using Shared.Models;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Atron.WebViews.Controllers
//{
//    public class SalarioController : MainController<SalarioDTO, Salario, ISalarioExternalService>
//    {
//        private readonly IUsuarioExternalService _usuarioService;


//        public SalarioController(IUrlTransfer urlFactory,
//                                 IPaginationService<SalarioDTO> paginationService,
//                                 ISalarioExternalService service,
//                                 IApiRouteExternalService apiRouteExternalService,
//                                 IConfiguration configuration,
//                                 IOptions<RotaDeAcesso> appSettingsConfig,
//                                 MessageModel<Salario> messageModel,
//                                 IUsuarioExternalService usuarioService) :
//            base(
//                urlFactory,
//                paginationService,
//                service,
//                apiRouteExternalService,
//                configuration,
//                appSettingsConfig,
//                messageModel)
//        {
//            ApiController = nameof(Salario);
//            _usuarioService = usuarioService;
//        }


//        [HttpGet, HttpPost]
//        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
//        {
//            ConfigureDataTitleForView("Painel de salários");

//            BuildRoute(nameof(Salario));
//            var salarios = await _service.ObterTodos();

//            Filter = filter;
//            KeyToSearch = nameof(TarefaDTO.Titulo);
//            ConfigurePaginationForView(salarios, itemPage, ApiController, filter);

//            var model = new SalarioModel()
//            {
//                Salarios = GetEntitiesPaginated(),
//                PageInfo = PageInfo
//            };

//            return View(model);
//        }
//        private void BuildUsuarioRoute()
//        {
//            BuildRoute(nameof(Usuario));
//        }


//        [HttpGet]
//        public async Task<IActionResult> Cadastrar()
//        {
//            ConfigureDataTitleForView("Cadastro de salários");
//            ConfigureCurrentPageAction(nameof(Cadastrar));

//            //TODO: Criar um método de view bag de usuários
//            BuildUsuarioRoute();
//            var usuarios = await _usuarioService.ObterTodos();
//            var usuariosFiltrados = usuarios.Select(usr => new
//            {
//                usr.Codigo,
//                Nome = usr.NomeCompleto(),
//            }).ToList();

//            ViewBag.Usuarios = new SelectList(usuariosFiltrados, nameof(Usuario.Codigo), nameof(Usuario.Nome));

//            BuildRoute(nameof(Salario), "ObterMeses");
//            var meses = await _service.ObterMeses();

//            ViewBag.Meses = new SelectList(meses.ToList(), nameof(MesDTO.Id), nameof(MesDTO.Descricao));
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Cadastrar(SalarioDTO salarioDTO)
//        {
//            ConfigureCurrentPageAction(nameof(Cadastrar));

//            if (ModelState.IsValid)
//            {
//                BuildRoute(nameof(Salario));

//                await _service.Criar(salarioDTO);
//                CreateTempDataMessages();
//                return !_messageModel.Messages.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
//            }

//            _messageModel.AddError("Registro inválido para gravação. Tente novamente.");
//            CreateTempDataMessages();

//            return RedirectToAction(nameof(Cadastrar));
//        }


//        [HttpGet]
//        public async Task<IActionResult> Atualizar(string id)
//        {
//            ConfigureDataTitleForView("Atualização de salários");
//            ConfigureCurrentPageAction(nameof(Atualizar));

//            BuildRoute(nameof(Salario), "ObterMeses");
//            var meses = await _service.ObterMeses();

//            ViewBag.Meses = new SelectList(meses.ToList(), nameof(MesDTO.Id), nameof(MesDTO.Descricao));

//            BuildRoute(nameof(Salario), id);
//            var salario = await _service.ObterPorId();
//            return View(salario);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Atualizar(string id, SalarioDTO salarioDTO)
//        {
//            BuildRoute(nameof(Salario), id);
//            await _service.Atualizar(id, salarioDTO);

//            CreateTempDataMessages();

//            if (!_messageModel.Messages.HasErrors())
//            {
//                return RedirectToAction(nameof(Index));
//            }
//            else
//            {
//                ConfigureCurrentPageAction(nameof(Atualizar));
//                return View(nameof(Atualizar), salarioDTO);
//            }
//        }


//        [HttpGet]
//        public async Task<IActionResult> CarregarFormularioSalario(string codigoUsuario, string actionPage)
//        {
//            BuildRoute(nameof(Usuario), codigoUsuario);
//            var usuario = await _usuarioService.ObterPorCodigo(codigoUsuario);
//            var salarioDTO = new SalarioDTO

//            {
//                UsuarioCodigo = usuario.Codigo,
//                Usuario = new UsuarioDTO()
//                {
//                    Codigo = usuario.Codigo,
//                    Cargo = new CargoDTO()
//                    {
//                        Descricao = usuario.Cargo.Descricao
//                    },
//                    Departamento = new DepartamentoDTO()
//                    {
//                        Descricao = usuario.Departamento.Descricao
//                    }
//                },
//            };

//            BuildRoute(nameof(Salario), "ObterMeses");
//            var meses = await _service.ObterMeses();

//            ViewBag.Meses = new SelectList(meses.ToList(), nameof(MesDTO.Id), nameof(MesDTO.Descricao));

//            ConfigureCurrentPageAction(actionPage);

//            return PartialView("Partials/Salario/FormularioSalarioPartial", salarioDTO);
//        }
//    }
//}
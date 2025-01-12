using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
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
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class TarefaController : MainController<TarefaDTO, Tarefa, ITarefaExternalService>
    {
        private readonly IUsuarioExternalService _usuarioService;
        private readonly IDepartamentoExternalService _departamentoService;
        private readonly ICargoExternalService _cargoService;
        private readonly ITarefaEstadoExternalService _tarefaEstadoExternalService;

        public TarefaController(IUrlModuleFactory urlFactory,
                                IPaginationService<TarefaDTO> paginationService,
                                ITarefaExternalService service,
                                IApiRouteExternalService apiRouteExternalService,
                                IConfiguration configuration,
                                IOptions<RotaDeAcesso> appSettingsConfig,
                                MessageModel<Tarefa> messageModel,
                                IUsuarioExternalService usuarioService,
                                ITarefaEstadoExternalService tarefaEstadoExternalService) :
            base(urlFactory,
                 paginationService,
                 service,
                 apiRouteExternalService,
                 configuration,
                 appSettingsConfig,
                 messageModel)
        {
            _tarefaEstadoExternalService = tarefaEstadoExternalService;
            _usuarioService = usuarioService;
            CurrentController = nameof(Tarefa);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de tarefas");

            BuildRoute(nameof(Tarefa));
            var tarefas = await _service.ObterTodos();

            Filter = filter;
            KeyToSearch = nameof(TarefaDTO.Titulo);
            ConfigurePaginationForView(tarefas, itemPage, CurrentController, filter);

            var model = new TarefaModel()
            {
                Tarefas = GetEntitiesPaginated(),
                PageInfo = PageInfo
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de tarefas");
            ConfigureCurrentPageAction(nameof(Cadastrar));

            BuildUsuarioRoute();
            var usuarios = await _usuarioService.ObterTodos();
            var usuariosFiltrados = usuarios.Select(usr => new
            {
                usr.Codigo,
                Nome = usr.NomeCompleto(),
            }).ToList();

            ViewBag.Usuarios = new SelectList(usuariosFiltrados, nameof(Usuario.Codigo), nameof(Usuario.Nome));
            await ConfigurarViewBagDeEstadoDasTarefas();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(TarefaDTO tarefaDTO)
        {
            ConfigureCurrentPageAction(nameof(Cadastrar));

            if (ModelState.IsValid)
            {
                BuildRoute(nameof(Tarefa));

                await _service.Criar(tarefaDTO);
                CreateTempDataMessages();
                return !_messageModel.Messages.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
            }

            _messageModel.AddError("Registro inválido para gravação. Tente novamente.");
            CreateTempDataMessages();

            return RedirectToAction(nameof(Cadastrar));
        }

        private void  BuildUsuarioRoute()
        {
            BuildRoute(nameof(Usuario));
        }

        [HttpGet]
        public async Task<IActionResult> CarregarFormularioTarefa(string codigoUsuario, string actionPage)
        {
            BuildRoute(nameof(Usuario), codigoUsuario);
            var usuario = await _usuarioService.ObterPorCodigo(codigoUsuario);
            var tarefaDto = new TarefaDTO
            {
                UsuarioCodigo = usuario.Codigo,
                Usuario = new UsuarioDTO()
                {
                    Codigo = usuario.Codigo,
                    Cargo = new CargoDTO()
                    {
                        Descricao = usuario.Cargo.Descricao
                    },
                    Departamento = new DepartamentoDTO()
                    {
                        Descricao = usuario.Departamento.Descricao
                    }
                },
            };

            await ConfigurarViewBagDeEstadoDasTarefas();
            ConfigureCurrentPageAction(actionPage);

            return PartialView("Partials/Tarefa/FormularioTarefa", tarefaDto);
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string id)
        {
            ConfigureDataTitleForView("Atualizar informação da tarefa");
            ConfigureCurrentPageAction(nameof(Atualizar));

            BuildRoute(nameof(Tarefa), id);
            var tarefa = await _service.ObterPorId();

            BuildUsuarioRoute();
            var usuarios = await _usuarioService.ObterTodos();
            var usuariosFiltrados = usuarios.Select(usr => new
            {
                usr.Codigo,
                Nome = usr.NomeCompleto(),
            }).ToList();

            ViewBag.Usuarios = new SelectList(usuariosFiltrados, nameof(Usuario.Codigo), nameof(Usuario.Nome));

            await ConfigurarViewBagDeEstadoDasTarefas();
            return View(tarefa);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string id, TarefaDTO tarefaDTO)
        {
            BuildRoute(nameof(Tarefa), id);

            await _service.Atualizar(id, tarefaDTO);

            CreateTempDataMessages();
            if (!_messageModel.Messages.HasErrors())
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ConfigureCurrentPageAction(nameof(Atualizar));
                return View(nameof(Atualizar), tarefaDTO);
            }
        }
        private async Task ConfigurarViewBagDeEstadoDasTarefas()
        {
            BuildRoute(nameof(TarefaEstado));

            var estados = await _tarefaEstadoExternalService.ObterTodosAsync();

            ViewBag.TarefaEstados = new SelectList(estados, nameof(TarefaEstado.Id), nameof(TarefaEstado.Descricao));
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            BuildRoute(nameof(Tarefa), codigo);
            await _service.Remover(codigo);

            CreateTempDataMessages();
            return RedirectToAction(nameof(Index));
        }
    }
}
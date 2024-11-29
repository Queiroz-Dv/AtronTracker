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
using Shared.Interfaces;
using Shared.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class TarefaController : DefaultController<TarefaDTO, Tarefa, ITarefaExternalService>
    {
        private readonly IUsuarioExternalService _usuarioService;
        private readonly IDepartamentoExternalService _departamentoService;
        private readonly ICargoExternalService _cargoService;

        public TarefaController(IUrlModuleFactory urlFactory,
                                IPaginationService<TarefaDTO> paginationService,
                                ITarefaExternalService service,
                                IApiRouteExternalService apiRouteExternalService,
                                IConfiguration configuration,
                                IOptions<RotaDeAcesso> appSettingsConfig,
                                MessageModel<Tarefa> messageModel,
                                IUsuarioExternalService usuarioService) :
            base(urlFactory,
                 paginationService,
                 service,
                 apiRouteExternalService,
                 configuration,
                 appSettingsConfig,
                 messageModel)
        {
            _usuarioService = usuarioService;
            CurrentController = nameof(Tarefa);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de tarefas");

            await BuildRoute(nameof(Tarefa));
            var tarefas = await _service.ObterTodos();

            Filter = filter;
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

            await BuildUsuarioRoute();
            var usuarios = await _usuarioService.ObterTodos();
            var usuariosFiltrados = usuarios.Select(usr => new
            {
                Codigo = usr.Codigo,
                Nome = $"{usr.Nome} {usr.Sobrenome}",
                //CargoDescricao = usr.Cargo.Descricao,
                //DepartamentoDescricao = usr.Departamento.Descricao
            }).ToList();

            ViewBag.Usuarios = new SelectList(usuariosFiltrados, nameof(Usuario.Codigo), nameof(Usuario.Nome));

            await BuildRoute(nameof(TarefaEstado));
            var 

            return View();
        }

        private async Task BuildUsuarioRoute()
        {
            await BuildRoute(nameof(Usuario));
        }

        [HttpGet]
        public async Task<IActionResult> CarregarFormularioTarefa(string codigoUsuario)
        {
            await BuildRoute(nameof(Usuario), codigoUsuario);
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
                }
            };

            return PartialView("Partials/Tarefa/FormularioTarefa", tarefaDto);
        }
    }
}
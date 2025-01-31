using Atron.Application.DTO;
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
    public class TarefaController : MainController<TarefaDTO, Tarefa>
    {
        private readonly IExternalService<TarefaDTO> _service;
        private readonly IExternalService<UsuarioDTO> _usuarioService;
        private readonly IExternalService<CargoDTO> _cargoService;
        private readonly IExternalService<DepartamentoDTO> _departamentoService;
        private readonly IExternalService<TarefaEstado> _tarefaEstadoService;

        public TarefaController(
                        IExternalService<TarefaDTO> service,
                        IPaginationService<TarefaDTO> paginationService,
                        IExternalService<UsuarioDTO> usuarioService,
                        IExternalService<CargoDTO> cargoService,
                        IExternalService<DepartamentoDTO> departamentoService,
                        IExternalService<TarefaEstado> tarefaEstadoService,
                        IRouterBuilderService router,
                        MessageModel messageModel)
            : base(messageModel, paginationService)
        {
            _service = service;
            _usuarioService = usuarioService;
            _cargoService = cargoService;
            _departamentoService = departamentoService;
            _tarefaEstadoService = tarefaEstadoService;
            _router = router;
            ApiControllerName = nameof(Tarefa);
        }

        public async override Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de tarefas");
            BuildRoute();
            var tarefas = await _service.ObterTodos();

            ConfigurePaginationForView(tarefas, new PageInfoDTO()
            {
                CurrentPage = itemPage,
                PageRequestInfo = new PageRequestInfoDTO()
                {
                    CurrentViewController = ApiControllerName,
                    Action = nameof(Index),
                    Filter = filter,
                }
            });

            var model = new TarefaModel()
            {
                Tarefas = _paginationService.GetEntitiesFilled(),
                PageInfo = _paginationService.GetPageInfo()
            };

            return View(model);
        }

        private void BuildUsuarioRoute()
        {
            ApiControllerName = nameof(Usuario);
            BuildRoute();
        }

        private void BuildTarefaEstadoRoute()
        {
            ApiControllerName = nameof(TarefaEstado);
            BuildRoute();
        }

        private async Task FetchUsuarioData()
        {
            BuildUsuarioRoute();
            var usuarios = await _usuarioService.ObterTodos();
            var usuariosFiltrados = usuarios.Where(usr => usr.Departamento is not null).Select(usr => new
            {
                usr.Codigo,
                Nome = usr.NomeCompleto(),
            }).ToList();

            ViewBag.Usuarios = new SelectList(usuariosFiltrados, nameof(Usuario.Codigo), nameof(Usuario.Nome));
        }

        private async Task FetchTarefaEstadoData()
        {
            BuildTarefaEstadoRoute();
            var estados = await _tarefaEstadoService.ObterTodos();

            ViewBag.TarefaEstados = new SelectList(estados, nameof(TarefaEstado.Id), nameof(TarefaEstado.Descricao));
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de tarefas");
            ConfigureCurrentPageAction(nameof(Cadastrar));

            await FetchUsuarioData();
            await FetchTarefaEstadoData();
            return View(new TarefaDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(TarefaDTO tarefaDTO)
        {
            ConfigureCurrentPageAction(nameof(Cadastrar));

            if (ModelState.IsValid)
            {
                BuildRoute();

                await _service.Criar(tarefaDTO);
                CreateTempDataMessages();
                return !_messageModel.Messages.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
            }
            else
            {
                _messageModel.AddError("Registro inválido para gravação. Tente novamente.");
                CreateTempDataMessages();

                return RedirectToAction(nameof(Cadastrar));
            }
        }


        [HttpGet]
        public async Task<IActionResult> CarregarFormularioTarefa(string tarefaId, string codigoUsuario, string actionPage)
        {
            BuildUsuarioRoute();
            var usuario = await _usuarioService.ObterPorCodigo(codigoUsuario);

            var tarefaDTO = new TarefaDTO();

            if (!tarefaId.Equals(0) && actionPage.Equals(nameof(Atualizar)))
            {
                ApiControllerName = nameof(Tarefa);
                BuildRoute();
                // Se for update 
                tarefaDTO = await _service.ObterPorId(tarefaId);

                tarefaDTO.UsuarioCodigo = usuario.Codigo;

                tarefaDTO.Usuario = new UsuarioDTO()
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
                };
            }
            else
            {
                // Se for cadastro
                tarefaDTO = new TarefaDTO
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
            }

            await FetchTarefaEstadoData();
            ConfigureCurrentPageAction(actionPage);
            return PartialView("Partials/Tarefa/FormularioTarefa", tarefaDTO);
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string id)
        {
            ConfigureDataTitleForView("Atualizar informação da tarefa");
            ConfigureCurrentPageAction(nameof(Atualizar));

            BuildRoute();
            var tarefa = await _service.ObterPorId(id);

            await FetchUsuarioData();
            await FetchTarefaEstadoData();
            return View(tarefa);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string id, TarefaDTO tarefaDTO)
        {
            BuildRoute();

            await _service.AtualizarPorId(id, tarefaDTO);

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

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            BuildRoute();
            await _service.Remover(codigo);

            CreateTempDataMessages();
            return RedirectToAction(nameof(Index));
        }
    }
}
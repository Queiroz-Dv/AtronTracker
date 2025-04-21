using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.Collections.Generic;
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
        private static readonly List<TarefaEstadoDTO> TarefaEstados = new()
        {
            new() { Id = 1, Descricao = "Em atividade" },
            new() { Id = 2, Descricao = "Pendente de aprovação" },
            new() { Id = 3, Descricao = "Entregue" },
            new() { Id = 4, Descricao = "Finalizada" },
            new() { Id = 5, Descricao = "Iniciada" }
        };

        public TarefaController(
                        IExternalService<TarefaDTO> service,
                        IPaginationService<TarefaDTO> paginationService,
                        IExternalService<UsuarioDTO> usuarioService,
                        IExternalService<CargoDTO> cargoService,
                        IExternalService<DepartamentoDTO> departamentoService,
                        IRouterBuilderService router,
                        MessageModel messageModel)
            : base(messageModel, paginationService)
        {
            _service = service;
            _usuarioService = usuarioService;
            _cargoService = cargoService;
            _departamentoService = departamentoService;
            _router = router;
            ApiControllerName = nameof(Tarefa);
        }

        [HttpGet]
        public async Task<IActionResult> MenuPrincipal(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de tarefas");
            BuildRoute();
            var tarefas = await _service.ObterTodos();

            BuildUsuarioRoute();
            var usuarios = await _usuarioService.ObterTodos();

            tarefas = tarefas.Join(usuarios, tarefa =>
                tarefa.UsuarioCodigo,
                usuario => usuario.Codigo,
                (tarefa, usuario) =>
                {
                    //tarefa.NomeUsuario = usuario.Nome;
                    //tarefa.CargoDescricao = usuario.Cargo.Descricao;
                    //tarefa.DepartamentoDescricao = usuario.Departamento.Descricao;
                    return tarefa;
                }
            //).Join(TarefaEstados, tarefa =>
            //  //  tarefa.EstadoDaTarefaId,
            //    tarefaEstado => tarefaEstado.Id.ToString(),
            //    (tarefa, tarefaEstado) =>
            //    {
            //      //  tarefa.EstadoDaTarefaDescricao = tarefaEstado.Descricao;
            //        return tarefa;
            //    }
            ).ToList();

            ApiControllerName = nameof(Tarefa);
            ConfigurePaginationForView(tarefas, nameof(MenuPrincipal), itemPage, filter);

            return View(GetModel<TarefaModel>());
        }

        private void BuildUsuarioRoute()
        {
            ApiControllerName = nameof(Usuario);
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


        private void PreencherViewBagTarefaEstados()
        {
            ViewBag.TarefaEstados = new SelectList(TarefaEstados, nameof(TarefaEstadoDTO.Id), nameof(TarefaEstadoDTO.Descricao));
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de tarefas");
            ConfigureCurrentPageAction(nameof(Cadastrar));
            await CarregarCamposComplementares();
            return View();
        }

        private async Task CarregarCamposComplementares()
        {
            await FetchUsuarioData();
            PreencherViewBagTarefaEstados();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(TarefaDTO tarefaDTO)
        {
            ConfigureCurrentPageAction(nameof(Cadastrar));
            ConfigureDataTitleForView("Cadastro de tarefas");

            if (ModelState.IsValid)
            {                
                BuildRoute();

                await _service.Criar(tarefaDTO);
                CreateTempDataMessages();
                return !_messageModel.Messages.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
            }

            await CarregarCamposComplementares();
            ApiControllerName = nameof(Tarefa);

            return View();
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

                //tarefaDTO.UsuarioCodigo = usuario.Codigo;
                //tarefaDTO.CargoDescricao = usuario.Cargo.Descricao;
                //tarefaDTO.DepartamentoDescricao = usuario.Departamento.Descricao;
            }
            else
            {
                // Se for cadastro
                tarefaDTO = new TarefaDTO
                {
                    //UsuarioCodigo = usuario.Codigo,
                    //CargoDescricao = usuario.Cargo.Descricao,
                    //DepartamentoDescricao = usuario.Departamento.Descricao,
                };
            }

            PreencherViewBagTarefaEstados();
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

            BuildUsuarioRoute();
            var usuario = await _usuarioService.ObterPorCodigo(tarefa.UsuarioCodigo);

            //tarefa.CargoDescricao = usuario.Cargo.Descricao;
            //tarefa.DepartamentoDescricao = usuario.Departamento.Descricao;

            await CarregarCamposComplementares();
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
                return RedirectToAction(nameof(MenuPrincipal));
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
            return RedirectToAction(nameof(MenuPrincipal));
        }
    }
}
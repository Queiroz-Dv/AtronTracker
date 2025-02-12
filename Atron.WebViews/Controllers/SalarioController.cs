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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    [Authorize]
    public class SalarioController : MainController<SalarioDTO, Salario>
    {
        private readonly IExternalService<SalarioDTO> _service;
        private readonly IExternalService<UsuarioDTO> _usuarioService;

        public SalarioController(IExternalService<SalarioDTO> service,
                                 IPaginationService<SalarioDTO> paginationService,
                                 IExternalService<UsuarioDTO> usuarioService,
                                 IRouterBuilderService router,
                                 MessageModel messageModel) :
            base(messageModel, paginationService)
        {
            _service = service;
            _usuarioService = usuarioService;
            _router = router;
            ApiControllerName = nameof(Salario);
        }

        public async override Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de salários");

            BuildRoute();
            var salarios = await _service.ObterTodos();

            ConfigurePaginationForView(salarios, nameof(Index), itemPage, filter);
            return View(GetModel<SalarioModel>());
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

        private void FetchMesesData()
        {
            var meses = new List<MesDTO>
            {
                new() { Id = 1, Descricao = "Janeiro" },
                new() { Id = 2, Descricao = "Fevereiro" },
                new() { Id = 3, Descricao = "Março" },
                new() { Id = 4, Descricao = "Abril" },
                new() { Id = 5, Descricao = "Maio" },
                new() { Id = 6, Descricao = "Junho" },
                new() { Id = 7, Descricao = "Julho" },
                new() { Id = 8, Descricao = "Agosto" },
                new() { Id = 9, Descricao = "Setembro" },
                new() { Id = 10, Descricao = "Outubro" },
                new() { Id = 11, Descricao = "Novembro" },
                new() { Id = 12, Descricao = "Dezembro" }
            };

            ViewBag.Meses = new SelectList(meses, nameof(MesDTO.Id), nameof(MesDTO.Descricao));
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de salários");
            ConfigureCurrentPageAction(nameof(Cadastrar));

            FetchMesesData();
            await FetchUsuarioData();

            return View(new SalarioDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(SalarioDTO salarioDTO)
        {
            ConfigureCurrentPageAction(nameof(Cadastrar));

            if (ModelState.IsValid)
            {
                BuildRoute();

                await _service.Criar(salarioDTO);
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
        public async Task<IActionResult> Atualizar(string id)
        {
            ConfigureDataTitleForView("Atualização de salários");
            ConfigureCurrentPageAction(nameof(Atualizar));

            BuildRoute();
            var salario = await _service.ObterPorId(id);

            FetchMesesData();
            await FetchUsuarioData();
            return View(salario);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string id, SalarioDTO salarioDTO)
        {
            BuildRoute();
            await _service.Atualizar(id, salarioDTO);

            CreateTempDataMessages();

            if (!_messageModel.Messages.HasErrors())
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ConfigureCurrentPageAction(nameof(Atualizar));
                return View(nameof(Atualizar), salarioDTO);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CarregarFormularioSalario(string salarioId, string codigoUsuario, string actionPage)
        {
            BuildUsuarioRoute();
            var usuario = await _usuarioService.ObterPorCodigo(codigoUsuario);

            var salarioDTO = new SalarioDTO();

            if (!salarioId.Equals(0) && actionPage.Equals(nameof(Atualizar)))
            {
                ApiControllerName = nameof(Salario);
                BuildRoute();

                salarioDTO = await _service.ObterPorId(salarioId);

                salarioDTO.UsuarioCodigo = usuario.Codigo;

                salarioDTO.Usuario = new UsuarioDTO()
                {
                    Codigo = usuario.Codigo,
                    Cargo = new CargoDTO()
                    {
                        Descricao = usuario.Cargo.Descricao
                    },
                    Departamento = new DepartamentoDTO(usuario.Departamento.Codigo, usuario.Departamento.Descricao)                   
                };
            }
            else
            {
                salarioDTO = new SalarioDTO

                {
                    UsuarioCodigo = usuario.Codigo,
                    Usuario = new UsuarioDTO()
                    {
                        Codigo = usuario.Codigo,
                        Cargo = new CargoDTO()
                        {
                            Descricao = usuario.Cargo.Descricao
                        },
                        Departamento = new DepartamentoDTO(usuario.Departamento.Codigo,usuario.Departamento.Descricao)                   
                    },
                };
            }

            FetchMesesData();

            ConfigureCurrentPageAction(actionPage);

            return PartialView("Partials/Salario/FormularioSalarioPartial", salarioDTO);
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
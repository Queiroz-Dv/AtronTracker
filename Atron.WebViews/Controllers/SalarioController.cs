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

            ConfigurePaginationForView(salarios, new PageInfoDTO()
            {
                CurrentPage = itemPage,
                PageRequestInfo = new PageRequestInfoDTO()
                {
                    CurrentViewController = ApiControllerName,
                    Action = nameof(Index),
                    Filter = filter,
                }
            });

            var model = new SalarioModel()
            {
                Salarios = _paginationService.GetEntitiesFilled(),
                PageInfo = _paginationService.GetPageInfo()
            };

            return View(model);
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

        private async Task FetchMesesData()
        {
            ApiControllerName = nameof(Salario);
            BuildRoute("ObterMeses");
            var meses = await _service.ObterTodos<MesDTO>();

            ViewBag.Meses = new SelectList(meses.ToList(), nameof(MesDTO.Id), nameof(MesDTO.Descricao));
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de salários");
            ConfigureCurrentPageAction(nameof(Cadastrar));

            await FetchMesesData();
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

            await FetchMesesData();
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
                    Departamento = new DepartamentoDTO()
                    {
                        Descricao = usuario.Departamento.Descricao
                    }
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
                        Departamento = new DepartamentoDTO()
                        {
                            Descricao = usuario.Departamento.Descricao
                        }
                    },
                };
            }

            await FetchMesesData();

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
using Atron.Application.DTO;
using Atron.Tracker.Domain.Entities;
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
    public class SalarioController : MainController<SalarioDTO, Salario>
    {
        private readonly IExternalService<SalarioDTO> _service;
        private readonly IExternalService<UsuarioDTO> _usuarioService;
        private static readonly List<MesDTO> Meses = new()
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

        [HttpGet, HttpPost]
        public async Task<IActionResult> MenuPrincipal(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de salários");

            BuildRoute();
            var salarios = await _service.ObterTodos();

            BuildUsuarioRoute();
            var usuarios = await _usuarioService.ObterTodos();

            salarios = salarios
            .Join(usuarios, salario =>
                            salario.UsuarioCodigo,
                            usuario => usuario.Codigo,
                            (salario, usuario) =>
                            {
                                //salario.NomeUsuario = usuario.Nome;
                                //salario.CargoDescricao = usuario.Cargo.Descricao;
                                //salario.DepartamentoDescricao = usuario.Departamento.Descricao;
                                return salario;
                            })
            .Join(Meses, salario =>
                         salario.MesId,
                         mes => mes.Id,
                         (salario, mes) =>
                         {
                           //  salario.MesDescricao = mes.Descricao;
                             return salario;
                         }
            ).ToList();

            ApiControllerName = nameof(Salario);
            ConfigurePaginationForView(salarios, nameof(MenuPrincipal), itemPage, filter);
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
                Nome = usr.Nome + " " + usr.Sobrenome,
            }).ToList();

            ViewBag.Usuarios = new SelectList(usuariosFiltrados, nameof(Usuario.Codigo), nameof(Usuario.Nome));
        }

        private void PreencherViewBagMeses()
        {
            ViewBag.Meses = new SelectList(Meses, nameof(MesDTO.Id), nameof(MesDTO.Descricao));
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de salários");
            ConfigureCurrentPageAction(nameof(Cadastrar));

            PreencherViewBagMeses();
            await FetchUsuarioData();

            ApiControllerName = nameof(Salario);
            return View();
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
                return !_messageModel.Notificacoes.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
            }

            ApiControllerName = nameof(Salario);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string id)
        {
            ConfigureDataTitleForView("Atualização de salários");
            ConfigureCurrentPageAction(nameof(Atualizar));

            BuildRoute();
            var salario = await _service.ObterPorId(id);

            BuildUsuarioRoute();
            var usuario = await _usuarioService.ObterPorCodigo(salario.UsuarioCodigo);

            //salario.CargoDescricao = usuario.Cargo.Descricao;
            //salario.DepartamentoDescricao = usuario.Departamento.Descricao;
            //salario.NomeUsuario = usuario.Nome;
            PreencherViewBagMeses();
            await FetchUsuarioData();
            return View(salario);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string id, SalarioDTO salarioDTO)
        {
            BuildRoute();
            await _service.Atualizar(id, salarioDTO);

            CreateTempDataMessages();

            if (!_messageModel.Notificacoes.HasErrors())
            {
                return RedirectToAction(nameof(MenuPrincipal));
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

            ApiControllerName = nameof(Salario);
            var salarioDTO = new SalarioDTO();

            if (!salarioId.Equals(0) && actionPage.Equals(nameof(Atualizar)))
            {
                BuildRoute();

                salarioDTO = await _service.ObterPorId(salarioId);

                salarioDTO.UsuarioCodigo = usuario.Codigo;
                //salarioDTO.NomeUsuario = usuario.Nome;
                //salarioDTO.CargoDescricao = usuario.Cargo.Descricao;
                //salarioDTO.DepartamentoDescricao = usuario.Departamento.Descricao;
            }
            else
            {
                salarioDTO = new SalarioDTO
                {
                    UsuarioCodigo = usuario.Codigo,
                    //CargoDescricao = usuario.Cargo.Descricao,
                    //DepartamentoDescricao = usuario.Departamento.Descricao
                };
            }

            PreencherViewBagMeses();
            ConfigureCurrentPageAction(actionPage);

            return PartialView("Partials/Salario/FormularioSalarioPartial", salarioDTO);
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
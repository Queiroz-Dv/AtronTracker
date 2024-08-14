using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
using ExternalServices.Interfaces;
using Communication.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Atron.WebViews.Controllers
{
    public class UsuarioController : DefaultController<UsuarioDTO>
    {
        IUsuarioExternalService _externalService;
        IDepartamentoExternalService _departamentoExternalService;
        ICargoExternalService _cargoExternalService;

        public UsuarioController(
            IPaginationService<UsuarioDTO> paginationService,
            IResultResponseService responseModel,
            IUsuarioExternalService externalService,
            IDepartamentoExternalService departamentoExternalService,
            ICargoExternalService cargoExternalService) : base(paginationService, responseModel)
        {
            _departamentoExternalService = departamentoExternalService;
            _cargoExternalService = cargoExternalService;
            _externalService = externalService;
            CurrentController = nameof(Usuario);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ConfigureDataTitleForView("Painel de usuários");
            var usuarios = await _externalService.ObterTodos();

            if (!usuarios.Any())
            {
                return View();
            }

            Filter = filter;
            ConfigurePaginationForView(usuarios, itemPage);
            var model = new UsuarioModel()
            {
                Usuarios = GetEntitiesPaginated(),
                PageInfo = PageInfo
            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de usuários");
            ViewBag.CurrentController = CurrentController;
            
            var departamentos = await _departamentoExternalService.ObterTodos();
            var cargos = await _cargoExternalService.ObterTodos();

            if (!departamentos.Any())
            {
                _responseService.AddError("Para criar um usuário é necessário ter um departamento.");
                CreateTempDataNotifications();
                return RedirectToAction(nameof(Index));
            }

            if (!cargos.Any())
            {
                _responseService.AddError("Para criar um usuário é necessário ter um cargo.");
                CreateTempDataNotifications();
                return RedirectToAction(nameof(Index));
            }

            var departamentosFiltrados = departamentos.Select(dpt =>
                new
                {
                    dpt.Codigo,
                    Descricao = $"{dpt.Codigo} - {dpt.Descricao}"
                }).ToList();

            ViewBag.Departamentos = new SelectList(departamentosFiltrados, "Codigo", "Descricao");

            if (TempData["CargosFiltrados"] != null)
            {
                var cargosFiltrados = JsonConvert.DeserializeObject<List<dynamic>>(TempData["CargosFiltrados"].ToString());
                ViewBag.Cargos = new SelectList(cargosFiltrados, "Codigo", "Descricao");
            }
            else
            {
                var cargosFiltrados = cargos.Select(crg =>
                    new
                    {
                        crg.Codigo,
                        Descricao = $"{crg.Codigo} - {crg.Descricao}"
                    }
                ).ToList();

                ViewBag.Cargos = new SelectList(cargosFiltrados, "Codigo", "Descricao");
            }

            UsuarioDTO usuarioDTO = new UsuarioDTO();
            if (TempData["UsuarioDTO"] != null)
            {
                usuarioDTO = JsonConvert.DeserializeObject<UsuarioDTO>(TempData[nameof(UsuarioDTO)].ToString());
            }

            return View(usuarioDTO);
        }


        //[HttpPost]
        //public async Task<IActionResult> CadastrarUsuario(UsuarioDTO model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _externalService.Criar(model);
        //        var responses = _externalService.ResultResponses;
        //        CreateTempDataNotifications(responses);
        //        return !responses.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View();
        //    }

        //    _responseService.AddError("Registro inválido para gravação. Tente novamente.");
        //    CreateTempDataNotifications();

        //    return RedirectToAction(nameof(Cadastrar));
        //}

        [HttpGet]
        public async Task<IActionResult> ObterCargosAssociados(UsuarioDTO usuarioDTO)
        {
            var cargos = await _cargoExternalService.ObterTodos();

            cargos = cargos.Where(crg => crg.DepartamentoCodigo == usuarioDTO.DepartamentoCodigo).ToList();

            var cargosFiltrados = cargos.Select(crg => new
            {
                crg.Codigo,
                Descricao = $"{crg.Codigo} - {crg.Descricao}"
            }).ToList();

            TempData["CargosFiltrados"] = JsonConvert.SerializeObject(cargosFiltrados);
            TempData[nameof(UsuarioDTO)] = JsonConvert.SerializeObject(usuarioDTO);
            return RedirectToAction(nameof(Cadastrar));
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartamentosByCargo(string cargoCodigo)
        {
            var departamentos = await _departamentoExternalService.ObterTodos();

            departamentos = departamentos.Where(dpt => dpt.CargoCodigo == cargoCodigo).ToList();

            var departamentosFiltrados = departamentos.Select(dpt => new
            {
                dpt.Codigo,
                Descricao = $"{dpt.Codigo} - {dpt.Descricao}"
            }).ToList();

            return Json(departamentosFiltrados);
        }

    }
}

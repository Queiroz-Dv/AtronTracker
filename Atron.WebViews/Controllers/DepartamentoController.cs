using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
using Communication.Extensions;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class DepartamentoController : Controller
    {
        private IDepartamentoExternalService _externalService;
        private readonly PaginationService _paginationService;

        public List<ResultResponse> ResultResponses { get; set; }

        public DepartamentoController(IDepartamentoExternalService externalService, PaginationService paginationService)
        {
            _externalService = externalService;
            _paginationService = paginationService;
            ResultResponses = new List<ResultResponse>();
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ViewData["Title"] = "Painel de departamentos";
            ViewData["Filter"] = filter;

            var departamentos = await _externalService.ObterTodos();

            if (!departamentos.Any())
            {
                return View();
            }

            var pageInfo = _paginationService.Paginate(departamentos, itemPage, nameof(Departamento), filter);
            if (!string.IsNullOrEmpty(filter))
            {
                _paginationService.ForceFilter = true;
                _paginationService.FilterBy = filter;
            }

            var entities = _paginationService.GetEntityPaginated(departamentos, filter);
            var model = new DepartamentoModel()
            {
                Departamentos = entities,
                PageInfo = pageInfo
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            ViewData["Title"] = "Cadastro de departamentos";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(DepartamentoDTO departamento)
        {
            if (ModelState.IsValid)
            {
                await _externalService.Criar(departamento);

                var responses = _externalService.ResultResponses;
                var responseSerialized = JsonConvert.SerializeObject(responses);
                TempData["Notifications"] = responseSerialized;
                return !responses.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View(nameof(Cadastrar), departamento);
            }

            ViewData["Title"] = "Cadastro de departamentos";
            return View(departamento);
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
            if (codigo is null)
            {
                var notification = new ResultResponse() { Message = "O código informado não foi encontrado", Level = "Error" };
                ResultResponses.Add(notification);
                TempData["Notifications"] = JsonConvert.SerializeObject(ResultResponses);

                return View(codigo);
            }

            var departamentos = await _externalService.ObterTodos();

            var departamentoDTO = departamentos.FirstOrDefault(dpt => dpt.Codigo == codigo);

            if (departamentoDTO is not null)
            {
                ViewData["Title"] = "Atualizar informação de departamento";

                return View(departamentoDTO);
            }
            else
            {
                return View(departamentoDTO);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string codigo, DepartamentoDTO departamentoDTO)
        {
            var response = await _externalService.Atualizar(codigo, departamentoDTO);
            ResultResponses.AddRange(response.responses);

            var responseSerialized = JsonConvert.SerializeObject(ResultResponses);
            TempData["Notifications"] = responseSerialized;

            return response.isSucess ? RedirectToAction(nameof(Index)) : View(nameof(Atualizar), departamentoDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                TempData["Erro"] = JsonConvert.SerializeObject(new ResultResponse() { Message = "Código não informado, tente novamente." });
                return RedirectToAction(nameof(Index));
            }

            var response = await _externalService.Remover(codigo);

            if (response.isSuccess)
            {
                ResultResponses.AddRange(response.responses);
                TempData["Notifications"] = JsonConvert.SerializeObject(ResultResponses);
            }
            else
            {
                ViewBag.Erros = JsonConvert.SerializeObject(response.responses);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ObterDepartamento(string codigoDepartamento)
        {
            var departamentos = await _externalService.ObterTodos();

            var departamento = departamentos.FirstOrDefault(dpt => dpt.Codigo == codigoDepartamento);

            return Ok(departamento);
        }
    }
}

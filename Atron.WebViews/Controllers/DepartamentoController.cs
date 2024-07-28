using Atron.Application.DTO;
using Atron.WebViews.Models;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class DepartamentoController : Controller
    {
        private IDepartamentoExternalService _externalService;
        public int PageSize = 4;

        public List<ResultResponse> ResultResponses { get; set; }

        public DepartamentoController(IDepartamentoExternalService externalService)
        {
            _externalService = externalService;
            ResultResponses = new List<ResultResponse>();
        }

        [HttpGet]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            ViewData["Title"] = "Painel de departamentos";

            var departamentos = await _externalService.ObterTodos();

            if (!departamentos.Any())
            {
                return View();
            }

            if (!string.IsNullOrEmpty(filter))
            {
                departamentos = departamentos.Where(dpt => dpt.Codigo.Contains(filter)).ToList();
            }

            var models = new DepartamentosListViewModel()
            {
                Departamentos = departamentos.Skip((itemPage - 1) * PageSize)
                                             .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = itemPage,
                    ItemsPerPage = PageSize,
                    TotalItems = departamentos.Count(),
                    Filter = filter
                }
            };
            return View(models);
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
                var response = await _externalService.Criar(departamento);
                ResultResponses.AddRange(response.responses);

                if (response.isSucess)
                {
                    var responseSerialized = JsonConvert.SerializeObject(ResultResponses);
                    TempData["Notifications"] = responseSerialized;
                    return RedirectToAction(nameof(Cadastrar));
                }
                else
                {
                    ViewBag.Erros = ResultResponses;
                    return View(nameof(Cadastrar), departamento);
                }
            }

            ViewData["Title"] = "Cadastro de departamentos";
            return View(departamento);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string codigo, [FromBody] DepartamentoDTO departamentoDTO)
        {
            var response = await _externalService.Atualizar(codigo, departamentoDTO);

            if (response.isSucess)
            {
                ResultResponses.AddRange(response.responses);
                var responseSerialized = JsonConvert.SerializeObject(ResultResponses);                
            }
            else
            {
                ViewBag.Erros = response.responses;
                return RedirectToAction(nameof(Index));
            }

            return Ok();
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

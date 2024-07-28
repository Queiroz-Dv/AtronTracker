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
        [Route("/Departamento/Index")]
        [Route("/Departamento/Index/{itemPage}")]
        public async Task<IActionResult> Index(int itemPage = 1)
        {
            ViewData["Title"] = "Painel de departamentos";

            var departamentos = await _externalService.ObterTodos();

            if (!departamentos.Any())
            {
                return View();
            }

            //if (!string.IsNullOrEmpty(codigoBuscado))
            //{
            //    departamentos = departamentos.Where(dpt => dpt.Codigo.Contains(codigoBuscado)).ToList();
            //}

            var models = new DepartamentosListViewModel()
            {
                Departamentos = departamentos.Skip((itemPage - 1) * PageSize)
                                             .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = itemPage,
                    ItemsPerPage = PageSize,
                    TotalItems = departamentos.Count()
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

            return View(departamento);
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
            if (codigo is null)
            {
                TempData["Erro"] = JsonConvert.SerializeObject(new ResultResponse() { Message = "Código não informado, tente novamente." });
                return RedirectToAction(nameof(Index));
            }

            var departamentos = await _externalService.ObterTodos();
            var departamento = departamentos.FirstOrDefault(dpt => dpt.Codigo == codigo);

            if (departamento is not null)
            {
                ViewData["Title"] = "Atualizar informação de departamento";

                return View(departamento);
            }
            else
            {
                ResultResponses.Add(new ResultResponse() { Message = "Código de departamento informado não existe ou não cadastrado." });
                TempData["Erro"] = JsonConvert.SerializeObject(ResultResponses);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string codigo, [FromBody] DepartamentoDTO departamentoDTO)
        {
            var response = await _externalService.Atualizar(codigo, departamentoDTO);

            if (response.isSucess)
            {
                ResultResponses.AddRange(response.responses);
                var responseSerialized = JsonConvert.SerializeObject(ResultResponses);
                TempData["Notifications"] = responseSerialized;
                return Ok(responseSerialized);
            }
            else
            {
                ViewBag.Erros = response.responses;
                return RedirectToAction(nameof(Index));
            }
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

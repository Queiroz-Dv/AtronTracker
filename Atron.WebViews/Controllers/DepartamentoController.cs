using Atron.Application.DTO;
using Atron.Domain.Entities;
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
        public int PageSize = 5;

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

            var startPage = itemPage - 2;
            var endPage = itemPage + 1;
            var models = new PageInfoDTO();
            models.TotalItems = departamentos.Count;
            models.CurrentPage = itemPage;
            models.ItemsPerPage = PageSize;
            models.Filter = filter;
            //models.Entities = departamentos.Skip((itemPage - 1) * PageSize).Take(PageSize).ToList();
            
            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }

            if (endPage > models.TotalPages)
            {
                endPage = models.TotalPages;
                if (endPage > 5)
                {
                    startPage = endPage - 4;
                }
            }

            models.StartPage = startPage;
            models.EndPage = endPage;
            //models.ItemsPerPage = PageSize;
            //{
            //    CurrentPage = itemPage,
            //    StartPage = ,
            //    EndPage= +4
            //    ItemsPerPage = PageSize,
            //    TotalItems = departamentos.Count,
            //    Filter = filter,
            //    Entities = departamentos.Skip((itemPage - 1) * PageSize).Take(PageSize).ToList()
            //};

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

                var responseSerialized = JsonConvert.SerializeObject(ResultResponses);
                TempData["Notifications"] = responseSerialized;
                return response.isSucess ? RedirectToAction(nameof(Cadastrar)) : View(nameof(Cadastrar), departamento);
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

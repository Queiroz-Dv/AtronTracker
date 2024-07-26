using Atron.Application.DTO;
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

        public List<ResultResponse> ResultResponses { get; set; }

        public DepartamentoController(IDepartamentoExternalService externalService)
        {
            _externalService = externalService;
            ResultResponses = new List<ResultResponse>();
        }

        [HttpGet]
        [Route("/Departamento/Index")]
        [Route("/Departamento/Index/{codigoBuscado}")]
        public async Task<IActionResult> Index(string codigoBuscado)
        {
            ViewData["Title"] = "Painel de departamentos";

            var departamentos = await _externalService.ObterDepartamentos();

            if (!departamentos.Any())
            {
                return View();
            }

            if (!string.IsNullOrEmpty(codigoBuscado))
            {
                departamentos = departamentos.Where(dpt => dpt.Codigo.Contains(codigoBuscado)).ToList();
            }

            return View(departamentos);
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
                var response = await _externalService.CriarDepartamento(departamento);

                if (response.isSucess)
                {
                    ResultResponses.AddRange(response.responses);
                    var responseSerialized = JsonConvert.SerializeObject(ResultResponses);
                    TempData["Notifications"] = responseSerialized;
                    return RedirectToAction(nameof(Cadastrar));
                }
                else
                {
                    ViewBag.Erros = response.responses;
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

            var departamentos = await _externalService.ObterDepartamentos();
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
        public async Task<IActionResult> Atualizar(string codigo, DepartamentoDTO departamentoDTO)
        {
            var response = await _externalService.Atualizar(codigo, departamentoDTO);

            if (response.isSucess)
            {
                ResultResponses.AddRange(response.responses);
                var responseSerialized = JsonConvert.SerializeObject(ResultResponses);
                TempData["Notifications"] = responseSerialized;
                return RedirectToAction(nameof(Index));
            }
            else
            {

                ViewBag.Erros = response.responses;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

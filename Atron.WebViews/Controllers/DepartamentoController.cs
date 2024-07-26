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

            //var departamentoDTO = await _service.ObterPorCodigo(codigo);

            //if (!_service._messages.HasErrors() && departamentoDTO is not null)
            //{
            //    ViewData["Title"] = "Atualizar informação de departamento";

            //    return View(departamentoDTO);
            //}
            //else
            //{
            //    //var erros = _departamentoService.GuardianModel.GetErrors();
            //    //TempData["Erro"] = JsonConvert.SerializeObject(erros);
            //    return RedirectToAction(nameof(Index));
            //}

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string codigo, DepartamentoDTO departamentoDTO)
        {
            //await _service.Atualizar(codigo, departamentoDTO);
            //var temErros = _departamentoService.GuardianModel.HasErrors();

            //if (departamento is not null && temErros is not true)
            //{
            //    await _departamentoService.AtualizarAsync(departamentoDTO);
            //    return RedirectToAction(nameof(Index));
            //}
            //else
            //{
            //    var erros = _departamentoService.GuardianModel.GetErrors();
            //    var json = JsonConvert.SerializeObject(erros, Formatting.Indented);
            //    TempData["Erro"] = json;
            //    return RedirectToAction(nameof(Index));
            //}
            return View(departamentoDTO);
        }
    }
}

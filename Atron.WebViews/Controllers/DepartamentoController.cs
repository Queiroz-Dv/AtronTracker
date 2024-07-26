using Atron.Application.DTO;
using Atron.Application.ViewInterfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Notification.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class DepartamentoController : Controller
    {
        private IDepartamentoViewService _service;

        public DepartamentoController(IDepartamentoViewService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("/Departamento/Index")]
        [Route("/Departamento/Index/{codigoBuscado}")]
        public async Task<IActionResult> Index(string codigoBuscado)
        {
            ViewData["Title"] = "Painel de departamentos";

            var departamentos = await _service.ObterDepartamentos();

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
                await _service.CriarDepartamento(departamento);

                var response = _service.ResultApiJson;

                var responseSerialized = JsonConvert.SerializeObject(response);

                if (_service._messages.HasErrors())
                {
                    ViewBag.Erros = _service._messages;
                    return View(nameof(Cadastrar), departamento);
                }
                else
                {
                    TempData["Notifications"] = responseSerialized;
                    return RedirectToAction(nameof(Cadastrar));
                }
            }

            return View(departamento);
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
            if (codigo is null)
            {
              //  TempData["Erro"] = JsonConvert.SerializeObject(new GuardianMessage("O código informado não foi encontrado", EGuardianMessageType.Error));
                return RedirectToAction(nameof(Index));
            }

            var departamentoDTO = await _service.ObterPorCodigo(codigo);

            if (!_service._messages.HasErrors() && departamentoDTO is not null)
            {
                ViewData["Title"] = "Atualizar informação de departamento";

                return View(departamentoDTO);
            }
            else
            {
                //var erros = _departamentoService.GuardianModel.GetErrors();
                //TempData["Erro"] = JsonConvert.SerializeObject(erros);
                return RedirectToAction(nameof(Index));
            }
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

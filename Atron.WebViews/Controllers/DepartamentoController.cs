using Atron.Application.DTO;
using Atron.Application.ViewInterfaces;
using Microsoft.AspNetCore.Mvc;
using Notification.Models;
using Notification.Services;
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

                if (_service._messages.HasErrors())
                {
                    ViewBag.Erros = _service._messages;
                    return View(nameof(Cadastrar), departamento);
                }
                else
                {
                    var response = _service.GetJsonResponseContent();
                    var responseSerialized = response.ResponseSerialized();
                    TempData["Notifications"] = responseSerialized; 
                    return RedirectToAction(nameof(Cadastrar));
                }
            }

            return View(departamento);
        }
    }
}

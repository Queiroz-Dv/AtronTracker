using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
using Communication.Extensions;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class DepartamentoController : DefaultController<DepartamentoDTO>
    {
        private IDepartamentoExternalService _externalService;

        public DepartamentoController(
            IResultResponseService resultResponse,
            IPaginationService<DepartamentoDTO> paginationService,
            IDepartamentoExternalService externalService)
            : base(paginationService, resultResponse)
        {
            _externalService = externalService;
            CurrentController = nameof(Departamento);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            var departamentos = await _externalService.ObterTodos();

            ConfigureDataTitleForView("Painel de departamentos");
            if (!departamentos.Any())
            {
                return View();
            }

            Filter = filter;
            ConfigurePaginationForView(departamentos, itemPage);

            var model = new DepartamentoModel()
            {
                Departamentos = GetEntitiesPaginated(),
                PageInfo = PageInfo
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            ConfigureDataTitleForView("Cadastro de departamentos");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(DepartamentoDTO departamento)
        {
            if (ModelState.IsValid)
            {
                await _externalService.Criar(departamento);

                var responses = _externalService.ResultResponses;
                CreateTempDataNotifications(responses);
                return !responses.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View(nameof(Cadastrar), departamento);
            }

            ConfigureDataTitleForView("Cadastro de departamentos");
            return View(departamento);
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
            if (codigo is null)
            {
                _responseService.AddError("O código informado não foi encontrado");
                CreateTempDataNotifications();
                return View(codigo);
            }

            var departamentos = await _externalService.ObterTodos();
            var departamentoDTO = departamentos.FirstOrDefault(dpt => dpt.Codigo == codigo);

            ConfigureDataTitleForView("Atualizar informação de departamento");
            return departamentoDTO is not null ? View(departamentoDTO) : View(codigo);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string codigo, DepartamentoDTO departamentoDTO)
        {
            await _externalService.Atualizar(codigo, departamentoDTO);
            var response = _externalService.ResultResponses;
            CreateTempDataNotifications(response);

            return !response.HasErrors() ? RedirectToAction(nameof(Index)) : View(nameof(Atualizar), departamentoDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                _responseService.AddError("Código não informado, tente novamente.");
                CreateTempDataNotifications();
                return RedirectToAction(nameof(Index));
            }

            await _externalService.Remover(codigo);
            CreateTempDataNotifications(_externalService.ResultResponses);
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

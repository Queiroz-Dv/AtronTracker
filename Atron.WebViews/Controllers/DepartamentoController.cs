using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    [Authorize]
    public class DepartamentoController : MainController<DepartamentoDTO, Departamento>
    {
        private readonly IExternalService<DepartamentoDTO> _service;

        public DepartamentoController(IPaginationService<DepartamentoDTO> paginationService,
                                      MessageModel messageModel, IExternalService<DepartamentoDTO> service)
            : base(messageModel, paginationService)
        {
            _paginationService = paginationService;
            _service = service;
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            BuildRoute();
            var departamentos = await _service.ObterTodos();


            ConfigurePaginationForView(new PageInfoDTO<DepartamentoDTO>()
            {
                Entities = departamentos.ToList(),
                CurrentPage = itemPage,
                PageRequestInfo = new PageRequestInfoDTO()
                {
                    Action = nameof(Index),
                    CurrentController = nameof(Departamento),
                    Filter = filter,
                }
            });

            var model = new DepartamentoModel()
            {
                Departamentos = _paginationService.GetPageInfo().Entities,
                PageInfo = _paginationService.GetPageInfo()
            };

            ConfigureDataTitleForView("Painel de departamentos");
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
                BuildRoute();

                await _service.Criar(departamento);

                CreateTempDataMessages();

                return !_messageModel.Messages.HasErrors() ? RedirectToAction(nameof(Cadastrar)) :
                    View(nameof(Cadastrar), departamento);
            }

            ConfigureDataTitleForView("Cadastro de departamentos");

            return View(departamento);
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
            // BuildRoute(nameof(Departamento), codigo);
            var departamentoDTO = await _service.ObterPorCodigo(codigo);

            if (departamentoDTO is null || _messageModel.Messages.HasErrors())
            {
                CreateTempDataMessages();
                return RedirectToAction(nameof(Index));
            }

            ConfigureDataTitleForView("Atualizar informação de departamento");
            return View(departamentoDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string codigo, DepartamentoDTO departamentoDTO)
        {
            //BuildRoute(nameof(Departamento), codigo);
            await _service.Atualizar(codigo, departamentoDTO);
            CreateTempDataMessages();

            return !_messageModel.Messages.HasErrors() ?
            RedirectToAction(nameof(Index)) :
            View(nameof(Atualizar), departamentoDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            // BuildRoute(nameof(Departamento), codigo);
            await _service.Remover(codigo);

            CreateTempDataMessages();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ObterDepartamento(string codigoDepartamento)
        {
            var departamento = await _service.ObterPorCodigo(codigoDepartamento);

            return Ok(departamento);
        }
    }
}

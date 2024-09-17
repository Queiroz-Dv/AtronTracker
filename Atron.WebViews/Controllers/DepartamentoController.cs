using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.WebViews.Models;
using Communication.Extensions;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class DepartamentoController : DefaultController<DepartamentoDTO, Departamento, IDepartamentoExternalService>
    {
        public DepartamentoController(
            IPaginationService<DepartamentoDTO> paginationService,
            IDepartamentoExternalService externalService,
            IApiRouteExternalService apiRouteExternalService,
            IConfiguration configuration,
            IOptions<RotaDeAcesso> appSettingsConfig,
            MessageModel<Departamento> messageModel)
            : base(paginationService,
                  externalService,
                  apiRouteExternalService,
                  configuration,
                  appSettingsConfig,
                  messageModel)
        { }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            var departamentos = await _service.ObterTodos();
            ConfigureDataTitleForView("Painel de departamentos");
            if (!departamentos.Any())
            {
                return View();
            }

            Filter = filter;
            ConfigurePaginationForView(departamentos, itemPage, CurrentController, filter);

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
                await _service.Criar(departamento);
                CreateTempDataMessages();
                return !_messageModel.Messages.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View(nameof(Cadastrar), departamento);
            }

            ConfigureDataTitleForView("Cadastro de departamentos");
            return View(departamento);
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {
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
            await _service.Atualizar(codigo, departamentoDTO);
            CreateTempDataMessages();

            return !_messageModel.Messages.HasErrors() ?
            RedirectToAction(nameof(Index)) :
            View(nameof(Atualizar), departamentoDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Remover(string codigo)
        {
            // if (string.IsNullOrEmpty(codigo))
            // {
            //     _messageModel.AddError("Código não informado, tente novamente.");
            //     CreateTempDataMessages();
            //     return RedirectToAction(nameof(Index));
            // }

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

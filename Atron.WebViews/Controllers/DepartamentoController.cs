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
        //private IDepartamentoExternalService _externalService;
        //private MessageModel<Departamento> _messageModel;

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
        {
            CurrentController = nameof(Departamento);
            BuildRoute(nameof(Departamento));
            _messageModel = messageModel;
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
                await _externalService.Criar(departamento);

                var messages = _externalService.GetMessages();
                CreateTempDataMessages(messages);
                return !messages.HasErrors() ? RedirectToAction(nameof(Cadastrar)) : View(nameof(Cadastrar), departamento);
            }

            ConfigureDataTitleForView("Cadastro de departamentos");
            return View(departamento);
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(string codigo)
        {                   
            var departamentoDTO = await _externalService.ObterPorCodigo(codigo);

            if (departamentoDTO is null || _externalService.GetMessages().HasErrors())
            {
                CreateTempDataMessages(_externalService.GetMessages());
                return RedirectToAction(nameof(Index));
            }

            ConfigureDataTitleForView("Atualizar informação de departamento");
            return View(departamentoDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(string codigo, DepartamentoDTO departamentoDTO)
        {
            await _externalService.Atualizar(codigo, departamentoDTO);
            var response = _messageModel.Messages;
            CreateTempDataMessages(response);

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
            CreateTempDataMessages(_externalService.GetMessages());
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

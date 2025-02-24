using Atron.Application.DTO;
using Atron.Domain.Entities;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers.ControleDeAcesso
{
    [Authorize]
    public class PerfilDeAcessoController : MainController<PerfilDeAcessoDTO, PerfilDeAcesso>
    {
        private readonly IExternalService<PerfilDeAcessoDTO> _service;
        private readonly IExternalService<ModuloDTO> _moduloService;

        public PerfilDeAcessoController(
                        MessageModel messageModel,
                        IExternalService<PerfilDeAcessoDTO> service,
                        IExternalService<ModuloDTO> moduloService,
                        IRouterBuilderService router,
                        IPaginationService<PerfilDeAcessoDTO> paginationService) : base(messageModel, paginationService)
        {
            _service = service;
            _moduloService = moduloService;
            ApiControllerName = nameof(PerfilDeAcesso);
        }

        [HttpGet]
        public Task<IActionResult> GerenciamentoDeAcesso()
        {
            
            ConfigureDataTitleForView("Gerenciamento de acesso");
            return Task.FromResult((IActionResult)View());
        }
    }
}

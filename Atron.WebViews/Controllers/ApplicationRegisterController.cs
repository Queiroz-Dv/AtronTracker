using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class ApplicationRegisterController : MainController<RegisterDTO, ApiRegister>
    {
        private readonly IRegisterExternalService _service;

        public ApplicationRegisterController(
            IRegisterExternalService service,
            IPaginationService<RegisterDTO> paginationService,
            IRouterBuilderService router,
            MessageModel messageModel)
            : base(messageModel, paginationService)
        {
            _router = router;
            _service = service;
            ApiControllerName = "AppRegister";
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View(new RegisterDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(RegisterDTO registerDTO)
        {
            BuildRoute("Registrar");
            await _service.Registrar(registerDTO);

            CreateTempDataMessages();

            return !_messageModel.Messages.HasErrors() ?
                    RedirectToAction("Login", "ApplicationLogin") :
                    View(registerDTO);
        }
    }
}
using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class ApplicationRegisterController : MainController<RegisterDTO, ApiRegister, IRegisterExternalService>
    {
        public ApplicationRegisterController(IUrlModuleFactory urlFactory,
           IPaginationService<RegisterDTO> paginationService,
           IRegisterExternalService service,
           IApiRouteExternalService apiRouteExternalService,
           IConfiguration configuration,
           IOptions<RotaDeAcesso> appSettingsConfig,
           MessageModel<ApiRegister> messageModel) :
            base(urlFactory,
                paginationService,
                service,
                apiRouteExternalService,
                configuration,
                appSettingsConfig,
                messageModel)
        {
            ApiController = "AppRegister";
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View(new RegisterDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(RegisterDTO registerDTO)
        {
            ActionName = nameof(Registrar);
            BuildRoute();

            await _service.Registrar(registerDTO);

            CreateTempDataMessages();

            return !_messageModel.Messages.HasErrors() ?
                    RedirectToAction("Login", "ApplicationLogin") :
                    View(registerDTO);
        }
    }
}
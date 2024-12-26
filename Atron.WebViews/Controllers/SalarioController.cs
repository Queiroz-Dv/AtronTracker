using Atron.Application.DTO;
using Atron.Domain.Entities;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.DTO.API;
using Shared.Interfaces;
using Shared.Models;

namespace Atron.WebViews.Controllers
{
    public class SalarioController : DefaultController<SalarioDTO, Salario, ISalarioExternalService>
    {
        public SalarioController(IUrlModuleFactory urlFactory,
                                 IPaginationService<SalarioDTO> paginationService,
                                 ISalarioExternalService service,
                                 IApiRouteExternalService apiRouteExternalService,
                                 IConfiguration configuration,
                                 IOptions<RotaDeAcesso> appSettingsConfig,
                                 MessageModel<Salario> messageModel) :
            base(
                urlFactory,
                paginationService,
                service,
                apiRouteExternalService,
                configuration,
                appSettingsConfig,
                messageModel)
        {
            CurrentController = nameof(Salario);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}

using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Razor;
using Shared.DTO.API;
using Shared.Interfaces;
using Shared.Models;
using System;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Atron.WebViews.Controllers
{
    public class ApplicationLoginController : MainController<LoginDTO, ApiLogin, ILoginExternalService>
    {
        public ApplicationLoginController(IUrlModuleFactory urlFactory,
            IPaginationService<LoginDTO> paginationService,
            ILoginExternalService service,
            IApiRouteExternalService apiRouteExternalService,
            IConfiguration configuration,
            IOptions<RotaDeAcesso> appSettingsConfig,
            MessageModel<ApiLogin> messageModel) :
            base(urlFactory,
                paginationService,
                service,
                apiRouteExternalService,
                configuration,
                appSettingsConfig,
                messageModel)
        {
            ApiController = "AppLogin";
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginDTO()
            {
                ReturnUrl = returnUrl,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            ActionName = "Logar";
            BuildRoute();
            var login = await _service.Autenticar(loginDTO);

            if (login.Authenticated)
            {
                // Configura o token nos cookies e na sessão
                SetAuthToken(login.UserToken.Token, login.UserToken.Expires);
                return RedirectToAction(nameof(Index), "Home");
            }
            else
            {
                ModelState.AddModelError("", "Usuário não foi autenticado, tente novamente ou contate a administração.");
                return View(loginDTO);
            }
        }
    }
}
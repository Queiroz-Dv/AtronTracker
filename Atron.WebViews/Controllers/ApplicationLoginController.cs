using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using Atron.WebViews.Helpers;
using Communication.Interfaces.Services;
using Communication.Security;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    [RedirectIfAuthenticated]
    public class ApplicationLoginController : MainController<LoginDTO, ApiLogin>
    {
        private readonly ILoginExternalService _service;

        public ApplicationLoginController(
            ILoginExternalService service,
            IPaginationService<LoginDTO> paginationService,
            IRouterBuilderService router,
            MessageModel messageModel) :
            base(messageModel, paginationService)
        {
            _router = router;
            _service = service;
            ApiControllerName = "AppLogin";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutUser()
        {
            BuildRoute("Desconectar");
            await _service.Logout();
            Response.Cookies.Delete("AuthToken");            

            if (!TokenServiceStore.Token.IsNullOrEmpty())
            {
                TokenServiceStore.Token = string.Empty;
            }

            return RedirectToAction(nameof(Login), string.Empty);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            BuildRoute("Logar");
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
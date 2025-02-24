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
            await _service.Logout(); // Aqui ele está deslogando da API

            Response.Cookies.Delete("AuthToken"); // Aqui ele está limpando o token do cookie

            if (!TokenServiceStore.Token.IsNullOrEmpty()) // Aqui ele está limpando o token da sessão
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
            if (ModelState.IsValid)
            {
                BuildRoute("Logar");
                var login = await _service.Autenticar(loginDTO);

                if (login != null)
                {
                    // Configura o token nos cookies e na sessão
                    SetAuthToken(login.UserToken.Token, login.UserToken.Expires);
                    return RedirectToAction("MenuPrincipal", "Home");
                }
                else
                {
                    CreateTempDataMessages();
                    return View(loginDTO);
                }
            }

            return View(loginDTO);
        }
    }
}
using Atron.Application.DTO;
using Atron.Domain.Account;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticate _authenticate;

        public AccountController(IAuthenticate authenticate)
        {
            _authenticate = authenticate;
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
            var result = await _authenticate.Authenticate(loginDTO.Email, loginDTO.Passsword);

            if (result)
            {
                if (loginDTO.ReturnUrl.IsNullOrEmpty())
                {
                    return RedirectToAction(nameof(Index), "Home");
                }

                return Redirect(loginDTO.ReturnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Login inválido. A senha deve mais forte");
                return View(loginDTO);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var result = await _authenticate.RegisterUser(registerDTO.Email, registerDTO.Passsword);

            if (result)
            {
                return Redirect("/");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Registro inválido. A senha deve mais forte");
                return View(registerDTO);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _authenticate.Logout();
            return Redirect("/Account/Login");
        }
    }
}

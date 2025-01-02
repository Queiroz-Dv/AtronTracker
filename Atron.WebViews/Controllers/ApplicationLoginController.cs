using Atron.Application.DTO.Account;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace Atron.WebViews.Controllers
{
    public class ApplicationLoginController : Controller
    {
        private readonly ILoginApplicationRepository _authenticate;

        public ApplicationLoginController(ILoginApplicationRepository authenticate)
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

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}

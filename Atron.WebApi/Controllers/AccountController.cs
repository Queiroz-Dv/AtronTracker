using Atron.Domain.Account;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticate _authenticate;

        public AccountController(IAuthenticate authenticate)
        {
            _authenticate = authenticate;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            return null;
        }

        [HttpGet]
        public Task<IActionResult> Login()
        {
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            return null;
        }

        public async Task<IActionResult> Logout()
        {
            await _authenticate.Logout();
            return null;
        }
    }

    public class RegisterViewModel
    {

    }

    public class LoginViewModel
    {

    }
}

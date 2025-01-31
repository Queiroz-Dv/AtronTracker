using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atron.WebViews.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("Home/MenuPrincipal")] // Esse é o nome que será exibido na URL
        public IActionResult Index() // Esse é o método que é acessado pelas outras controllers
        {
            return View();
        }
    }
}
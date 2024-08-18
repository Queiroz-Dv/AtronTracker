using Microsoft.AspNetCore.Mvc;

namespace Atron.WebApi.Controllers
{
    public class ApiConnectionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

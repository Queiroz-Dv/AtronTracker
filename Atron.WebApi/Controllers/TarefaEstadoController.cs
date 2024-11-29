using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarefaEstadoController : ModuleController<TarefaEstado, ITarefaEstadoService>
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

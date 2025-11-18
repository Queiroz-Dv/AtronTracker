using Application.DTO.ApiDTO;
using Communication.Interfaces.Services;
using Domain.ApiEntities;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Interfaces.Service;
using Shared.Extensions;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class ApplicationRegisterController : MainController<RegisterDTO, ApiRegister>
    {
        private readonly IRegisterExternalService _service;

        public ApplicationRegisterController(
            IRegisterExternalService service,
            IPaginationService<RegisterDTO> paginationService,
            IRouterBuilderService router,
            MessageModel messageModel)
            : base(messageModel, paginationService)
        {
            _router = router;
            _service = service;
            ApiControllerName = "AppRegister";
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                BuildRoute("Registrar");
                await _service.Registrar(registerDTO);
                CreateTempDataMessages();

                return !_messageModel.Notificacoes.HasErrors() ?
                        RedirectToAction("Login", "ApplicationLogin") :
                        View(registerDTO);
            }

            return View(registerDTO);
        }

        [HttpGet]
        public async Task<IActionResult> VerificarUsuarioPorCodigo(string codigo)
        {
            BuildRoute("VerificarUsuarioPorCodigo");
            var usuarioExiste = await _service.UsuarioExiste(codigo);
            return Json(new { usuarioExiste });
        }

        [HttpGet]
        public async Task<IActionResult> VerificarEmail(string email)
        {
            BuildRoute(nameof(VerificarEmail));
            var emailResponse = await _service.EmailExiste(email);
            return Json(new { emailResponse });
        }
    }
}
﻿using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using Communication.Interfaces.Services;
using Communication.Services;
using ExternalServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces;
using Shared.Models;
using System;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class ApplicationLoginController : MainController<LoginDTO, ApiLogin>
    {
        private readonly ILoginExternalService _service;

        public ApplicationLoginController(
            ILoginExternalService service, 
            MessageModel messageModel, 
            IPaginationService<LoginDTO> paginationService,
            IRouterBuilderService router) :
            base(messageModel, paginationService)
        {
            _router = router;
            _service = service;         
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
            _paginationService.ConfigurePageRequestInfo("AppLogin", "Logar");
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
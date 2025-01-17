﻿using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.DTO.API;
using Shared.Interfaces;
using Shared.Models;
using System;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class ApplicationLoginController : DefaultController<LoginDTO, ApiLogin, ILoginExternalService>
    {
        public ApplicationLoginController(IUrlModuleFactory urlFactory,
            IPaginationService<LoginDTO> paginationService,
            ILoginExternalService service,
            IApiRouteExternalService apiRouteExternalService,
            IConfiguration configuration,
            IOptions<RotaDeAcesso> appSettingsConfig,
            MessageModel<ApiLogin> messageModel) :
            base(urlFactory,
                paginationService,
                service,
                apiRouteExternalService,
                configuration,
                appSettingsConfig,
                messageModel)
        {
            CurrentController = "ApplicationLogin";
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
             BuildRoute("AppLogin");
            var login = await _service.Autenticar(loginDTO);

            if (login.Authenticated)
            {
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
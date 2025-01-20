using Atron.WebViews.Helpers;
using Communication.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Shared.DTO;
using Shared.Interfaces;
using Shared.Models;
using Shared.Services;
using System;

namespace Atron.WebViews.Controllers
{
    public class MainController<DTO, Entity> : ServiceConteinerController<DTO, Entity>
    {
        protected IRouterBuilderService _router;

        public MainController(MessageModel messageModel, IPaginationService<DTO> paginationService)               
            : base(messageModel)
        { 
          _paginationService = paginationService;
        }

        // Monta a rota de acordo com o modulo
        protected void BuildRoute()
        {
            var rota = _router.BuildRoute(AppSettings.RotaDeAcesso.Protocolo, AppSettings.RotaDeAcesso.Url);

            _router.BuildUrl(rota, _paginationService.GetPageRequestInfo());
        }

        /// <summary>
        /// Configura a paginação
        /// </summary>
        /// <param name="itens">Lista de entidades a serem paginadas</param>
        /// <param name="itemPage">Número da página atual</param>
        protected virtual void ConfigurePaginationForView(PageInfoDTO<DTO> pageInfo)
        {
            ConfigureViewDataFilter();
            ConfigureViewBagCurrentController();
            _paginationService.ConfigurePagination(pageInfo);
        }

        protected void SetAuthToken(string token, DateTime expires)
        {
            // Configurar o token nos cookies
            HttpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true, // Apenas acessível via HTTP
                Secure = true, // Apenas HTTPS
                SameSite = SameSiteMode.Strict, // Restringir a envio a requisições da mesma origem
                Expires = expires, // Tempo de expiração
            });

            // Configurar o token na sessão
            HttpContext.Session.SetString("AuthToken", token);
        }
    }
}
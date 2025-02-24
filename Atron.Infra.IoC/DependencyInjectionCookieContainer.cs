using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Atron.Infra.IoC
{
    public static class DependencyInjectionCookieContainer
    {
        public static IServiceCollection AddCustomCookieConfiguration(this IServiceCollection services)
        {
            // Configuração do cookie de autenticação
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/";
                options.Cookie = new CookieBuilder
                {
                    Name = "AuthToken",
                    HttpOnly = true,
                    SecurePolicy = CookieSecurePolicy.Always,
                    SameSite = SameSiteMode.Strict,
                    IsEssential = true,
                    MaxAge = TimeSpan.FromDays(31)
                };
                options.LoginPath = "/";
                options.LogoutPath = "/";             
                options.ExpireTimeSpan = TimeSpan.FromDays(31);
            });

            // Configuração da sessão
            services.AddSession(options =>
            {
                options.Cookie.Name = "AuthToken";                            // Nome do cookie
                options.IdleTimeout = TimeSpan.FromDays(31);                 // Tempo de expiração
                options.Cookie.HttpOnly = true;                             // Impede acesso via JavaScript
                options.Cookie.IsEssential = true;                         // Necessário para funcionamento essencial
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // HTTPS obrigatório
                options.Cookie.SameSite = SameSiteMode.Strict;           // Restringe envio a origens externas                
            });

            return services;
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Atron.Infra.IoC
{
    public static class DependencyInjectionCookieContainer
    {
        public static IServiceCollection AddCustomCookieConfiguration(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);                 // Tempo de expiração
                options.Cookie.HttpOnly = true;                             // Impede acesso via JavaScript
                options.Cookie.IsEssential = true;                         // Necessário para funcionamento essencial
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // HTTPS obrigatório
                options.Cookie.SameSite = SameSiteMode.Strict;           // Restringe envio a origens externas
            });
          
            return services;
        }
    }
}
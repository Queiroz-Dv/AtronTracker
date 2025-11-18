using Application.Interfaces.Contexts;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Application.Services.Contexts;
using Application.Services.EntitiesServices;
using Application.Services.Identity;
using Domain.Interfaces.Identity;
using Infrastructure.Repositories.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services;
using Shared.Services.Accessor;
using Shared.Services.Caching;
using Shared.Services.Contexts;
using Shared.Services.Factory;

namespace IoC
{
    public static class DependencyInjectionContexts
    {
        public static IServiceCollection AddContexts(this IServiceCollection services)
        {
            services.AddScoped<IAccessorService, ServiceAccessor>();
            services.AddScoped<ILoginContext, LoginContext>();
            services.AddScoped<IUsuarioContext, UsuarioContext>();
            services.AddScoped<IControleDeSessaoContext, ControleDeSessaoContext>();

            services.AddScoped<ICookieService, CookieService>();
            services.AddScoped<ICookieFactoryService, CookieFactory>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITokenFactoryService, TokenFactory>();

            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ICacheUsuarioService, CacheUsuarioService>();
            services.AddScoped<IDadosComplementaresDoUsuarioService, DadosComplementaresDoUsuarioService>();

            services.AddScoped<IAuthManagerService, AuthManagerContext>();
            services.AddScoped<IUserIdentityService, UserIdentityService>();
            services.AddScoped<IUsuarioIdentityRepository, UserIdentityRepository>();
            return services;
        }
    }
}

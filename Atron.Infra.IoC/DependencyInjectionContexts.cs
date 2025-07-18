using Atron.Application.Interfaces.Contexts;
using Atron.Application.Interfaces.Services;
using Atron.Application.Interfaces.Services.Identity;
using Atron.Application.Services.Contexts;
using Atron.Application.Services.EntitiesServices;
using Atron.Application.Services.Identity;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.Identity;
using Atron.Infrastructure.Repositories;
using Atron.Infrastructure.Repositories.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces.Accessor;
using Shared.Interfaces.Caching;
using Shared.Interfaces.Factory;
using Shared.Interfaces.Services;
using Shared.Services;
using Shared.Services.Accessor;
using Shared.Services.Caching;
using Shared.Services.Factory;

namespace Atron.Infra.IoC
{
    public static class DependencyInjectionContexts
    {
        public static IServiceCollection AddContexts(this IServiceCollection services)
        {
            services.AddScoped<IServiceAccessor, ServiceAccessor>();
            services.AddScoped<ILoginContext, LoginContext>();
            services.AddScoped<IUsuarioContext, UsuarioContext>();
            services.AddScoped<IControleDeSessaoContext, ControleDeSessaoContext>();

            services.AddScoped<ICookieService, CookieService>();
            services.AddScoped<ICookieFactory, CookieFactory>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITokenFactory, TokenFactory>();

            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ICacheUsuarioService, CacheUsuarioService>();
            services.AddScoped<IDadosComplementaresDoUsuarioService, DadosComplementaresDoUsuarioService>();

            services.AddScoped<IUserIdentityService, UserIdentityService>();
            services.AddScoped<IUsuarioIdentityRepository, UsuarioIdentityRepository>();
            services.AddScoped<IRepository<UsuarioIdentity>, Repository<UsuarioIdentity>>();
            services.AddScoped<IPasswordHasher<UsuarioIdentity>, PasswordHasher<UsuarioIdentity>>();
            return services;
        }
    }
}

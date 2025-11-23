using Application.Interfaces.Contexts;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Shared.Application.Interfaces.Service;

namespace Application.Services.Contexts
{
    public class ControleDeSessaoContext : IControleDeSessaoContext
    {
        public ControleDeSessaoContext(IUserIdentityService userIdentityService,
                                       ICacheUsuarioService cacheUsuarioService,
                                       ITokenService tokenService,
                                       ICookieService cookieService,
                                       ICacheService cacheService)
        {
            CacheUsuarioService = cacheUsuarioService;
            TokenService = tokenService;
            CookieService = cookieService;
            CacheService = cacheService;
        }

        public ITokenService TokenService { get; }

        public ICookieService CookieService { get; }

        public ICacheUsuarioService CacheUsuarioService { get; }

        public ICacheService CacheService { get; }

        public IUserIdentityService UserIdentityService { get; }
    }
}
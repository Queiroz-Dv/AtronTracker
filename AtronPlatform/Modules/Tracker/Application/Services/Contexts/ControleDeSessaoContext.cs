using Application.Interfaces.Contexts;
using Application.Interfaces.Services;
using Domain.Interfaces.Identity;
using Shared.Application.Interfaces.Service;

namespace Application.Services.Contexts
{
    public class ControleDeSessaoContext : IControleDeSessaoContext
    {
        public ControleDeSessaoContext(IUsuarioIdentityRepository userIdentityRepository,
                                       ICacheUsuarioService cacheUsuarioService,
                                       ITokenService tokenService,
                                       ICookieService cookieService,
                                       ICacheService cacheService)
        {
            CacheUsuarioService = cacheUsuarioService;
            TokenService = tokenService;
            CookieService = cookieService;
            CacheService = cacheService;
            UserIdentityRepository = userIdentityRepository;
        }

        public ITokenService TokenService { get; }

        public ICookieService CookieService { get; }

        public ICacheUsuarioService CacheUsuarioService { get; }

        public ICacheService CacheService { get; }

        public IUsuarioIdentityRepository UserIdentityRepository { get; }
    }
}



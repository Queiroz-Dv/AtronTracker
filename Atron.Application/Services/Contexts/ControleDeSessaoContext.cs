using Atron.Application.Interfaces.Contexts;
using Atron.Application.Interfaces.Services;
using Shared.Interfaces.Services;

namespace Atron.Application.Services.Contexts
{
    public class ControleDeSessaoContext : IControleDeSessaoContext
    {
        public ControleDeSessaoContext(ICacheUsuarioService cacheUsuarioService,
                                       ITokenService tokenService,
                                       ICookieService cookieService)
        {
            CacheUsuarioService = cacheUsuarioService;
            TokenService = tokenService;
            CookieService = cookieService;
        }

        public ITokenService TokenService { get; }

        public ICookieService CookieService { get; }

        public ICacheUsuarioService CacheUsuarioService { get; }
    }
}
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Shared.Application.Interfaces.Service;

namespace Application.Interfaces.Contexts
{
    public interface IControleDeSessaoContext
    {
        ICacheUsuarioService CacheUsuarioService { get; }

        IUserIdentityService UserIdentityService { get; }

        ITokenService TokenService { get; }

        ICookieService CookieService { get; }

        ICacheService CacheService { get; }
    }
}
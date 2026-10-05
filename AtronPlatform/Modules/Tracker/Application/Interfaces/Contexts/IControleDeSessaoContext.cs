using Application.Interfaces.Services;
using Domain.Interfaces.Identity;
using Shared.Application.Interfaces.Service;

namespace Application.Interfaces.Contexts
{
    public interface IControleDeSessaoContext
    {
        ICacheUsuarioService CacheUsuarioService { get; }

        IUsuarioIdentityRepository UserIdentityRepository { get; }

        ITokenService TokenService { get; }

        ICookieService CookieService { get; }

        ICacheService CacheService { get; }
    }
}
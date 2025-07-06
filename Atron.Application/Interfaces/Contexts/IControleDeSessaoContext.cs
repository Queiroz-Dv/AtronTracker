using Atron.Application.Interfaces.Services;
using Shared.Interfaces.Services;

namespace Atron.Application.Interfaces.Contexts
{
    public interface IControleDeSessaoContext
    {
        ICacheUsuarioService CacheUsuarioService { get; }

        ITokenService TokenService { get; }

        ICookieService CookieService { get; }
    }
}
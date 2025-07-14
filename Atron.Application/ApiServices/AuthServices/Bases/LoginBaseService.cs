using Atron.Application.Interfaces.Contexts;
using Atron.Application.Interfaces.Services;
using Atron.Application.Interfaces.Services.Identity;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Shared.Interfaces.Accessor;
using Shared.Interfaces.Services;

namespace Atron.Application.ApiServices.AuthServices.Bases
{
    public abstract class LoginBaseService : ServiceBase
    {
        protected readonly IServiceAccessor _serviceAccessor;
        protected readonly ILoginRepository _loginRepository;

        public LoginBaseService(IServiceAccessor serviceAccessor, ILoginRepository loginRepository) : base(serviceAccessor)
        {
            _serviceAccessor = serviceAccessor;
            _loginRepository = loginRepository;
        }

        protected ICookieService CookieService => ObterService<ILoginContext>().ControleDeSessaoContext.CookieService;

        protected ITokenService TokenService => ObterService<ILoginContext>()
                                       .ControleDeSessaoContext
                                       .TokenService;

        protected IUsuarioService UsuarioService => ObterService<ILoginContext>()
                    .UsuarioContext
                    .UsuarioService;

        protected ICacheUsuarioService CacheUsuarioService => ObterService<ILoginContext>()
                    .UsuarioContext
                    .CacheUsuarioService;

        protected IDadosComplementaresDoUsuarioService DadosComplementaresDoUsuarioService => ObterService<IUsuarioContext>()
                    .DadosComplementaresDoUsuarioService;

        protected IUserIdentityService UserIdentityService => ObterService<ILoginContext>()
                    .ControleDeSessaoContext
                    .UserIdentityService;
    }
}
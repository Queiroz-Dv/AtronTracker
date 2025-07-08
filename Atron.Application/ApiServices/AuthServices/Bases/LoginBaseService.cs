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

        protected ICookieService CookieService
        {
            get
            {
                return ObterService<ILoginContext>().ControleDeSessaoContext.CookieService;
            }
        }

        protected ITokenService TokenService
        {
            get
            {
                return ObterService<ILoginContext>()
                                       .ControleDeSessaoContext
                                       .TokenService;
            }
        }

        protected IUsuarioService UsuarioService
        {
            get
            {
                return ObterService<ILoginContext>()
                    .UsuarioContext
                    .UsuarioService;
            }
        }

        protected ICacheUsuarioService CacheUsuarioService
        {
            get
            {
                return ObterService<ILoginContext>()
                    .UsuarioContext
                    .CacheUsuarioService;
            }
        }

        protected IDadosComplementaresDoUsuarioService DadosComplementaresDoUsuarioService
        {
            get
            {
                return ObterService<IUsuarioContext>()
                    .DadosComplementaresDoUsuarioService;
            }
        }

        protected IUserIdentityService UserIdentityService
        {
            get
            {
                return ObterService<ILoginContext>()
                    .ControleDeSessaoContext
                    .UserIdentityService;
            }
        }
    }
}
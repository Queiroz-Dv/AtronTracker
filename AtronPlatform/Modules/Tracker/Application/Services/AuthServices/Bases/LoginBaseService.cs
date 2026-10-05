using Application.Interfaces.Contexts;
using Application.Interfaces.Services;
using Application.UseCases.UsuarioCases;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Shared.Application.Interfaces.Service;

namespace Application.Services.AuthServices.Bases
{
    public abstract class LoginBaseService : ServiceBase
    {
        protected readonly IAccessorService _serviceAccessor;
        protected readonly ILoginRepository _loginRepository;

        public LoginBaseService(IAccessorService serviceAccessor, ILoginRepository loginRepository) : base(serviceAccessor)
        {
            _serviceAccessor = serviceAccessor;
            _loginRepository = loginRepository;
        }

        protected ICookieService CookieService => ObterService<ILoginContext>().ControleDeSessaoContext.CookieService;

        protected ITokenService TokenService => ObterService<ILoginContext>()
                                       .ControleDeSessaoContext
                                       .TokenService;

        protected ObterUsuarioCase UsuarioService => ObterService<ILoginContext>()
                    .UsuarioContext
                    .UsuarioService;

        protected ICacheUsuarioService CacheUsuarioService => ObterService<ILoginContext>()
                    .UsuarioContext
                    .CacheUsuarioService;

        protected IDadosComplementaresDoUsuarioService DadosComplementaresDoUsuarioService => ObterService<IUsuarioContext>()
                    .DadosComplementaresDoUsuarioService;

        protected IUsuarioIdentityRepository UserIdentityRepository => ObterService<ILoginContext>()
                    .ControleDeSessaoContext
                    .UserIdentityRepository;
    }
}

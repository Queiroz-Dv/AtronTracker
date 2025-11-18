using Application.Interfaces.Contexts;
using Shared.Application.Interfaces.Service;

namespace Application.Services.Contexts
{
    public class LoginContext : ILoginContext
    {
        public LoginContext(
         IUsuarioContext usuarioContext,
         IAuthManagerService authManagerContext,
         IControleDeSessaoContext controleDeSessaoContext)
        {
            UsuarioContext = usuarioContext;
            AuthManagerContext = authManagerContext;
            ControleDeSessaoContext = controleDeSessaoContext;
        }

        public IUsuarioContext UsuarioContext { get; }
        public IAuthManagerService AuthManagerContext { get; }
        public IControleDeSessaoContext ControleDeSessaoContext { get; }
    }
}
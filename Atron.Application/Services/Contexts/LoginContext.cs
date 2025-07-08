using Atron.Application.Interfaces.Contexts;
using Shared.Interfaces.Contexts;

namespace Atron.Application.Services.Contexts
{
    public class LoginContext : ILoginContext
    {
        public LoginContext(
         IUsuarioContext usuarioContext,
         IAuthManagerContext authManagerContext,
         IControleDeSessaoContext controleDeSessaoContext)
        {
            UsuarioContext = usuarioContext;
            AuthManagerContext = authManagerContext;
            ControleDeSessaoContext = controleDeSessaoContext;
        }

        public IUsuarioContext UsuarioContext { get; }
        public IAuthManagerContext AuthManagerContext { get; }
        public IControleDeSessaoContext ControleDeSessaoContext { get; }
    }
}
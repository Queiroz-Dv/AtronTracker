using Atron.Application.Interfaces.Contexts;

namespace Atron.Application.Services.Contexts
{
    public class LoginContext : ILoginContext
    {
        public LoginContext(
         IUsuarioContext usuarioContext,
         IControleDeSessaoContext controleDeSessaoContext)
        {
            UsuarioContext = usuarioContext;
            ControleDeSessaoContext = controleDeSessaoContext;
        }

        public IUsuarioContext UsuarioContext { get; }
        public IControleDeSessaoContext ControleDeSessaoContext { get; }
    }
}
using Atron.Application.Interfaces.Contexts;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Shared.Interfaces.Contexts;

namespace Atron.Application.Services.Contexts
{
    public class LoginContext : ILoginContext
    {
        public LoginContext(
         IUsuarioContext usuarioContext,
         IAuthManagerContext authManagerContext,
         ILoginRepository loginRepository)
        {
            UsuarioContext = usuarioContext;
            AuthManagerContext = authManagerContext;
            LoginRepository = loginRepository;
        }

        public IUsuarioContext UsuarioContext { get; }
        public IAuthManagerContext AuthManagerContext { get; }
        public ILoginRepository LoginRepository { get; }
    }
}
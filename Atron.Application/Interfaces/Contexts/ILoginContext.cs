using Atron.Domain.Interfaces.ApplicationInterfaces;
using Shared.Interfaces.Contexts;

namespace Atron.Application.Interfaces.Contexts
{
    public interface ILoginContext 
    {
        IUsuarioContext UsuarioContext { get; }

        IAuthManagerContext AuthManagerContext { get; }

        IControleDeSessaoContext ControleDeSessaoContext { get; }        
    }
}
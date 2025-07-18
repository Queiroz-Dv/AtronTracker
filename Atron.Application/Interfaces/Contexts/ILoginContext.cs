using Atron.Domain.Interfaces.ApplicationInterfaces;

namespace Atron.Application.Interfaces.Contexts
{
    public interface ILoginContext 
    {
        IUsuarioContext UsuarioContext { get; }

        IControleDeSessaoContext ControleDeSessaoContext { get; }        
    }
}
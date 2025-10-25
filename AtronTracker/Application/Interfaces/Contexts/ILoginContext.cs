namespace Application.Interfaces.Contexts
{
    public interface ILoginContext
    {
        IUsuarioContext UsuarioContext { get; }

        IControleDeSessaoContext ControleDeSessaoContext { get; }
    }
}
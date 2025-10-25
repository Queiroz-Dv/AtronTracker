using Application.Interfaces.Services;

namespace Application.Interfaces.Contexts
{
    public interface IUsuarioContext
    {
        IUsuarioService UsuarioService { get; }

        ICacheUsuarioService CacheUsuarioService { get; }

        IDadosComplementaresDoUsuarioService DadosComplementaresDoUsuarioService { get; }
    }
}
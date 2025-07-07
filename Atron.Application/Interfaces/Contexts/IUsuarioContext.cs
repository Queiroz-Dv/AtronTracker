using Atron.Application.Interfaces.Services;

namespace Atron.Application.Interfaces.Contexts
{
    public interface IUsuarioContext
    {
        IUsuarioService UsuarioService { get; }

        ICacheUsuarioService CacheUsuarioService { get; }

        IDadosComplementaresDoUsuarioService DadosComplementaresDoUsuarioService { get; }
    }
}
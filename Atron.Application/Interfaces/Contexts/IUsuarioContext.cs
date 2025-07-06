using Atron.Application.Interfaces.Services;
using Atron.Domain.Interfaces.UsuarioInterfaces;

namespace Atron.Application.Interfaces.Contexts
{
    public interface IUsuarioContext
    {
        IUsuarioService UsuarioService { get; }

        IUsuarioRepository UsuarioRepository { get; }

        ICacheUsuarioService CacheUsuarioService { get; }

        IDadosComplementaresDoUsuarioService DadosComplementaresDoUsuarioService { get; }
    }
}
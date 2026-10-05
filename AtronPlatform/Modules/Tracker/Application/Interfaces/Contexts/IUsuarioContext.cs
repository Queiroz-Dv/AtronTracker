using Application.UseCases.UsuarioCases;
using Application.Interfaces.Services;

namespace Application.Interfaces.Contexts
{
    public interface IUsuarioContext
    {
        ObterUsuarioCase UsuarioService { get; }

        ICacheUsuarioService CacheUsuarioService { get; }

        IDadosComplementaresDoUsuarioService DadosComplementaresDoUsuarioService { get; }
    }
}
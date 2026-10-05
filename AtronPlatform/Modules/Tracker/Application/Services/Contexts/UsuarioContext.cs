using Application.UseCases.UsuarioCases;
using Application.Interfaces.Contexts;
using Application.Interfaces.Services;
using Domain.Interfaces.UsuarioInterfaces;

namespace Application.Services.Contexts
{
    public class UsuarioContext : IUsuarioContext
    {
        public UsuarioContext(ObterUsuarioCase usuarioService,
                              IUsuarioRepository usuarioRepository,
                              ICacheUsuarioService cacheUsuarioService,
                              IDadosComplementaresDoUsuarioService dadosComplementaresDoUsuarioService)
        {
            UsuarioService = usuarioService;
            UsuarioRepository = usuarioRepository;
            CacheUsuarioService = cacheUsuarioService;
            DadosComplementaresDoUsuarioService = dadosComplementaresDoUsuarioService;
        }

        public ObterUsuarioCase UsuarioService { get; }

        public IUsuarioRepository UsuarioRepository { get; }

        public ICacheUsuarioService CacheUsuarioService { get; }

        public IDadosComplementaresDoUsuarioService DadosComplementaresDoUsuarioService { get; }
    }
}
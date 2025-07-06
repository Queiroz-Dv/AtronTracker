using Atron.Application.Interfaces.Contexts;
using Atron.Application.Interfaces.Services;
using Atron.Domain.Interfaces.UsuarioInterfaces;

namespace Atron.Application.Services.Contexts
{
    public class UsuarioContext : IUsuarioContext
    {
        public UsuarioContext(IUsuarioService usuarioService,
                              IUsuarioRepository usuarioRepository,
                              ICacheUsuarioService cacheUsuarioService,
                              IDadosComplementaresDoUsuarioService dadosComplementaresDoUsuarioService)
        {
            UsuarioService = usuarioService;
            UsuarioRepository = usuarioRepository;
            CacheUsuarioService = cacheUsuarioService;
            DadosComplementaresDoUsuarioService = dadosComplementaresDoUsuarioService;
        }

        public IUsuarioService UsuarioService { get; }

        public IUsuarioRepository UsuarioRepository { get; }

        public ICacheUsuarioService CacheUsuarioService { get; }

        public IDadosComplementaresDoUsuarioService DadosComplementaresDoUsuarioService { get; }
    }
}
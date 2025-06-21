using Atron.Application.DTO;
using Atron.Application.DTO.ApiDTO;
using Atron.Application.Interfaces;
using Atron.Application.Interfaces.Handlers;
using Shared.DTO.API;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services.Handlers
{
    public class UsuarioHandler : IUsuarioHandler
    {
        private readonly IPerfilDeAcessoService _perfilDeAcessoService;

        public UsuarioHandler(IPerfilDeAcessoService perfilDeAcessoService)
        {
            _perfilDeAcessoService = perfilDeAcessoService;
        }

        public async Task<LoginDTO> PreencherInformacoesDeUsuarioParaLoginAsync(UsuarioDTO usuarioDTO)
        {
            var loginDTO = new LoginDTO
            {
                DadosDoUsuario = new DadosDoUsuario()
                {
                    NomeDoUsuario = usuarioDTO.Nome,
                    CodigoDoUsuario = usuarioDTO.Codigo,
                    Email = usuarioDTO.Email,
                    CodigoDoCargo = usuarioDTO.CargoCodigo,
                    CodigoDoDepartamento = usuarioDTO.DepartamentoCodigo,
                    DadosDoToken = new DadosDoToken(DateTime.Now.AddMinutes(1), DateTime.Now.AddDays(7))
                }
            };

            // Obtém os perfis associados
            var perfisAssociados = await _perfilDeAcessoService.ObterPerfisPorCodigoUsuarioServiceAsync(usuarioDTO.Codigo);

            foreach (var perf in perfisAssociados)
            {
                // Adiciona o código de cada perfil na lista do usuário
                var perfilComModulo = new PerfilComModulos { CodigoPerfil = perf.Codigo };

                foreach (var mod in perf.Modulos)
                {                    
                    // Verifica se o código do módulo não existe dentro da lista do usuário                    
                    if (!loginDTO.DadosDoUsuario.PerfisDeAcesso.Any(x => x.Modulos.Any(m => m.Codigo == mod.Codigo)))
                    {
                        // Adiciona o código do módulo na lista do usuário
                        perfilComModulo.Modulos.Add(new DadosDoModulo(mod.Codigo, mod.Descricao));
                    }
                }

                // Adiciona o perfil com os módulos na lista de perfis de acesso do usuário
                loginDTO.DadosDoUsuario.PerfisDeAcesso.Add(perfilComModulo);
            }

            return loginDTO;
        }
    }    
}
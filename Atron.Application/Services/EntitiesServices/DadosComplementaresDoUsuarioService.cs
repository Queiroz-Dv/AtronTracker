using Atron.Application.DTO;
using Atron.Application.Interfaces.Services;
using Shared.DTO.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services.EntitiesServices
{
    public class DadosComplementaresDoUsuarioService : IDadosComplementaresDoUsuarioService
    {
        private readonly IPerfilDeAcessoService _perfilDeAcessoService;

        public DadosComplementaresDoUsuarioService(IPerfilDeAcessoService perfilDeAcessoService)
        {
            _perfilDeAcessoService = perfilDeAcessoService;
        }

        public async Task<DadosComplementaresDoUsuarioDTO> ObterInformacoesComplementaresDoUsuario(UsuarioDTO usuarioDTO)
        {
            var dadosComplementares = new DadosComplementaresDoUsuarioDTO
            {
                DadosDoUsuario = new DadosDoUsuarioDTO
                {
                    NomeDoUsuario = usuarioDTO.Nome,
                    CodigoDoUsuario = usuarioDTO.Codigo,
                    Email = usuarioDTO.Email,
                    CodigoDoCargo = usuarioDTO.CargoCodigo,
                    CodigoDoDepartamento = usuarioDTO.DepartamentoCodigo,                    
                },

                DadosDoPerfil = new List<DadosDoPerfilDTO>(),
                DadosDoToken = new TempoDosTokensDoUsuarioDTO(DateTime.Now.AddMinutes(15), DateTime.Now.AddDays(7))
            };

            // Obtém os perfis associados
            var perfisAssociados = await _perfilDeAcessoService.ObterPerfisPorCodigoUsuarioServiceAsync(usuarioDTO.Codigo);
            
            foreach (var perf in perfisAssociados)
            {
                // Adiciona o código de cada perfil na lista do usuário
                var perfilComModulo = new DadosDoPerfilDTO { CodigoPerfil = perf.Codigo };

                foreach (var mod in perf.Modulos)
                {
                    // Verifica se o código do módulo não existe dentro da lista do usuário                    
                    if (!dadosComplementares.DadosDoPerfil.Any(x => x.Modulos.Any(m => m.Codigo == mod.Codigo)))
                    {
                        // Adiciona o código do módulo na lista do usuário
                        perfilComModulo.Modulos.Add(new DadosDoModuloDTO(mod.Codigo, mod.Descricao));
                    }
                }

                // Adiciona o perfil com os módulos na lista de perfis de acesso do usuário
                dadosComplementares.DadosDoPerfil.Add(perfilComModulo);
            }

            return dadosComplementares;
        }
    }
}
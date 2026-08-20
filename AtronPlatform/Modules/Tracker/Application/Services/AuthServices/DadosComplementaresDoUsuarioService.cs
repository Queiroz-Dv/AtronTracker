using Application.DTO;
using Application.Interfaces.Services;
using Shared.Application.DTOS.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.AuthServices
{
    public class DadosComplementaresDoUsuarioService(IPerfilDeAcessoService perfilDeAcessoService) : IDadosComplementaresDoUsuarioService
    {
        private readonly IPerfilDeAcessoService _perfilDeAcessoService = perfilDeAcessoService;

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

                DadosDoPerfil = [],
                DadosDoToken = new TempoDosTokensDoUsuarioDTO(DateTime.UtcNow.AddMinutes(15), DateTime.UtcNow.AddDays(7))
            };

            var perfisAssociados = await _perfilDeAcessoService.ObterPerfisPorCodigoUsuarioAsync(usuarioDTO.Codigo);

            foreach (var perf in perfisAssociados)
            {
                var perfilComModulo = new DadosDoPerfilDTO { CodigoPerfil = perf.Codigo };

                foreach (var mod in perf.Modulos)
                {
                    if (!dadosComplementares.DadosDoPerfil.Any(x => x.Modulos.Any(m => m.Codigo == mod.Codigo)))
                    {
                        perfilComModulo.Modulos.Add(new DadosDoModuloDTO(mod.Codigo, mod.Descricao));
                    }
                }

                dadosComplementares.DadosDoPerfil.Add(perfilComModulo);
            }

            return dadosComplementares;
        }
    }
}
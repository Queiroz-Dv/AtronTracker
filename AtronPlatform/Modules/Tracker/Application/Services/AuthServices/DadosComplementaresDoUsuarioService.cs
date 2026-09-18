using Application.DTO;
using Application.Interfaces.Services;
using Application.UseCases.WorkspaceCases;
using Shared.Application.DTOS.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.AuthServices
{
    public class DadosComplementaresDoUsuarioService(
        ObterWorkspaceCase workspaceCase,
        IPerfilDeAcessoService perfilDeAcessoService) : IDadosComplementaresDoUsuarioService
    {

        public async Task<DadosComplementaresDoUsuarioDTO> ObterInformacoesComplementaresDoUsuario(UsuarioDTO usuarioDTO)
        {
            var workspaceResultado = await workspaceCase.ObterPorDadosDoUsuario(usuarioDTO.Codigo, usuarioDTO.Email);
            var workspace = workspaceResultado.Dados!;

            var dadosComplementares = new DadosComplementaresDoUsuarioDTO
            {
                DadosDoUsuario = new DadosDoUsuarioDTO
                {
                    NomeDoUsuario = usuarioDTO.Nome,
                    CodigoDoUsuario = usuarioDTO.Codigo,
                    Email = usuarioDTO.Email,
                    CodigoDoCargo = usuarioDTO.CargoCodigo,
                    CodigoDoDepartamento = usuarioDTO.DepartamentoCodigo,
                    Workspace = workspace != null ? new WorkspaceDoUsuarioDTO()
                    {
                        Codigo = workspace.Codigo,
                        Descricao = workspace.Descricao
                    } : null,
                },
                DadosDoPerfil = [],
                DadosDoToken = new TempoDosTokensDoUsuarioDTO(DateTime.UtcNow.AddMinutes(15), DateTime.UtcNow.AddDays(7))
            };

            var perfisAssociados = await perfilDeAcessoService.ObterPerfisPorCodigoUsuarioAsync(usuarioDTO.Codigo);

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
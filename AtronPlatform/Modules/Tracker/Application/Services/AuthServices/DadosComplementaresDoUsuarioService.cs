using Application.DTO;
using Application.Interfaces.Services;
using Domain.Enums;
using Domain.Interfaces;
using Shared.Application.DTOS.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.AuthServices
{
    public class DadosComplementaresDoUsuarioService(
        IPerfilDeAcessoService perfilDeAcessoService,
        IEmpresaRepository empresaRepository = null) : IDadosComplementaresDoUsuarioService
    {
        private readonly IPerfilDeAcessoService _perfilDeAcessoService = perfilDeAcessoService;
        private readonly IEmpresaRepository _empresaRepository = empresaRepository;

        public async Task<DadosComplementaresDoUsuarioDTO> ObterInformacoesComplementaresDoUsuario(UsuarioDTO usuarioDTO)
        {
            var vinculo = _empresaRepository is null
                ? null
                : await _empresaRepository.ObterVinculoAsync(usuarioDTO.Id, usuarioDTO.Codigo);
            var dadosDaEmpresa = vinculo is null
                ? null
                : new DadosDaEmpresaDTO
                {
                    Id = vinculo.EmpresaId,
                    Codigo = vinculo.Empresa.Codigo,
                    NomeFantasia = vinculo.Empresa.NomeFantasia,
                    AcessoPermitido = vinculo.Status == StatusUsuarioEmpresa.Ativo
                        && vinculo.Empresa.Status == StatusEmpresa.Ativa
                };

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
                DadosDaEmpresa = dadosDaEmpresa,
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

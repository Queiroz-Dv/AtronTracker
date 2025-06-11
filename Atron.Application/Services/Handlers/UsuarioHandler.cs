using Atron.Application.DTO;
using Atron.Application.DTO.ApiDTO;
using Atron.Application.Interfaces;
using Atron.Application.Interfaces.Handlers;
using Shared.DTO.API;
using System;
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
                // Dados adicionais para as claims do usuário
                DadosDoUsuario = new DadosDoUsuario()
                {
                    NomeDoUsuario = usuarioDTO.Nome,
                    CodigoDoUsuario = usuarioDTO.Codigo,
                    Email = usuarioDTO.Email,
                    CodigoDoCargo = usuarioDTO.CargoCodigo,
                    CodigoDoDepartamento = usuarioDTO.DepartamentoCodigo,
                    ExpiracaoDeAcesso = DateTime.Now.AddMinutes(5),
                    DadosDoToken = new DadosDoToken
                    {
                        ExpiracaoDoToken = DateTime.Now.AddMinutes(5),
                        ExpiracaoDoRefreshToken = DateTime.Now.AddDays(7)
                    },
                }
            };

            // Obtém os perfis associados
            var perfisAssociados = await _perfilDeAcessoService.ObterPerfisPorCodigoUsuarioServiceAsync(usuarioDTO.Codigo);

            foreach (var perf in perfisAssociados)
            {
                // Adiciona o código de cada perfil na lista do usuário
                loginDTO.DadosDoUsuario.CodigosPerfis.Add(perf.Codigo);

                foreach (var mod in perf.Modulos)
                {
                    // Verifica se o código do módulo não existe dentro da lista do usuário
                    if (!loginDTO.DadosDoUsuario.ModulosCodigo.Contains(mod.Codigo))
                    {
                        // Adiciona o código do módulo na lista do usuário
                        loginDTO.DadosDoUsuario.ModulosCodigo.Add(mod.Codigo);
                    }
                }
            }

            return loginDTO;
        }
    }
}

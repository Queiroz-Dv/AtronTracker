using Application.DTO;
using Application.Interfaces.Services;
using Application.UseCases.PerfilDeAcessoCases;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.PerfisDeAcesso
{
    public class PerfilDeAcessoService(
        CriarPerfilDeAcessoCase criarPerfil,
        AtualizarPerfilDeAcessoCase atualizarPerfil,
        RemoverPerfilDeAcessoCase removerPerfil,
        ObterPerfilDeAcessoCase obterPerfil,
        ObterPerfisUsuarioCase obterPerfisUsuario,
        IPerfilDeAcessoUsuarioRelacionamentoService relacionamentoService) : IPerfilDeAcessoService
    {
        private readonly CriarPerfilDeAcessoCase _criarPerfil = criarPerfil;
        private readonly AtualizarPerfilDeAcessoCase _atualizarPerfil = atualizarPerfil;
        private readonly RemoverPerfilDeAcessoCase _removerPerfil = removerPerfil;
        private readonly ObterPerfilDeAcessoCase _obterPerfil = obterPerfil;
        private readonly ObterPerfisUsuarioCase _obterPerfisUsuario = obterPerfisUsuario;
        private readonly IPerfilDeAcessoUsuarioRelacionamentoService _relacionamentoService = relacionamentoService;

        public Task<Resultado<List<PerfilDeAcessoDTO>>> ObterTodosAsync()
            => _obterPerfil.ObterTodosAsync();

        public Task<Resultado<PerfilDeAcessoDTO>> ObterPorCodigoAsync(string codigo)
            => _obterPerfil.ObterPorCodigoAsync(codigo);

        public Task<Resultado<PerfilDeAcessoDTO>> CriarAsync(PerfilDeAcessoDTO perfilDeAcessoDTO)
            => _criarPerfil.ExecutarAsync(perfilDeAcessoDTO);

        public Task<Resultado<PerfilDeAcessoDTO>> AtualizarAsync(
            string codigo,
            PerfilDeAcessoDTO perfilDeAcessoDTO)
            => _atualizarPerfil.ExecutarAsync(codigo, perfilDeAcessoDTO);

        public Task<Resultado> RemoverAsync(string codigo)
            => _removerPerfil.ExecutarAsync(codigo);

        public async Task<Resultado<PerfilDeAcessoUsuarioDTO>> RelacionarPerfilDeAcessoUsuarioAsync(
            PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario)
        {
            var resultado = await _relacionamentoService.RelacionarAsync(perfilDeAcessoUsuario);
            return resultado.TeveFalha
                ? Resultado<PerfilDeAcessoUsuarioDTO>.Falhas(resultado.Messages)
                : Resultado<PerfilDeAcessoUsuarioDTO>.Sucesso(perfilDeAcessoUsuario);
        }

        public Task<Resultado<PerfilDeAcessoUsuarioDTO>> ObterRelacionamentoDePerfilUsuarioPorCodigoAsync(
            string codigo)
            => _obterPerfil.ObterRelacionamentoAsync(codigo);

        public Task<List<PerfilDeAcessoDTO>> ObterPerfisPorCodigoUsuarioAsync(string usuarioCodigo)
            => _obterPerfisUsuario.ExecutarAsync(usuarioCodigo);
    }
}

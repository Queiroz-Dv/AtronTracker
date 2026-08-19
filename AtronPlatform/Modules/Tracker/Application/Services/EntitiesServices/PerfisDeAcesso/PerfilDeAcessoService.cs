using Application.DTO;
using Application.Interfaces.Mapping;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.PerfisDeAcesso
{
    public class PerfilDeAcessoService(
        IPerfilDeAcessoMapping map,
        IPerfilDeAcessoRepository perfilDeAcessoRepository,
        IPerfilDeAcessoPreparacaoService preparacaoService,
        IPerfilDeAcessoUsuarioRelacionamentoService relacionamentoService,
        IPerfilDeAcessoCacheInvalidator cacheInvalidator) : IPerfilDeAcessoService
    {
        private readonly IPerfilDeAcessoMapping _map = map;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository = perfilDeAcessoRepository;
        private readonly IPerfilDeAcessoPreparacaoService _preparacaoService = preparacaoService;
        private readonly IPerfilDeAcessoUsuarioRelacionamentoService _relacionamentoService = relacionamentoService;
        private readonly IPerfilDeAcessoCacheInvalidator _cacheInvalidator = cacheInvalidator;

        public async Task<Resultado<List<PerfilDeAcessoDTO>>> ObterTodosAsync()
        {
            var perfis = await _perfilDeAcessoRepository.ObterTodosPerfisRepositoryAsync();
            var dtos = _map.MapToDtos(perfis).ToList();
            return Resultado<List<PerfilDeAcessoDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<PerfilDeAcessoDTO>> ObterPorCodigoAsync(string codigo)
        {
            var perfil = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            return perfil is null
                ? Resultado<PerfilDeAcessoDTO>.Falha(PerfilDeAcessoResource.Erro_RegistroNaoEncontrado)
                : Resultado<PerfilDeAcessoDTO>.Sucesso(_map.MapToDto(perfil));
        }

        public async Task<Resultado<PerfilDeAcessoDTO>> CriarAsync(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            var preparacao = await _preparacaoService.PrepararAsync(perfilDeAcessoDTO);
            if (preparacao.TeveFalha)
                return Resultado<PerfilDeAcessoDTO>.Falhas(preparacao.Messages);

            var criado = await _perfilDeAcessoRepository.CriarPerfilRepositoryAsync(preparacao.Dados);
            return criado
                ? Resultado<PerfilDeAcessoDTO>.Sucesso(perfilDeAcessoDTO).AdicionarMensagem(string.Format(PerfilDeAcessoResource.Mensagem_PerfilCriado, preparacao.Dados.Codigo))
                : Resultado<PerfilDeAcessoDTO>.Falha(PerfilDeAcessoResource.Erro_CriarPerfil);
        }

        public async Task<Resultado<PerfilDeAcessoDTO>> AtualizarAsync(string codigo, PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            var preparacao = await _preparacaoService.PrepararAsync(perfilDeAcessoDTO);
            if (preparacao.TeveFalha)
                return Resultado<PerfilDeAcessoDTO>.Falhas(preparacao.Messages);

            var perfilAtual = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            var atualizado = await _perfilDeAcessoRepository.AtualizarPerfilRepositoryAsync(codigo, preparacao.Dados);
            if (!atualizado)
                return Resultado<PerfilDeAcessoDTO>.Falha(PerfilDeAcessoResource.Erro_AtualizarPerfil);

            _cacheInvalidator.InvalidarUsuariosDoPerfil(perfilAtual);
            return Resultado<PerfilDeAcessoDTO>.Sucesso(perfilDeAcessoDTO)
                .AdicionarMensagem(string.Format(PerfilDeAcessoResource.Mensagem_PerfilAtualizado, preparacao.Dados.Codigo));
        }

        public async Task<Resultado> RemoverAsync(string codigo)
        {
            var perfil = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            if (perfil is null)
                return Resultado.Falha(PerfilDeAcessoResource.Erro_RegistroNaoEncontrado);

            var removido = await _perfilDeAcessoRepository.DeletarPerfilRepositoryAsync(perfil);
            if (!removido)
                return Resultado.Falha(PerfilDeAcessoResource.Erro_RemoverPerfil);

            _cacheInvalidator.InvalidarUsuariosDoPerfil(perfil);
            return Resultado.Sucesso(PerfilDeAcessoResource.Mensagem_PerfilRemovido);
        }

        public async Task<Resultado<PerfilDeAcessoUsuarioDTO>> RelacionarPerfilDeAcessoUsuarioAsync(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario)
        {
            var resultado = await _relacionamentoService.RelacionarAsync(perfilDeAcessoUsuario);
            return resultado.TeveFalha
                ? Resultado<PerfilDeAcessoUsuarioDTO>.Falhas(resultado.Messages)
                : Resultado<PerfilDeAcessoUsuarioDTO>.Sucesso(perfilDeAcessoUsuario);
        }

        public async Task<Resultado<PerfilDeAcessoUsuarioDTO>> ObterRelacionamentoDePerfilUsuarioPorCodigoAsync(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return Resultado<PerfilDeAcessoUsuarioDTO>.Sucesso(new PerfilDeAcessoUsuarioDTO());

            var perfilDeAcesso = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            if (perfilDeAcesso is null)
                return Resultado<PerfilDeAcessoUsuarioDTO>.Falha(PerfilDeAcessoResource.Erro_RegistroNaoEncontrado);

            var dto = _map.MapToPerfilDeAcessoUsuarioDto(perfilDeAcesso);
            return Resultado<PerfilDeAcessoUsuarioDTO>.Sucesso(dto);
        }

        public async Task<List<PerfilDeAcessoDTO>> ObterPerfisPorCodigoUsuarioAsync(string usuarioCodigo)
        {
            var perfis = await _perfilDeAcessoRepository.ObterPerfisPorCodigoDeUsuarioRepositoryAsync(usuarioCodigo);
            return perfis is null ? null : _map.MapToDtos(perfis).ToList();
        }
    }
}

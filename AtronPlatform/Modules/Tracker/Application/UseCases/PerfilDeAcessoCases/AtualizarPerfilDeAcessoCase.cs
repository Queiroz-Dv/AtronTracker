using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Application.Services.EntitiesServices.PerfisDeAcesso;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.PerfilDeAcessoCases
{
    public sealed class AtualizarPerfilDeAcessoCase(
        PerfilDeAcessoPreparacaoService preparacaoService,
        IPerfilDeAcessoRepository perfilDeAcessoRepository,
        IPerfilDeAcessoCacheInvalidator cacheInvalidator)
    {
        private readonly PerfilDeAcessoPreparacaoService _preparacaoService = preparacaoService;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository = perfilDeAcessoRepository;
        private readonly IPerfilDeAcessoCacheInvalidator _cacheInvalidator = cacheInvalidator;

        public async Task<Resultado<PerfilDeAcessoDTO>> ExecutarAsync(
            string codigo,
            PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            var preparacao = await _preparacaoService.PrepararAsync(perfilDeAcessoDTO);
            if (preparacao.TeveFalha)
                return Resultado<PerfilDeAcessoDTO>.Falhas(preparacao.Messages);

            var perfilAtual = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            if (perfilAtual is null)
                return Resultado<PerfilDeAcessoDTO>.Falha(PerfilDeAcessoResource.Erro_RegistroNaoEncontrado);

            var perfilPreparado = preparacao.Dados!;
            var atualizado = await _perfilDeAcessoRepository
                .AtualizarPerfilRepositoryAsync(codigo, perfilPreparado);
            if (!atualizado)
                return Resultado<PerfilDeAcessoDTO>.Falha(PerfilDeAcessoResource.Erro_AtualizarPerfil);

            _cacheInvalidator.InvalidarUsuariosDoPerfil(perfilAtual);

            return Resultado<PerfilDeAcessoDTO>
                .Sucesso(perfilDeAcessoDTO)
                .AdicionarMensagem(string.Format(
                    PerfilDeAcessoResource.Mensagem_PerfilAtualizado,
                    perfilPreparado.Codigo));
        }
    }
}
using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.PerfilDeAcessoCases
{
    public sealed class CriarPerfilDeAcessoCase(
        IPerfilDeAcessoPreparacaoService preparacaoService,
        IPerfilDeAcessoRepository perfilDeAcessoRepository)
    {
        private readonly IPerfilDeAcessoPreparacaoService _preparacaoService = preparacaoService;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository = perfilDeAcessoRepository;

        public async Task<Resultado<PerfilDeAcessoDTO>> ExecutarAsync(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            var preparacao = await _preparacaoService.PrepararAsync(perfilDeAcessoDTO);
            if (preparacao.TeveFalha)
                return Resultado<PerfilDeAcessoDTO>.Falhas(preparacao.Messages);

            var perfil = preparacao.Dados!;
            var criado = await _perfilDeAcessoRepository.CriarPerfilRepositoryAsync(perfil);

            return criado
                ? Resultado<PerfilDeAcessoDTO>
                    .Sucesso(perfilDeAcessoDTO)
                    .AdicionarMensagem(string.Format(PerfilDeAcessoResource.Mensagem_PerfilCriado, perfil.Codigo))
                : Resultado<PerfilDeAcessoDTO>.Falha(PerfilDeAcessoResource.Erro_CriarPerfil);
        }
    }
}

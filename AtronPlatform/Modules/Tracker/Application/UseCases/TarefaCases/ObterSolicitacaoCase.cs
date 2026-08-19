using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases
{
    public class ObterSolicitacaoCase(
        IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> mapper,
        IUsuarioService usuarioService, 
        ISolicitacaoObtencaoTarefaRepository solicitacaoRepository)
    {
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly ISolicitacaoObtencaoTarefaRepository Repository = solicitacaoRepository;
        private readonly IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> Mapper = mapper;

        public async Task<Resultado<IReadOnlyCollection<SolicitacaoObtencaoTarefaDTO>>> ExecutarAsync()
        {
            var usuarioResultado = await _usuarioService.ObterUsuarioAtual();
            if (usuarioResultado.TeveFalha)
                return Resultado<IReadOnlyCollection<SolicitacaoObtencaoTarefaDTO>>.Falhas(usuarioResultado.Messages);

            var usuario = usuarioResultado.Dados;

            var solicitacoes = await Repository.ObterPendentesPorAprovadorAsync(usuario.Id, usuario.Codigo);
            var dtos = Mapper.MapToDtos(solicitacoes).ToList();

            return Resultado<IReadOnlyCollection<SolicitacaoObtencaoTarefaDTO>>.Sucesso(dtos);
        }
    }
}
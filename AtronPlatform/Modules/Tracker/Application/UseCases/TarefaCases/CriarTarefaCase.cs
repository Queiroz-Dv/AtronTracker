using Application.DTO;
using Application.Events;
using Application.Extensions;
using Application.Resources;
using Application.Services.EntitiesServices.Tarefas;
using Application.UseCases.TarefaCases.Movimentacao;
using Application.UseCases.UsuarioCases;
using Domain.Enums;
using Domain.Interfaces;
using Shared.Application.Messaging;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.UseCases.TarefaCases
{
    public class CriarTarefaCase(
        ITarefaRepository tarefaRepository,
        TarefaPreparacaoService tarefaPreparacaoService,
        IEventBus eventBus,
        ObterUsuarioCase usuarioService,
        CriarTarefaMovimentacaoCase criarMovimentacao)
    {
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly TarefaPreparacaoService _tarefaPreparacaoService = tarefaPreparacaoService;
        private readonly IEventBus _eventBus = eventBus;
        private readonly ObterUsuarioCase _usuarioService = usuarioService;

        private readonly CriarTarefaMovimentacaoCase _criarMovimentacao = criarMovimentacao;

        public async Task<Resultado> ExecutarAsync(TarefaDTO tarefaDTO)
        {
            var responsavelResultado = await _usuarioService.ObterAsync();
            if (responsavelResultado.TeveFalha)
                return Resultado.Falha(responsavelResultado.Messages);

            if (tarefaDTO.DestinoInicial == DestinoInicialTarefa.Equipe.GetDescription())
            {
                var departamentoEquipeResultado = DefinirDepartamentoEquipePorTarefaCase.Executar(tarefaDTO, responsavelResultado.Dados!);
                if (departamentoEquipeResultado.TeveFalha)
                    return Resultado.Falha(departamentoEquipeResultado.Messages);
            }

            var preparacaoResultado = await _tarefaPreparacaoService.PrepararParaPersistenciaAsync(tarefaDTO);
            if (preparacaoResultado.TeveFalha)
                return Resultado.Falha(preparacaoResultado.Messages);

            var tarefa = preparacaoResultado.Dados;
            var responsavel = responsavelResultado.Dados!;

            if (!await _tarefaRepository.CriarTarefaAsync(tarefa))
                return Resultado.Falha(TarefaResource.Erro_GravarTarefa);

            var movimentacao = await _criarMovimentacao.ExecutarAsync(tarefa, responsavel);
            if (movimentacao.TeveFalha)
                return Resultado.Falha(movimentacao.Messages);

            tarefaDTO.Id = tarefa.Id;
            var resultado = Resultado.Sucesso().AdicionarMensagem(TarefaResource.Mensagem_TarefaCriada);

            var notificacaoEvent = new TarefaNotificacaoEvent(
                tarefaDTO.CriarNotificacaoDeAtribuicao(),
                tarefaDTO,
                tarefaDTO.Usuario);

            await _eventBus.PublicarAsync(notificacaoEvent);

            return resultado;
        }
    }
}
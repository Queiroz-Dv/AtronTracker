using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases
{
    public class CriarTarefa(
        ITarefaRepository tarefaRepository,
        ITarefaPreparacaoService tarefaPreparacaoService,
        ITarefaNotificacaoService tarefaNotificacaoService,
        ITarefaNotificacaoInternaService notificacaoInternaService,
        ITarefaMovimentacaoService tarefaMovimentacaoService,
        ITarefaUsuarioAtualService usuarioAtualService)
    {
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly ITarefaPreparacaoService _tarefaPreparacaoService = tarefaPreparacaoService;
        private readonly ITarefaNotificacaoService _tarefaNotificacaoService = tarefaNotificacaoService;
        private readonly ITarefaNotificacaoInternaService _notificacaoInternaService = notificacaoInternaService;
        private readonly ITarefaMovimentacaoService _tarefaMovimentacaoService = tarefaMovimentacaoService;
        private readonly ITarefaUsuarioAtualService _usuarioAtualService = usuarioAtualService;

        public async Task<Resultado<TarefaDTO>> ExecutarAsync(TarefaDTO tarefaDTO)
        {
            var preparacaoResultado = await _tarefaPreparacaoService.PrepararParaPersistenciaAsync(tarefaDTO);
            if (preparacaoResultado.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(preparacaoResultado.Messages);

            var tarefaPreparada = preparacaoResultado.Dados;
            var tarefa = tarefaPreparada.Tarefa;

            var responsavel = await _usuarioAtualService.ObterAsync();
            if (responsavel.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(responsavel.Messages);

            if (!await _tarefaRepository.CriarTarefaAsync(tarefa))
                return Resultado<TarefaDTO>.Falha(TarefaResource.Erro_GravarTarefa);

            var movimentacao = await _tarefaMovimentacaoService.RegistrarCriacaoAsync(tarefa, responsavel.Dados);
            if (movimentacao.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(movimentacao.Messages);

            tarefaDTO.Id = tarefa.Id;
            tarefaDTO.Identificador = tarefa.Identificador;
            var resultado = Resultado<TarefaDTO>.Sucesso(tarefaDTO).AdicionarMensagem(TarefaResource.Mensagem_TarefaCriada);

            await _notificacaoInternaService.NotificarAtribuicaoAsync(tarefa, tarefaPreparada.Usuario);
            var envioEmail = await _tarefaNotificacaoService.NotificarAtribuicaoAsync(tarefaDTO, tarefaPreparada.Usuario);
            if (envioEmail.TeveFalha)
                resultado.AdicionarAviso(TarefaResource.Aviso_EmailNotificacaoNaoEnviado);

            return resultado;
        }
    }
}

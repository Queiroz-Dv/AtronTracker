using Application.DTO;
using Application.Extensions;
using Application.Interfaces.Services;
using Application.Resources;
using Application.UseCases.TarefaCases.Movimentacao;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases
{
    public class CriarTarefaCase(
        ITarefaRepository tarefaRepository,
        ITarefaPreparacaoService tarefaPreparacaoService,
        ITarefaNotificacaoService tarefaNotificacaoService,
        TarefaNotificacaoInternaCase notificacaoInternaCase,
        IUsuarioService usuarioService,
        CriarTarefaMovimentacaoCase criarMovimentacao)
    {
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly ITarefaPreparacaoService _tarefaPreparacaoService = tarefaPreparacaoService;
        private readonly ITarefaNotificacaoService _tarefaNotificacaoService = tarefaNotificacaoService;
        private readonly TarefaNotificacaoInternaCase _notificacaoInternaCase = notificacaoInternaCase;
        private readonly IUsuarioService _usuarioService = usuarioService;

        private readonly CriarTarefaMovimentacaoCase _criarMovimentacao = criarMovimentacao;

        public async Task<Resultado<TarefaDTO>> ExecutarAsync(TarefaDTO tarefaDTO)
        {
            var preparacaoResultado = await _tarefaPreparacaoService.PrepararParaPersistenciaAsync(tarefaDTO);
            if (preparacaoResultado.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(preparacaoResultado.Messages);

            var tarefa = preparacaoResultado.Dados;

            var responsavelResultado = await _usuarioService.ObterUsuarioAtual();
            if (responsavelResultado.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(responsavelResultado.Messages);

            var responsavel = responsavelResultado.Dados;

            if (!await _tarefaRepository.CriarTarefaAsync(tarefa))
                return Resultado<TarefaDTO>.Falha(TarefaResource.Erro_GravarTarefa);

            var movimentacao = await _criarMovimentacao.ExecutarAsync(tarefa, responsavel);
            if (movimentacao.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(movimentacao.Messages);

            tarefaDTO.Id = tarefa.Id;
            tarefaDTO.Identificador = tarefa.Identificador;
            var resultado = Resultado<TarefaDTO>.Sucesso(tarefaDTO).AdicionarMensagem(TarefaResource.Mensagem_TarefaCriada);

            await _notificacaoInternaCase.ExecutarAsync(tarefaDTO.CriarNotificacaoDeAtribuicao());
            var envioEmail = await _tarefaNotificacaoService.NotificarAtribuicaoAsync(tarefaDTO, tarefaDTO.Usuario);

            if (envioEmail.TeveFalha)
                resultado.AdicionarAviso(TarefaResource.Aviso_EmailNotificacaoNaoEnviado);

            return resultado;
        }
    }
}

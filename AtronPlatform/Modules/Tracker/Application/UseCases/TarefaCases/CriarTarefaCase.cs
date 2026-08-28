using Application.DTO;
using Application.Extensions;
using Application.Interfaces.Services;
using Application.Resources;
using Application.UseCases.TarefaCases.Movimentacao;
using Domain.Enums;
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

        public async Task<Resultado> ExecutarAsync(TarefaDTO tarefaDTO)
        {
            var responsavelResultado = await _usuarioService.ObterUsuarioAtual();
            if (responsavelResultado.TeveFalha)
                return Resultado.Falha(responsavelResultado.Messages);

            if (tarefaDTO.DestinoInicial == (int)DestinoInicialTarefa.Equipe)
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

            await _notificacaoInternaCase.ExecutarAsync(tarefaDTO.CriarNotificacaoDeAtribuicao());
            var envioEmail = await _tarefaNotificacaoService.NotificarAtribuicaoAsync(tarefaDTO, tarefaDTO.Usuario);

            if (envioEmail.TeveFalha)
                resultado.AdicionarAviso(TarefaResource.Aviso_EmailNotificacaoNaoEnviado);

            return resultado;
        }
    }
}

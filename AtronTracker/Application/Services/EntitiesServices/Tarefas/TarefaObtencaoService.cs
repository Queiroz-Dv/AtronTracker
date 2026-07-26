using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaObtencaoService(
        ITarefaRepository tarefaRepository,
        ISolicitacaoObtencaoTarefaRepository solicitacaoRepository,
        ITarefaUsuarioAtualService usuarioAtualService,
        ITarefaObtencaoValidador validador,
        IAprovadorObtencaoTarefaResolver aprovadorResolver,
        ISolicitacaoObtencaoTarefaMapeador mapeador,
        ITarefaNotificacaoInternaService notificacaoInternaService,
        IAsyncApplicationMapService<TarefaDTO, Tarefa> tarefaMapeador) : ITarefaObtencaoService
    {
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly ISolicitacaoObtencaoTarefaRepository _solicitacaoRepository = solicitacaoRepository;
        private readonly ITarefaUsuarioAtualService _usuarioAtualService = usuarioAtualService;
        private readonly ITarefaObtencaoValidador _validador = validador;
        private readonly IAprovadorObtencaoTarefaResolver _aprovadorResolver = aprovadorResolver;
        private readonly ISolicitacaoObtencaoTarefaMapeador _mapeador = mapeador;
        private readonly ITarefaNotificacaoInternaService _notificacaoInternaService = notificacaoInternaService;
        private readonly IAsyncApplicationMapService<TarefaDTO, Tarefa> _tarefaMapeador = tarefaMapeador;

        public async Task<Resultado<List<SolicitacaoObtencaoTarefaDTO>>> ObterSolicitacoesAsync()
        {
            var usuario = await _usuarioAtualService.ObterAsync();
            if (usuario.TeveFalha)
                return Resultado<List<SolicitacaoObtencaoTarefaDTO>>.Falhas(usuario.Messages);

            var solicitacoes = await _solicitacaoRepository.ObterPendentesPorAprovadorAsync(usuario.Dados.Id, usuario.Dados.Codigo);
            var dtos = new List<SolicitacaoObtencaoTarefaDTO>();
            foreach (var solicitacao in solicitacoes)
                dtos.Add(await _mapeador.MapearAsync(solicitacao));

            return Resultado<List<SolicitacaoObtencaoTarefaDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<TarefaDTO>> AssumirAsync(int tarefaId)
        {
            var usuario = await _usuarioAtualService.ObterAsync();
            if (usuario.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(usuario.Messages);

            var tarefa = await _tarefaRepository.ObterTarefaPorId(tarefaId);
            if (tarefa is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var validacao = _validador.ValidarAssuncao(usuario.Dados, tarefa);
            if (validacao.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(validacao.Messages);

            if (!await _tarefaRepository.AssumirTarefaAsync(tarefaId, usuario.Dados.Id, usuario.Dados.Codigo))
                return Resultado<TarefaDTO>.Falha(TarefaResource.Erro_AssumirTarefa);

            var tarefaAtualizada = await _tarefaRepository.ObterTarefaPorId(tarefaId);
            await _notificacaoInternaService.NotificarObtencaoAsync(tarefaAtualizada, usuario.Dados);
            var dto = await _tarefaMapeador.MapToDTOAsync(tarefaAtualizada);
            return Resultado<TarefaDTO>.Sucesso(dto).AdicionarMensagem(TarefaResource.Mensagem_TarefaAssumida);
        }

        public async Task<Resultado<SolicitacaoObtencaoTarefaDTO>> SolicitarAsync(int tarefaId)
        {
            var usuario = await _usuarioAtualService.ObterAsync();
            if (usuario.TeveFalha)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falhas(usuario.Messages);

            var tarefa = await _tarefaRepository.ObterTarefaPorId(tarefaId);
            if (tarefa is null)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var validacao = _validador.ValidarSolicitacao(usuario.Dados, tarefa);
            if (validacao.TeveFalha)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falhas(validacao.Messages);

            if (await _solicitacaoRepository.ExisteSolicitacaoPendenteParaTarefaAsync(tarefaId))
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha(TarefaResource.Erro_SolicitacaoPendenteExistente);

            var aprovador = await _aprovadorResolver.ResolverAsync(usuario.Dados, tarefa);
            if (aprovador is null)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha(TarefaResource.Erro_AprovadorIndisponivel);

            var solicitacao = SolicitacaoObtencaoTarefa.CriarPendente(tarefa, usuario.Dados, aprovador);
            if (!await _solicitacaoRepository.CriarAsync(solicitacao))
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha(TarefaResource.Erro_CriarSolicitacao);

            var solicitacaoGravada = await _solicitacaoRepository.ObterPorIdAsync(solicitacao.Id);
            await _notificacaoInternaService.NotificarSolicitacaoRecebidaAsync(solicitacaoGravada);

            var dto = await _mapeador.MapearAsync(solicitacaoGravada);
            return Resultado<SolicitacaoObtencaoTarefaDTO>.Sucesso(dto).AdicionarMensagem(TarefaResource.Mensagem_SolicitacaoEnviada);
        }

        public async Task<Resultado<SolicitacaoObtencaoTarefaDTO>> DecidirAsync(int solicitacaoId, bool aprovar)
        {
            var usuarioResultado = await _usuarioAtualService.ObterAsync();
            var usuario = usuarioResultado.Dados;
            if (usuarioResultado.TeveFalha)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falhas(usuarioResultado.Messages);

            var atualizado = aprovar
                ? await _solicitacaoRepository.AprovarAsync(solicitacaoId, usuario.Id, usuario.Codigo)
                : await _solicitacaoRepository.RecusarAsync(solicitacaoId, usuario.Id, usuario.Codigo);
            if (!atualizado)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha(TarefaResource.Erro_DecidirSolicitacao);

            var solicitacao = await _solicitacaoRepository.ObterPorIdAsync(solicitacaoId);
            var dto = await _mapeador.MapearAsync(solicitacao);

            await _notificacaoInternaService.NotificarDecisaoSolicitacaoAsync(solicitacao, aprovar);
            return Resultado<SolicitacaoObtencaoTarefaDTO>
                .Sucesso(dto)
                .AdicionarMensagem(aprovar ? TarefaResource.Mensagem_SolicitacaoAprovada : TarefaResource.Mensagem_SolicitacaoRecusada);
        }
    }
}

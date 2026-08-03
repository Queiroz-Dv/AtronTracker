using Application.DTO;
using Application.DTO.Request;
using Application.Interfaces.Services;
using Application.Resources;
using Application.UseCases.TarefaCases;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class TarefaService(
        IAsyncApplicationMapService<TarefaDTO, Tarefa> map,
        ITarefaRepository tarefaRepository,
        CriarTarefa criarTarefa,
        ITarefaPreparacaoService tarefaPreparacaoService,
        ITarefaObtencaoService tarefaObtencaoService,
        ITarefaUsuarioAtualService usuarioAtualService,
        ITarefaConfiguracoesService tarefaConfiguracoesService,
        ITarefaMovimentacaoService tarefaMovimentacaoService) : ITarefaService
    {
        private readonly IAsyncApplicationMapService<TarefaDTO, Tarefa> _map = map;
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly CriarTarefa _criarTarefa = criarTarefa;
        private readonly ITarefaPreparacaoService _tarefaPreparacaoService = tarefaPreparacaoService;
        private readonly ITarefaObtencaoService _tarefaObtencaoService = tarefaObtencaoService;
        private readonly ITarefaUsuarioAtualService _usuarioAtualService = usuarioAtualService;
        private readonly ITarefaConfiguracoesService _tarefaConfiguracoesService = tarefaConfiguracoesService;
        private readonly ITarefaMovimentacaoService _tarefaMovimentacaoService = tarefaMovimentacaoService;

        public async Task<Resultado<TarefaDTO>> CriarAsync(TarefaDTO tarefaDTO)
            => await _criarTarefa.ExecutarAsync(tarefaDTO);

        public async Task<Resultado<List<TarefaDTO>>> ObterTodosAsync()
        {
            var tarefas = await _tarefaRepository.ObterTodasTarefas();
            return Resultado<List<TarefaDTO>>.Sucesso(await _map.MapToListDTOAsync(tarefas));
        }

        public async Task<Resultado<List<TarefaDTO>>> ObterMeuQuadroAsync()
        {
            var usuario = await _usuarioAtualService.ObterAsync();
            if (usuario.TeveFalha)
                return Resultado<List<TarefaDTO>>.Falhas(usuario.Messages);

            var tarefas = await _tarefaRepository.ObterTarefasAtivasPorUsuarioAsync(usuario.Dados.Id, usuario.Dados.Codigo);
            return Resultado<List<TarefaDTO>>.Sucesso(await _map.MapToListDTOAsync([.. tarefas]));
        }

        public async Task<Resultado<List<TarefaDTO>>> ObterEquipeAsync()
        {
            var usuario = await _usuarioAtualService.ObterAsync();
            if (usuario.TeveFalha)
                return Resultado<List<TarefaDTO>>.Falhas(usuario.Messages);

            var tarefas = await _tarefaRepository.ObterTarefasAtivasPorSubordinadosDiretosAsync(usuario.Dados.Id, usuario.Dados.Codigo);
            return Resultado<List<TarefaDTO>>.Sucesso(await _map.MapToListDTOAsync([.. tarefas]));
        }

        public async Task<Resultado<List<TarefaDTO>>> ObterDisponiveisAsync()
        {
            var usuario = await _usuarioAtualService.ObterAsync();
            if (usuario.TeveFalha)
                return Resultado<List<TarefaDTO>>.Falhas(usuario.Messages);

            var tarefas = await _tarefaRepository.ObterTarefasAtivasDisponiveisAsync();

            return Resultado<List<TarefaDTO>>.Sucesso(await _map.MapToListDTOAsync([.. tarefas]));
        }

        public async Task<Resultado<TarefaAcessoDTO>> ObterAcessoAsync()
        {
            var usuario = await _usuarioAtualService.ObterAsync();
            if (usuario.TeveFalha)
                return Resultado<TarefaAcessoDTO>.Falhas(usuario.Messages);

            var possuiResponsabilidadeGestao = await _tarefaRepository
                .PossuiResponsabilidadeGestaoAsync(usuario.Dados.Id, usuario.Dados.Codigo);

            return Resultado<TarefaAcessoDTO>.Sucesso(new TarefaAcessoDTO
            {
                PossuiResponsabilidadeGestao = possuiResponsabilidadeGestao
            });
        }

        public async Task<Resultado<TarefaDTO>> AtualizarAsync(int id, TarefaDTO tarefaDTO)
        {
            var tarefaAnterior = await _tarefaRepository.ObterTarefaPorId(id);
            if (tarefaAnterior is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var preparacaoResultado = await _tarefaPreparacaoService.PrepararParaPersistenciaAsync(tarefaDTO);
            if (preparacaoResultado.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(preparacaoResultado.Messages);

            var responsavel = await _usuarioAtualService.ObterAsync();
            if (responsavel.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(responsavel.Messages);

            var tarefaPreparada = preparacaoResultado.Dados;
            var movimentacao = await _tarefaMovimentacaoService.RegistrarAtualizacaoAsync(
                tarefaAnterior,
                tarefaPreparada.Tarefa,
                responsavel.Dados);
            if (movimentacao.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(movimentacao.Messages);

            if (!await _tarefaRepository.AtualizarTarefaAsync(id, tarefaPreparada.Tarefa))
                return Resultado<TarefaDTO>.Falha(TarefaResource.Erro_AtualizarTarefa);

            tarefaDTO.Id = id;
            return Resultado<TarefaDTO>.Sucesso(tarefaDTO).AdicionarMensagem(TarefaResource.Mensagem_TarefaAtualizada);
        }

        public async Task<Resultado> ExcluirAsync(string id)
        {
            if (!int.TryParse(id, out var tarefaId))
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var tarefa = await _tarefaRepository.ObterTarefaPorId(tarefaId);
            if (tarefa is null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            if (!await _tarefaRepository.RemoverRepositoryAsync(tarefa))
                return Resultado.Falha(TarefaResource.Erro_RemoverTarefa);

            return Resultado.Sucesso().AdicionarMensagem(TarefaResource.Mensagem_TarefaRemovida);
        }

        public async Task<Resultado<TarefaDTO>> ObterPorId(int id)
        {
            var tarefa = await _tarefaRepository.ObterTarefaPorId(id);
            return tarefa is null
                ? Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado)
                : Resultado<TarefaDTO>.Sucesso(await _map.MapToDTOAsync(tarefa));
        }

        public Task<Resultado<TarefaDTO>> AssumirAsync(int id)
            => _tarefaObtencaoService.AssumirAsync(id);

        public Task<Resultado<SolicitacaoObtencaoTarefaDTO>> SolicitarObtencaoAsync(int id)
            => _tarefaObtencaoService.SolicitarAsync(id);

        public Task<Resultado<SolicitacaoObtencaoTarefaDTO>> AprovarSolicitacaoAsync(int id)
            => _tarefaObtencaoService.DecidirAsync(id, aprovar: true);

        public Task<Resultado<SolicitacaoObtencaoTarefaDTO>> RecusarSolicitacaoAsync(int id)
            => _tarefaObtencaoService.DecidirAsync(id, aprovar: false);

        public Task<Resultado<List<SolicitacaoObtencaoTarefaDTO>>> ObterSolicitacoesAsync()
           => _tarefaObtencaoService.ObterSolicitacoesAsync();

        public Task<Resultado<TarefaMovimentacaoPaginaDTO>> ObterHistoricoAsync(
            int tarefaId,
            int pagina,
            int tamanhoPagina)
            => _tarefaMovimentacaoService.ObterAsync(tarefaId, pagina, tamanhoPagina);

        public Task<Resultado<List<TarefaEstadoDTO>>> ObterEstadosAsync()
            => _tarefaPreparacaoService.ObterEstadosAsync();

        public Task<Resultado<TarefaConfiguracoesDTO>> ObterConfiguracoesAsync()
            => _tarefaConfiguracoesService.ObterAsync();

        public Task<Resultado<TarefaConfiguracoesDTO>> AtualizarConfiguracoesAsync(TarefaConfiguracoesRequest request)
            => _tarefaConfiguracoesService.AtualizarAsync(request);
    }
}

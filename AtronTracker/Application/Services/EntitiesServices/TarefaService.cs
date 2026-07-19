using Application.DTO;
using Application.DTO.Request;
using Application.Extensions;
using Application.Interfaces.Services;
using Application.Resources;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class TarefaService : ITarefaService
    {
        private readonly IAsyncApplicationMapService<TarefaDTO, Tarefa> _map;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly ITarefaPreparacaoService _tarefaPreparacaoService;
        private readonly ITarefaNotificacaoService _tarefaNotificacaoService;
        private readonly ITarefaNotificacaoInternaService _notificacaoInternaService;
        private readonly ITarefaObtencaoService _tarefaObtencaoService;
        private readonly ITarefaUsuarioAtualService _usuarioAtualService;
        private readonly ITarefaConfiguracoesService _tarefaConfiguracoesService;

        public TarefaService(
            IAsyncApplicationMapService<TarefaDTO, Tarefa> map,
            ITarefaRepository tarefaRepository,
            ITarefaPreparacaoService tarefaPreparacaoService,
            ITarefaNotificacaoService tarefaNotificacaoService,
            ITarefaNotificacaoInternaService notificacaoInternaService,
            ITarefaObtencaoService tarefaObtencaoService,
            ITarefaUsuarioAtualService usuarioAtualService,
            ITarefaConfiguracoesService tarefaConfiguracoesService)
        {
            _map = map;
            _tarefaRepository = tarefaRepository;
            _tarefaPreparacaoService = tarefaPreparacaoService;
            _tarefaNotificacaoService = tarefaNotificacaoService;
            _notificacaoInternaService = notificacaoInternaService;
            _tarefaObtencaoService = tarefaObtencaoService;
            _usuarioAtualService = usuarioAtualService;
            _tarefaConfiguracoesService = tarefaConfiguracoesService;
        }

        public async Task<Resultado<TarefaDTO>> CriarAsync(TarefaDTO tarefaDTO)
        {
            var preparacao = await _tarefaPreparacaoService.PrepararParaPersistenciaAsync(tarefaDTO);
            if (preparacao.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(preparacao.Messages);

            if (!await _tarefaRepository.CriarTarefaAsync(preparacao.Dados.Entidade))
                return Resultado<TarefaDTO>.Falha(TarefaResource.Erro_GravarTarefa);

            tarefaDTO.Id = preparacao.Dados.Entidade.Id;
            tarefaDTO.Identificador = preparacao.Dados.Entidade.Identificador;
            var resultado = Resultado<TarefaDTO>.Sucesso(tarefaDTO).AdicionarMensagem(TarefaResource.Mensagem_TarefaCriada);

            await _notificacaoInternaService.NotificarAtribuicaoAsync(preparacao.Dados.Entidade, preparacao.Dados.Usuario);
            var envioEmail = await _tarefaNotificacaoService.NotificarAtribuicaoAsync(tarefaDTO, preparacao.Dados.Usuario);
            if (envioEmail.TeveFalha)
                resultado.AdicionarAviso(TarefaResource.Aviso_EmailNotificacaoNaoEnviado);

            return resultado;
        }

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

            var tarefas = await _tarefaRepository.ObterTarefasAtivasDisponiveisParaUsuarioAsync(
                usuario.Dados.Id,
                usuario.Dados.Codigo,
                usuario.Dados.ObterDepartamentoIdsParaTarefas(),
                usuario.Dados.ObterCargoIdsParaTarefas());

            return Resultado<List<TarefaDTO>>.Sucesso(await _map.MapToListDTOAsync([.. tarefas]));
        }

        public Task<Resultado<List<SolicitacaoObtencaoTarefaDTO>>> ObterSolicitacoesAsync()
        {
            return _tarefaObtencaoService.ObterSolicitacoesAsync();
        }

        public Task<Resultado<List<TarefaEstadoDTO>>> ObterEstadosAsync()
        {
            return _tarefaPreparacaoService.ObterEstadosAsync();
        }

        public Task<Resultado<TarefaConfiguracoesDTO>> ObterConfiguracoesAsync()
        {
            return _tarefaConfiguracoesService.ObterAsync();
        }

        public Task<Resultado<TarefaConfiguracoesDTO>> AtualizarConfiguracoesAsync(TarefaConfiguracoesRequest request)
        {
            return _tarefaConfiguracoesService.AtualizarAsync(request);
        }

        public async Task<Resultado<TarefaDTO>> AtualizarAsync(int id, TarefaDTO tarefaDTO)
        {
            if (await _tarefaRepository.ObterTarefaPorId(id) is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var preparacao = await _tarefaPreparacaoService.PrepararParaPersistenciaAsync(tarefaDTO);
            if (preparacao.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(preparacao.Messages);

            if (!await _tarefaRepository.AtualizarTarefaAsync(id, preparacao.Dados.Entidade))
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

        public Task<Resultado<TarefaDTO>> AssumirAsync(int id)
        {
            return _tarefaObtencaoService.AssumirAsync(id);
        }

        public Task<Resultado<SolicitacaoObtencaoTarefaDTO>> SolicitarObtencaoAsync(int id)
        {
            return _tarefaObtencaoService.SolicitarAsync(id);
        }

        public Task<Resultado<SolicitacaoObtencaoTarefaDTO>> AprovarSolicitacaoAsync(int id)
        {
            return _tarefaObtencaoService.DecidirAsync(id, aprovar: true);
        }

        public Task<Resultado<SolicitacaoObtencaoTarefaDTO>> RecusarSolicitacaoAsync(int id)
        {
            return _tarefaObtencaoService.DecidirAsync(id, aprovar: false);
        }

        public async Task<Resultado<TarefaDTO>> ObterPorId(int id)
        {
            var tarefa = await _tarefaRepository.ObterTarefaPorId(id);
            return tarefa is null
                ? Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado)
                : Resultado<TarefaDTO>.Sucesso(await _map.MapToDTOAsync(tarefa));
        }
    }
}

using Application.DTO;
using Application.Interfaces.Services;
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

        public TarefaService(
            IAsyncApplicationMapService<TarefaDTO, Tarefa> map,
            ITarefaRepository tarefaRepository,
            ITarefaPreparacaoService tarefaPreparacaoService,
            ITarefaNotificacaoService tarefaNotificacaoService)
        {
            _map = map;
            _tarefaRepository = tarefaRepository;
            _tarefaPreparacaoService = tarefaPreparacaoService;
            _tarefaNotificacaoService = tarefaNotificacaoService;
        }

        public async Task<Resultado<TarefaDTO>> CriarAsync(TarefaDTO tarefaDTO)
        {
            var preparacao = await _tarefaPreparacaoService.PrepararParaPersistenciaAsync(tarefaDTO);
            if (preparacao.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(preparacao.Messages);

            var gravado = await _tarefaRepository.CriarTarefaAsync(preparacao.Dados.Entidade);
            if (!gravado)
                return Resultado<TarefaDTO>.Falha("Erro ao gravar a tarefa.");

            tarefaDTO.Id = preparacao.Dados.Entidade.Id;
            var resultado = Resultado<TarefaDTO>
                .Sucesso(tarefaDTO)
                .AdicionarMensagem("Tarefa gravada com sucesso.");

            var envioEmail = await _tarefaNotificacaoService.NotificarAtribuicaoAsync(tarefaDTO, preparacao.Dados.Usuario);
            if (envioEmail.TeveFalha)
            {
                resultado.AdicionarAviso("Tarefa criada, mas nao foi possivel enviar o e-mail de notificacao.");
            }

            return resultado;
        }

        public async Task<Resultado<List<TarefaDTO>>> ObterTodosAsync()
        {
            var tarefas = await _tarefaRepository.ObterTodasTarefas();
            var dtos = await _map.MapToListDTOAsync(tarefas);
            return Resultado<List<TarefaDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<List<TarefaEstadoDTO>>> ObterEstadosAsync()
        {
            return await _tarefaPreparacaoService.ObterEstadosAsync();
        }

        public async Task<Resultado<TarefaDTO>> AtualizarAsync(int id, TarefaDTO tarefaDTO)
        {
            var tarefaExistente = await _tarefaRepository.ObterTarefaPorId(id);
            if (tarefaExistente is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var preparacao = await _tarefaPreparacaoService.PrepararParaPersistenciaAsync(tarefaDTO);
            if (preparacao.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(preparacao.Messages);

            var atualizado = await _tarefaRepository.AtualizarTarefaAsync(id, preparacao.Dados.Entidade);
            if (!atualizado)
                return Resultado<TarefaDTO>.Falha("Erro ao atualizar a tarefa.");

            tarefaDTO.Id = id;
            return Resultado<TarefaDTO>
                .Sucesso(tarefaDTO)
                .AdicionarMensagem("Tarefa atualizada com sucesso.");
        }

        public async Task<Resultado> ExcluirAsync(string id)
        {
            if (!int.TryParse(id, out var tarefaId))
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var tarefa = await _tarefaRepository.ObterTarefaPorId(tarefaId);
            if (tarefa is null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var deletado = await _tarefaRepository.RemoverRepositoryAsync(tarefa);
            if (!deletado)
                return Resultado.Falha("Erro ao remover a tarefa.");

            return Resultado
                .Sucesso()
                .AdicionarMensagem("Tarefa removida com sucesso.");
        }

        public async Task<Resultado<TarefaDTO>> ObterPorId(int id)
        {
            var tarefaRepository = await _tarefaRepository.ObterTarefaPorId(id);
            if (tarefaRepository is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dto = await _map.MapToDTOAsync(tarefaRepository);
            return Resultado<TarefaDTO>.Sucesso(dto);
        }
    }
}

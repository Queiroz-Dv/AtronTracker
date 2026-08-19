using Application.DTO;
using Application.Interfaces.Services;
using Application.UseCases.TarefaCases;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class TarefaService(
        CriarTarefaCase criarTarefa,
        AtualizarTarefaCase atualizarTarefa,
        ExcluirTarefaCase excluirTarefa,
        ObterTarefaCase obterTarefa) : ITarefaService
    {
        private readonly CriarTarefaCase _criarTarefa = criarTarefa;
        private readonly AtualizarTarefaCase _atualizarTarefa = atualizarTarefa;
        private readonly ExcluirTarefaCase _excluirTarefa = excluirTarefa;
        private readonly ObterTarefaCase _obterTarefa = obterTarefa;

        public async Task<Resultado<TarefaDTO>> CriarAsync(TarefaDTO tarefaDTO)
            => await _criarTarefa.ExecutarAsync(tarefaDTO);

        public async Task<Resultado<TarefaDTO>> AtualizarAsync(int id, TarefaDTO tarefaDTO)
        {
            return await _atualizarTarefa.ExecutarAsync(id, tarefaDTO);
        }

        public async Task<Resultado> ExcluirAsync(string id)
        {
            return await _excluirTarefa.ExecutarAsync(id);
        }

        public Task<Resultado<IReadOnlyCollection<TarefaDTO>>> ObterTodosAsync()
            => _obterTarefa.ObterTodosAsync();

        public Task<Resultado<TarefaDTO>> ObterPorId(int id)
            => _obterTarefa.ExecutarAsync(id);
    }
}
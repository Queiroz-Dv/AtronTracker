using Application.DTO;
using Application.UseCases.UsuarioCases;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.TarefaCases
{
    public sealed class ObterEquipeCase(
        ObterUsuarioCase usuarioService,
        ITarefaRepository tarefaRepository,
        IToDtoMapper<Tarefa, TarefaDTO> mapper)
    {
        private readonly ObterUsuarioCase _usuarioService = usuarioService;
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly IToDtoMapper<Tarefa, TarefaDTO> _mapper = mapper;

        public async Task<Resultado<IReadOnlyCollection<TarefaDTO>>> ExecutarAsync()
        {
            var usuarioResultado = await _usuarioService.ObterAsync();
            if (usuarioResultado.TeveFalha)
                return Resultado<IReadOnlyCollection<TarefaDTO>>.Falhas(usuarioResultado.Messages);

            var usuario = usuarioResultado.Dados;
            var tarefas = await _tarefaRepository.ObterTarefasAtivasPorSubordinadosDiretosAsync(
                usuario.Id,
                usuario.Codigo);
            var dtos = _mapper.MapToDtos(tarefas).ToList();

            return Resultado<IReadOnlyCollection<TarefaDTO>>.Sucesso(dtos);
        }
    }
}
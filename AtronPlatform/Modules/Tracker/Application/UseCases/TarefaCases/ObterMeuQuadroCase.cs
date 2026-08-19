using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases
{
    public sealed class ObterMeuQuadroCase(
        IUsuarioService usuarioService,
        ITarefaRepository tarefaRepository,
        IToDtoMapper<Tarefa, TarefaDTO> mapper)
    {
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly IToDtoMapper<Tarefa, TarefaDTO> _mapper = mapper;

        public async Task<Resultado<IReadOnlyCollection<TarefaDTO>>> ExecutarAsync()
        {
            var usuarioResultado = await _usuarioService.ObterUsuarioAtual();
            if (usuarioResultado.TeveFalha)
                return Resultado<IReadOnlyCollection<TarefaDTO>>.Falhas(usuarioResultado.Messages);

            var usuario = usuarioResultado.Dados;
            var tarefas = await _tarefaRepository.ObterTarefasAtivasPorUsuarioAsync(
                usuario.Id,
                usuario.Codigo);
            var dtos = _mapper.MapToDtos(tarefas).ToList();

            return Resultado<IReadOnlyCollection<TarefaDTO>>.Sucesso(dtos);
        }
    }
}

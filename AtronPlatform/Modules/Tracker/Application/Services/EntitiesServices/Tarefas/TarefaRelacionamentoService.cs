using Application.DTO;
using Application.Policies.Tarefas;
using Domain.Entities;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaRelacionamentoService(
        TarefaUsuarioRelacionamentoService usuarioRelacionamentoService,
        TarefaDepartamentoCargoRelacionamentoService tarefaDepartamentoCargoRelacionamentoService)
    {
        private readonly TarefaUsuarioRelacionamentoService _usuarioRelacionamentoService = usuarioRelacionamentoService;
        private readonly TarefaDepartamentoCargoRelacionamentoService _tarefaDepartamentoCargoRelacionamentoService = tarefaDepartamentoCargoRelacionamentoService;

        public async Task<Resultado> RelacionarAsync(Tarefa tarefa, TarefaDTO tarefaDTO)
        {
            var usuarioResultado = await _usuarioRelacionamentoService.RelacionarAsync(tarefa, tarefaDTO);
            if (usuarioResultado.TeveFalha)
                return Resultado.Falha(usuarioResultado.Messages);

            var tarefaDepartamentoCargoResultado = await _tarefaDepartamentoCargoRelacionamentoService.RelacionarAsync(tarefa, tarefaDTO);
            if (tarefaDepartamentoCargoResultado.TeveFalha)
                return Resultado.Falha(tarefaDepartamentoCargoResultado.Messages);

            var usuario = usuarioResultado.Dados;
            var tarefaDepartamentoCargo = tarefaDepartamentoCargoResultado.Dados;

            var governancaResultado = TarefaGovernancaPolicy.Validar(usuario, tarefaDepartamentoCargo);
            if (governancaResultado.TeveFalha)
                return Resultado.Falha(governancaResultado.Messages);

            return Resultado.Sucesso();
        }
    }
}

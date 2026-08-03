using Application.DTO;
using Application.Services.EntitiesServices.Tarefas;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ITarefaPreparacaoService
    {
        Task<Resultado<TarefaPreparada>> PrepararParaPersistenciaAsync(TarefaDTO tarefaDTO);

        Task<Resultado<List<TarefaEstadoDTO>>> ObterEstadosAsync();
    }
}

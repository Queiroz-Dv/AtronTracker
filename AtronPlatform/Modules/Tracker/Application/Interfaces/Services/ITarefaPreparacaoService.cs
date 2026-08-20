using Application.DTO;
using Domain.Entities;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ITarefaPreparacaoService
    {
        Task<Resultado<Tarefa>> PrepararParaPersistenciaAsync(TarefaDTO tarefaDTO);
    }
}
